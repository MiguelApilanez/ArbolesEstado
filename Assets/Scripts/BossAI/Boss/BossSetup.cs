// =====================================================================
//  BossSetup.cs  —  Fábrica que conecta todos los sistemas del boss
// =====================================================================
//
//  FSM GENERALISTA — 8 estados:
//    1. Idle          — estado inicial, boss quieto
//    2. Patrulla      — recorre waypoints
//    3. Persecución   — persigue al jugador
//    4. AtaqueMelee   — ataque cuerpo a cuerpo (fase 1)
//    5. AtaqueDistancia — ataque a distancia media
//    6. Contraataque  — comportamiento reactivo al recibir daño
//    7. Enrage        — transición a fase 2
//    8. AtaqueEnrage  — ataque cuerpo a cuerpo (fase 2, más daño)
//
//  ÁRBOL DE COMPORTAMIENTO (BTSelector raíz):
//    ├── Seq[ActivarEnrage]  → BTEnrageLeaf
//    ├── Seq[Fase2]          → Selector[Melee2 | Chase]
//    ├── Seq[Contraataque]   → BTCounterLeaf
//    ├── Seq[Melee]          → CooldownDec(BTMeleeLeaf)      ← decorator
//    ├── Seq[Distancia]      → CooldownDec(BTRangedLeaf)     ← decorator
//    ├── Seq[Perseguir]      → BTChaseLeaf
//    └── BTPatrolLeaf
//
// =====================================================================

using UnityEngine;

namespace BossAI
{
    public static class BossSetup
    {
        // =================================================================
        //  FSM GENERALISTA — 8 estados
        // =================================================================

