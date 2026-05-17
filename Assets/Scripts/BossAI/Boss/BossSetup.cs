using UnityEngine;

namespace BossAI
{
    public static class BossSetup
    {
        public static StateMachine CrearFSMGeneralista(BossAgent agent, Transform[] waypoints)
        {
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

            var logIdle         = new LogAction("Entrando en IDLE");
            var logPatrulla     = new LogAction("Entrando en PATRULLA");
            var logPersecucion  = new LogAction("Entrando en PERSECUCION");
            var logMelee        = new LogAction("Entrando en ATAQUE MELEE");
            var logDistancia    = new LogAction("Entrando en ATAQUE DISTANCIA");
            var logContra       = new LogAction("Entrando en CONTRAATAQUE");
            var logEnrage       = new LogAction("Entrando en ENRAGE - FASE 2");
            var logAtaqueEnrage = new LogAction("Entrando en ATAQUE ENRAGE");

            var estadoIdle = new State(
                accionesEstado:  new AIAction[] { idleAction },
                accionesEntrada: new AIAction[] { logIdle }
            );

            var estadoPatrulla = new State(
                accionesEstado:  new AIAction[] { patrolAction },
                accionesEntrada: new AIAction[] { logPatrulla }
            );

            var estadoPersecucion = new State(
                accionesEstado:  new AIAction[] { chaseAction },
                accionesEntrada: new AIAction[] { logPersecucion },
                accionesSalida:  new AIAction[] { stopNav }
            );

            var estadoAtaqueMelee = new State(
                accionesEstado:  new AIAction[] { meleeAction },
                accionesEntrada: new AIAction[] { logMelee }
            );

            var estadoAtaqueDistancia = new State(
                accionesEstado:  new AIAction[] { rangedAction },
                accionesEntrada: new AIAction[] { stopNav, logDistancia }
            );

            var estadoContraataque = new State(
                accionesEstado:  new AIAction[] { counterAction },
                accionesEntrada: new AIAction[] { logContra },
                accionesSalida:  new AIAction[] { resetHit }
            );

            var estadoEnrage = new State(
                accionesEstado:  new AIAction[] { chaseAction },
                accionesEntrada: new AIAction[] { enrageEnter, logEnrage }
            );

            var estadoAtaqueEnrage = new State(
                accionesEstado:  new AIAction[] { meleeEnrage },
                accionesEntrada: new AIAction[] { logAtaqueEnrage }
            );

            var detectado        = new DistanceCondition(agent,  0f,         15f);
            var perdido          = new DistanceCondition(agent, 20f, float.MaxValue);
            var enMelee          = new DistanceCondition(agent,  0f,          4f);
            var fueraMelee       = new DistanceCondition(agent,  4f, float.MaxValue);
            var enMedio          = new DistanceCondition(agent,  4f,         10f);
            var fueraMedio       = new DistanceCondition(agent, 10f, float.MaxValue);

            var enrageCondicion = new BoolCondition(() =>
            {
                BossCombat c = agent.GameObject.GetComponent<BossCombat>();
                return c != null && agent.VidaPorcentaje <= 40f && !c.HasEnraged();
            });

            var wasHitCondicion    = new BoolCondition(() => agent.WasHit);
            var notWasHitCondicion = new BoolCondition(() => !agent.WasHit);
            var siempre            = new BoolCondition(() => true);

            estadoIdle.transiciones = new Transition[]
            {
                new Transition(estadoPatrulla, siempre)
            };

            estadoPatrulla.transiciones = new Transition[]
            {
                new Transition(estadoEnrage,      enrageCondicion),
                new Transition(estadoPersecucion, detectado)
            };

            estadoPersecucion.transiciones = new Transition[]
            {
                new Transition(estadoEnrage,          enrageCondicion),
                new Transition(estadoContraataque,    wasHitCondicion),
                new Transition(estadoAtaqueMelee,     enMelee),
                new Transition(estadoAtaqueDistancia, enMedio),
                new Transition(estadoPatrulla,        perdido)
            };

            estadoAtaqueMelee.transiciones = new Transition[]
            {
                new Transition(estadoEnrage,          enrageCondicion),
                new Transition(estadoAtaqueDistancia, enMedio),
                new Transition(estadoPersecucion,     fueraMelee)
            };

            estadoAtaqueDistancia.transiciones = new Transition[]
            {
                new Transition(estadoEnrage,      enrageCondicion),
                new Transition(estadoAtaqueMelee, enMelee),
                new Transition(estadoPersecucion, fueraMedio)
            };

            estadoContraataque.transiciones = new Transition[]
            {
                new Transition(estadoPersecucion, notWasHitCondicion)
            };

            estadoEnrage.transiciones = new Transition[]
            {
                new Transition(estadoAtaqueEnrage, enMelee),
                new Transition(estadoPersecucion,  fueraMelee)
            };

            estadoAtaqueEnrage.transiciones = new Transition[]
            {
                new Transition(estadoEnrage, fueraMelee)
            };

            return new StateMachine { estadoInicial = estadoIdle };
        }