        public static StateMachine CrearFSMGeneralista(BossAgent agent,
                                                        Transform[] waypoints)
        {
            // ── Acciones ──────────────────────────────────────────────────
            var idleAction      = new IdleAction();
            var chaseAction     = new ChaseAction();
            var patrolAction    = new PatrolAction(waypoints);
            var meleeAction     = new MeleeAttackAction(danio: 20f, rangoAtaque: 4f);
            var meleeEnrage     = new MeleeAttackAction(danio: 35f, rangoAtaque: 4f);
            var rangedAction    = new RangedAttackAction(danio: 15f, rangoMaximo: 10f);
            var counterAction   = new CounterAttackAction();
            var enrageEnter     = new EnrageEnterAction();
            var stopNav         = new StopNavAction();
            var resetHit        = new ResetHitFlagAction();

            // Logs (acciones de entrada)
            var logIdle         = new LogAction("Entrando en IDLE");
            var logPatrulla     = new LogAction("Entrando en PATRULLA");
            var logPersecucion  = new LogAction("Entrando en PERSECUCIÓN");
            var logMelee        = new LogAction("Entrando en ATAQUE MELEE");
            var logDistancia    = new LogAction("Entrando en ATAQUE DISTANCIA");
            var logContra       = new LogAction("Entrando en CONTRAATAQUE");
            var logEnrage       = new LogAction("Entrando en ENRAGE — FASE 2");
            var logAtaqueEnrage = new LogAction("Entrando en ATAQUE ENRAGE");

            // ── Estados ───────────────────────────────────────────────────

            // 1. IDLE — estado inicial
            var estadoIdle = new State(
                accionesEstado:  new AIAction[] { idleAction },
                accionesEntrada: new AIAction[] { logIdle }
            );

            // 2. PATRULLA
            var estadoPatrulla = new State(
                accionesEstado:  new AIAction[] { patrolAction },
                accionesEntrada: new AIAction[] { logPatrulla }
            );

            // 3. PERSECUCIÓN
            //    Acción de SALIDA: StopNavAction  ← acción de salida #1
            var estadoPersecucion = new State(
                accionesEstado:  new AIAction[] { chaseAction },
                accionesEntrada: new AIAction[] { logPersecucion },
                accionesSalida:  new AIAction[] { stopNav }         // SALIDA #1
            );

            // 4. ATAQUE MELEE (fase 1)
            var estadoAtaqueMelee = new State(
                accionesEstado:  new AIAction[] { meleeAction },
                accionesEntrada: new AIAction[] { logMelee }
            );

            // 5. ATAQUE DISTANCIA (rango medio 4–10u)
            //    Acción de ENTRADA: StopNavAction  ← acción de entrada #1
            var estadoAtaqueDistancia = new State(
                accionesEstado:  new AIAction[] { rangedAction },
                accionesEntrada: new AIAction[] { stopNav, logDistancia }  // ENTRADA #1
            );

            // 6. CONTRAATAQUE — comportamiento reactivo
            //    Acción de ENTRADA: logContra       ← acción de entrada #2
            //    Acción de SALIDA:  resetHit        ← acción de salida #2
            var estadoContraataque = new State(
                accionesEstado:  new AIAction[] { counterAction },
                accionesEntrada: new AIAction[] { logContra },     // ENTRADA #2
                accionesSalida:  new AIAction[] { resetHit }       // SALIDA #2
            );

            // 7. ENRAGE — transición de fase (acción entrada: EnrageEnterAction)
            var estadoEnrage = new State(
                accionesEstado:  new AIAction[] { chaseAction },
                accionesEntrada: new AIAction[] { enrageEnter, logEnrage }
            );

            // 8. ATAQUE ENRAGE (fase 2, más daño)
            var estadoAtaqueEnrage = new State(
                accionesEstado:  new AIAction[] { meleeEnrage },
                accionesEntrada: new AIAction[] { logAtaqueEnrage }
            );

            // ── Condiciones ───────────────────────────────────────────────
            var detectado        = new DistanceCondition(agent,  0f,         15f);
            var perdido          = new DistanceCondition(agent, 20f,  float.MaxValue);
            var enMelee          = new DistanceCondition(agent,  0f,          4f);
            var fueraMelee       = new DistanceCondition(agent,  4f,  float.MaxValue);
            var enMedio          = new DistanceCondition(agent,  4f,         10f);
            var fueraMedio       = new DistanceCondition(agent, 10f,  float.MaxValue);

            var enrageCondicion = new BoolCondition(() =>
            {
                BossCombat c = agent.GameObject.GetComponent<BossCombat>();
                return c != null && agent.VidaPorcentaje <= 40f && !c.HasEnraged();
            });

            var wasHitCondicion    = new BoolCondition(() => agent.WasHit);
            var notWasHitCondicion = new BoolCondition(() => !agent.WasHit);
            var siempre            = new BoolCondition(() => true);

            // ── Transiciones ──────────────────────────────────────────────

            // Idle → Patrulla (inmediato al iniciar)
            estadoIdle.transiciones = new Transition[]
            {
                new Transition(estadoPatrulla, siempre)
            };

            // Patrulla → Persecución / Enrage
            estadoPatrulla.transiciones = new Transition[]
            {
                new Transition(estadoEnrage,      enrageCondicion),
                new Transition(estadoPersecucion, detectado)
            };

            // Persecución → Contraataque / Enrage / Melee / Distancia / Patrulla
            estadoPersecucion.transiciones = new Transition[]
            {
                new Transition(estadoEnrage,          enrageCondicion),
                new Transition(estadoContraataque,    wasHitCondicion),
                new Transition(estadoAtaqueMelee,     enMelee),
                new Transition(estadoAtaqueDistancia, enMedio),
                new Transition(estadoPatrulla,        perdido)
            };

            // AtaqueMelee → Enrage / Distancia / Persecución
            estadoAtaqueMelee.transiciones = new Transition[]
            {
                new Transition(estadoEnrage,          enrageCondicion),
                new Transition(estadoAtaqueDistancia, enMedio),
                new Transition(estadoPersecucion,     fueraMelee)
            };

            // AtaqueDistancia → Enrage / Melee / Persecución
            estadoAtaqueDistancia.transiciones = new Transition[]
            {
                new Transition(estadoEnrage,      enrageCondicion),
                new Transition(estadoAtaqueMelee, enMelee),
                new Transition(estadoPersecucion, fueraMedio)
            };

            // Contraataque → Persecución (cuando WasHit ya está limpio)
            estadoContraataque.transiciones = new Transition[]
            {
                new Transition(estadoPersecucion, notWasHitCondicion)
            };

            // Enrage → AtaqueEnrage / Persecución
            estadoEnrage.transiciones = new Transition[]
            {
                new Transition(estadoAtaqueEnrage, enMelee),
                new Transition(estadoPersecucion,  fueraMelee)
            };

            // AtaqueEnrage → Enrage (si el jugador se aleja)
            estadoAtaqueEnrage.transiciones = new Transition[]
            {
                new Transition(estadoEnrage, fueraMelee)
            };

            // ── Construir FSM ──────────────────────────────────────────────
            return new StateMachine { estadoInicial = estadoIdle };
        }