        public static StateMachine CrearFSMJerarquica(BossAgent agent, Transform[] waypoints)
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

            var detectado       = new DistanceCondition(agent,  0f,         15f);
            var perdido         = new DistanceCondition(agent, 20f, float.MaxValue);
            var enEnrage        = new HealthCondition(agent,    0f,         40f);
            var enMeleeRango    = new DistanceCondition(agent,  0f,          2.5f);
            var fueraMeleeRango = new DistanceCondition(agent,  3f, float.MaxValue);

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

        public static BTNode CrearBehaviorTree(BossAgent agent, Transform[] waypoints)
        {
            var idle      = new BTIdleLeaf();
            var patrol    = new BTPatrolLeaf(waypoints);
            var chase     = new BTChaseLeaf();
            var melee     = new BTMeleeLeaf(danio: 20f);
            var melee2    = new BTMeleePhase2Leaf(danio: 35f);
            var ranged    = new BTRangedLeaf(danio: 15f);
            var counter   = new BTCounterLeaf();
            var enrage    = new BTEnrageLeaf();

            var condEnMelee = new BTCondition(a =>
                a.PlayerTransform != null &&
                Vector3.Distance(a.Transform.position, a.PlayerTransform.position) <= 4f);

            var condEnMedio = new BTCondition(a =>
                a.PlayerTransform != null &&
                Vector3.Distance(a.Transform.position, a.PlayerTransform.position) is > 4f and <= 10f);

            var condDetectado = new BTCondition(a =>
                a.PlayerTransform != null &&
                Vector3.Distance(a.Transform.position, a.PlayerTransform.position) <= 15f);

            var condEnPhase2 = new BTCondition(a =>
                a.GameObject.GetComponent<BossCombat>()?.HasEnraged() ?? false);

            var condActivarEnrage = new BTCondition(a =>
            {
                BossCombat c = a.GameObject.GetComponent<BossCombat>();
                return c != null && a.VidaPorcentaje <= 40f && !c.HasEnraged();
            });

            var condWasHit = new BTCondition(a => a.WasHit);

            var meleeCooldown  = new BTCooldownDecorator(melee,  2f);
            var rangedCooldown = new BTCooldownDecorator(ranged, 3f);
            var melee2Cooldown = new BTCooldownDecorator(melee2, 1.5f);

            var fase2Combate = new BTSelector(
                new BTSequence(condEnMelee, melee2Cooldown),
                chase
            );

            return new BTSelector(
                new BTSequence(condActivarEnrage, enrage),
                new BTSequence(condEnPhase2, fase2Combate),
                new BTSequence(condWasHit, counter),
                new BTSequence(condEnMelee, meleeCooldown),
                new BTSequence(condEnMedio, rangedCooldown),
                new BTSequence(condDetectado, chase),
                patrol
            );
        }

        public static DecisionTreeNode CrearArbolDecision(BossAgent agent)
        {
            var nodoIdle            = new IdleDTAction();
            var nodoPerseguir       = new ChaseDTAction();
            var nodoAtaqueMelee     = new MeleeAttackDTAction(danio: 20f, rangoAtaque: 2f);
            var nodoAtaqueDistancia = new RangedAttackDTAction();
            var nodoEnrage          = new EnrageDTAction();

            bool EstaEnMelee()   => agent.PlayerTransform != null &&
                Vector3.Distance(agent.Transform.position, agent.PlayerTransform.position) <= 2.5f;
            bool EstaDetectado() => agent.PlayerTransform != null &&
                Vector3.Distance(agent.Transform.position, agent.PlayerTransform.position) <= 15f;
            bool EstaEnEnrage()  => agent.VidaPorcentaje <= 40f;

            var decisionMelee       = new BoolDecision(EstaEnMelee,
                nodoTrue: nodoAtaqueMelee,     nodoFalse: nodoPerseguir);
            var decisionDetectado   = new BoolDecision(EstaDetectado,
                nodoTrue: decisionMelee,       nodoFalse: nodoIdle);
            var decisionMeleeEnrage = new BoolDecision(EstaEnMelee,
                nodoTrue: nodoEnrage,          nodoFalse: nodoPerseguir);

            return new BoolDecision(EstaEnEnrage,
                nodoTrue: decisionMeleeEnrage, nodoFalse: decisionDetectado);
        }
    }
}