        // =================================================================
        //  FSM JERÁRQUICA — sin cambios respecto al original
        // =================================================================

        public static StateMachine CrearFSMJerarquica(BossAgent agent,
                                                       Transform[] waypoints)
        {
            var chaseAction  = new ChaseAction();
            var patrolAction = new PatrolAction(waypoints);
            var meleeAction  = new MeleeAttackAction(danio: 30f, rangoAtaque: 2f);
            var enrageEnter  = new EnrageEnterAction();

            var estadoRaiz   = new HierarchicalState();
            var estadoNormal = new HierarchicalState(estadoPadre: estadoRaiz);
            var estadoEnrage = new HierarchicalState(
                accionesEntrada: new AIAction[] { enrageEnter },
                estadoPadre: estadoRaiz);

            var estadoPatrulla = new HierarchicalState(
                accionesEstado: new AIAction[] { patrolAction },
                estadoPadre: estadoNormal);

            var estadoPersecucion = new HierarchicalState(
                accionesEstado: new AIAction[] { chaseAction },
                estadoPadre: estadoNormal);

            var estadoEnrageChase = new HierarchicalState(
                accionesEstado: new AIAction[] { chaseAction },
                estadoPadre: estadoEnrage);

            var estadoEnrageAtaque = new HierarchicalState(
                accionesEstado: new AIAction[] { meleeAction },
                estadoPadre: estadoEnrage);

            var detectado      = new DistanceCondition(agent,  0f,         15f);
            var perdido        = new DistanceCondition(agent, 20f, float.MaxValue);
            var enEnrage       = new HealthCondition(agent,    0f,         40f);
            var enMeleeRango   = new DistanceCondition(agent,  0f,          2.5f);
            var fueraMeleeRango= new DistanceCondition(agent,  3f, float.MaxValue);

            estadoNormal.transiciones = new Transition[]
            {
                new Transition(estadoEnrageChase, enEnrage)
            };

            estadoPatrulla.transiciones = new Transition[]
            {
                new Transition(estadoPersecucion, detectado)
            };

            estadoPersecucion.transiciones = new Transition[]
            {
                new Transition(estadoEnrageAtaque, enMeleeRango),
                new Transition(estadoPatrulla,     perdido)
            };

            estadoEnrageChase.transiciones = new Transition[]
            {
                new Transition(estadoEnrageAtaque, enMeleeRango)
            };

            estadoEnrageAtaque.transiciones = new Transition[]
            {
                new Transition(estadoEnrageChase, fueraMeleeRango)
            };

            return new HierarchicalStateMachine { estadoInicial = estadoPatrulla };
        }

        // =================================================================
        //  ÁRBOL DE COMPORTAMIENTO (Behavior Tree)
        //
        //  Estructura:
        //  BTSelector (raíz)
        //  ├── Seq[ActivarEnrage]  → BTEnrageLeaf
        //  ├── Seq[Fase2]          → Sel[ Seq[Melee2 + CooldownDec] | Chase ]
        //  ├── Seq[Contraataque]   → BTCounterLeaf
        //  ├── Seq[Melee]          → CooldownDec(BTMeleeLeaf, 2s)
        //  ├── Seq[Distancia]      → CooldownDec(BTRangedLeaf, 3s)
        //  ├── Seq[Perseguir]      → BTChaseLeaf
        //  └── BTPatrolLeaf
        // =================================================================

        public static BTNode CrearBehaviorTree(BossAgent agent, Transform[] waypoints)
        {
            // ── Hojas ─────────────────────────────────────────────────────
            var idle      = new BTIdleLeaf();
            var patrol    = new BTPatrolLeaf(waypoints);
            var chase     = new BTChaseLeaf();
            var melee     = new BTMeleeLeaf(danio: 20f);
            var melee2    = new BTMeleePhase2Leaf(danio: 35f);
            var ranged    = new BTRangedLeaf(danio: 15f);
            var counter   = new BTCounterLeaf();
            var enrage    = new BTEnrageLeaf();

            // ── Condiciones ───────────────────────────────────────────────
            var condEnMelee   = new BTCondition(a =>
                a.PlayerTransform != null &&
                Vector3.Distance(a.Transform.position, a.PlayerTransform.position) <= 4f);

            var condEnMedio   = new BTCondition(a =>
                a.PlayerTransform != null &&
                Vector3.Distance(a.Transform.position, a.PlayerTransform.position) is > 4f and <= 10f);

            var condDetectado = new BTCondition(a =>
                a.PlayerTransform != null &&
                Vector3.Distance(a.Transform.position, a.PlayerTransform.position) <= 15f);

            var condEnPhase2  = new BTCondition(a =>
                a.GameObject.GetComponent<BossCombat>()?.HasEnraged() ?? false);

            var condActivarEnrage = new BTCondition(a =>
            {
                BossCombat c = a.GameObject.GetComponent<BossCombat>();
                return c != null && a.VidaPorcentaje <= 40f && !c.HasEnraged();
            });

            var condWasHit = new BTCondition(a => a.WasHit);

            // ── Decorators ────────────────────────────────────────────────
            var meleeCooldown  = new BTCooldownDecorator(melee,  2f);   // decorator #1
            var rangedCooldown = new BTCooldownDecorator(ranged, 3f);   // decorator #2
            var melee2Cooldown = new BTCooldownDecorator(melee2, 1.5f); // decorator #3

            // ── Árbol ─────────────────────────────────────────────────────

            // Subárbol fase 2: melee rápido o perseguir
            var fase2Combate = new BTSelector(
                new BTSequence(condEnMelee, melee2Cooldown),
                chase
            );

            // Raíz: selector de prioridades
            return new BTSelector(
                // 1. Activar enrage si vida < 40% y no está ya activo
                new BTSequence(condActivarEnrage, enrage),

                // 2. Combate fase 2 (si ya está en enrage)
                new BTSequence(condEnPhase2, fase2Combate),

                // 3. Contraataque reactivo
                new BTSequence(condWasHit, counter),

                // 4. Ataque melee (< 4u)
                new BTSequence(condEnMelee, meleeCooldown),

                // 5. Ataque a distancia (4–10u)
                new BTSequence(condEnMedio, rangedCooldown),

                // 6. Perseguir (< 15u)
                new BTSequence(condDetectado, chase),

                // 7. Patrullar (por defecto)
                patrol
            );
        }

        // =================================================================
        //  ÁRBOL DE DECISIÓN (mantenido por compatibilidad)
        // =================================================================

        public static DecisionTreeNode CrearArbolDecision(BossAgent agent)
        {
            var nodoIdle          = new IdleDTAction();
            var nodoPerseguir     = new ChaseDTAction();
            var nodoAtaqueMelee   = new MeleeAttackDTAction(danio: 20f, rangoAtaque: 2f);
            var nodoAtaqueDistancia = new RangedAttackDTAction();
            var nodoEnrage        = new EnrageDTAction();

            bool EstaEnMelee()    => agent.PlayerTransform != null &&
                Vector3.Distance(agent.Transform.position,
                                 agent.PlayerTransform.position) <= 2.5f;
            bool EstaDetectado()  => agent.PlayerTransform != null &&
                Vector3.Distance(agent.Transform.position,
                                 agent.PlayerTransform.position) <= 15f;
            bool EstaEnEnrage()   => agent.VidaPorcentaje <= 40f;

            var decisionMelee      = new BoolDecision(EstaEnMelee,
                nodoTrue: nodoAtaqueMelee, nodoFalse: nodoPerseguir);
            var decisionDetectado  = new BoolDecision(EstaDetectado,
                nodoTrue: decisionMelee,   nodoFalse: nodoIdle);
            var decisionMeleeEnrage= new BoolDecision(EstaEnMelee,
                nodoTrue: nodoEnrage,      nodoFalse: nodoPerseguir);

            return new BoolDecision(EstaEnEnrage,
                nodoTrue: decisionMeleeEnrage, nodoFalse: decisionDetectado);
        }
    }
}
