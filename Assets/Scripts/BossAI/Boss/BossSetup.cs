// =====================================================================
//  BossSetup.cs  —  Fábrica que conecta todos los sistemas del boss
//  Aquí se definen los estados, transiciones y el árbol de decisión
//  MODIFICA ESTE ARCHIVO para personalizar el comportamiento del boss
// =====================================================================

using UnityEngine;

namespace BossAI
{
    /// <summary>
    /// Clase estática que construye las tres variantes de IA del boss.
    /// Separa la configuración del comportamiento del código de ejecución.
    /// </summary>
    public static class BossSetup
    {
        // =================================================================
        //  FSM GENERALISTA
        //  Estados: Patrulla → Persecución → Ataque → Enrage
        //  Reglas sencillas sin jerarquía
        // =================================================================

        public static StateMachine CrearFSMGeneralista(BossAgent agent,Transform[] waypoints)
        {
            // ── Acciones reutilizables ────────────────────────────────
            var idleAction = new IdleAction();
            var chaseAction = new ChaseAction();
            var patrolAction = new PatrolAction(waypoints);
            var meleeAction = new MeleeAttackAction(danio: 20f, rangoAtaque: 2f);
            var enrageEnter = new EnrageEnterAction();
            var logPatrullar = new LogAction("Entrando en PATRULLA");
            var logPerseguir = new LogAction("Entrando en PERSECUCIÓN");
            var logAtacar = new LogAction("Entrando en ATAQUE");
            var logEnrage = new LogAction("Entrando en ENRAGE");

            // ── Crear estados (sin transiciones todavía) ──────────────
            var estadoPatrulla = new State(
                accionesEstado: new AIAction[] { patrolAction },
                accionesEntrada: new AIAction[] { logPatrullar }
            );

            var estadoPersecucion = new State(
                accionesEstado: new AIAction[] { chaseAction },
                accionesEntrada: new AIAction[] { logPerseguir }
            );

            var estadoAtaque = new State(
                accionesEstado: new AIAction[] { meleeAction },
                accionesEntrada: new AIAction[] { logAtacar }
            );

            var estadoEnrage = new State(
                accionesEstado: new AIAction[] { chaseAction, meleeAction },
                accionesEntrada: new AIAction[] { enrageEnter, logEnrage }
            );

            // ── Condiciones ───────────────────────────────────────────
            // Jugador detectado si está a menos de 15 unidades
            var detectadoCondicion = new DistanceCondition(agent, min: 0f, max: 15f);
            // Jugador perdido si está a más de 20 unidades
            var perdidoCondicion = new DistanceCondition(agent, min: 20f, max: float.MaxValue);
            // Rango cuerpo a cuerpo si está a menos de 2.5 unidades
            var meleeRangoCondicion = new DistanceCondition(agent, min: 0f, max: 2.5f);
            // Fuera de rango melee si está a más de 3 unidades
            var fueraMeleeCondicion = new DistanceCondition(agent, min: 3f, max: float.MaxValue);
            // Enrage al bajar del 40% de vida
            var enrageCondicion = new HealthCondition(agent, min: 0f, max: 40f);

            // ── Transiciones ──────────────────────────────────────────
            // Patrulla → Persecución (si el jugador entra en rango)
            estadoPatrulla.transiciones = new Transition[]
            {
                new Transition(estadoPersecucion, detectadoCondicion)
            };

            // Persecución → Ataque (si llega al rango melee)
            // Persecución → Patrulla (si pierde al jugador)
            estadoPersecucion.transiciones = new Transition[]
            {
                new Transition(estadoEnrage,     enrageCondicion),
                new Transition(estadoAtaque,     meleeRangoCondicion),
                new Transition(estadoPatrulla,   perdidoCondicion)
            };

            // Ataque → Persecución (si el jugador se aleja)
            // Ataque → Enrage (si baja la vida)
            estadoAtaque.transiciones = new Transition[]
            {
                new Transition(estadoEnrage,      enrageCondicion),
                new Transition(estadoPersecucion, fueraMeleeCondicion)
            };

            // Enrage → (sin salida, estado final hasta muerte del boss)
            estadoEnrage.transiciones = new Transition[0];

            // ── Construir la FSM ──────────────────────────────────────
            var fsm = new StateMachine { estadoInicial = estadoPatrulla };
            return fsm;
        }

        // =================================================================
        //  FSM JERÁRQUICA
        //  Estructura:
        //    [Raíz]
        //      ├── [Normal]
        //      │     ├── Patrulla
        //      │     └── Persecución
        //      └── [Enrage]
        //            ├── PersecuciónEnrage
        //            └── AtaqueEnrage
        //
        //  Las transiciones globales (ej: enrage) se ponen en el padre.
        // =================================================================

        public static StateMachine CrearFSMJerarquica(BossAgent agent,
                                                       Transform[] waypoints)
        {
            var chaseAction = new ChaseAction();
            var patrolAction = new PatrolAction(waypoints);
            var meleeAction = new MeleeAttackAction(danio: 30f, rangoAtaque: 2f);
            var enrageEnter = new EnrageEnterAction();

            // ── Estados padre ─────────────────────────────────────────
            var estadoRaiz = new HierarchicalState();   // estado raíz sin acciones
            var estadoNormal = new HierarchicalState(estadoPadre: estadoRaiz);
            var estadoEnrage = new HierarchicalState(
                accionesEntrada: new AIAction[] { enrageEnter },
                estadoPadre: estadoRaiz);

            // ── Estados hoja del grupo Normal ─────────────────────────
            var estadoPatrulla = new HierarchicalState(
                accionesEstado: new AIAction[] { patrolAction },
                estadoPadre: estadoNormal);

            var estadoPersecucion = new HierarchicalState(
                accionesEstado: new AIAction[] { chaseAction },
                estadoPadre: estadoNormal);

            // ── Estados hoja del grupo Enrage ─────────────────────────
            var estadoEnrageChase = new HierarchicalState(
                accionesEstado: new AIAction[] { chaseAction },
                estadoPadre: estadoEnrage);

            var estadoEnrageAtaque = new HierarchicalState(
                accionesEstado: new AIAction[] { meleeAction },
                estadoPadre: estadoEnrage);

            // ── Condiciones ───────────────────────────────────────────
            var detectado = new DistanceCondition(agent, 0f, 15f);
            var perdido = new DistanceCondition(agent, 20f, float.MaxValue);
            var enEnrage = new HealthCondition(agent, 0f, 40f);
            var enMeleeRango = new DistanceCondition(agent, 0f, 2.5f);
            var fueraMeleeRango = new DistanceCondition(agent, 3f, float.MaxValue);

            // ── Transiciones por estado ───────────────────────────────

            // La transición global de enrage va en el padre Normal
            estadoNormal.transiciones = new Transition[]
            {
                new Transition(estadoEnrageChase, enEnrage)
            };

            // Transiciones locales en los hijos de Normal
            estadoPatrulla.transiciones = new Transition[]
            {
                new Transition(estadoPersecucion, detectado)
            };

            estadoPersecucion.transiciones = new Transition[]
            {
                new Transition(estadoAtaqueProxy(estadoEnrageAtaque), enMeleeRango),
                new Transition(estadoPatrulla,                         perdido)
            };

            // Transiciones dentro del grupo Enrage
            estadoEnrageChase.transiciones = new Transition[]
            {
                new Transition(estadoEnrageAtaque, enMeleeRango)
            };

            estadoEnrageAtaque.transiciones = new Transition[]
            {
                new Transition(estadoEnrageChase, fueraMeleeRango)
            };

            // ── Construir la HSM ──────────────────────────────────────
            var hsm = new HierarchicalStateMachine
            {
                estadoInicial = estadoPatrulla
            };
            return hsm;
        }

        // Helper para evitar referencia circular en el setup inline
        private static HierarchicalState estadoAtaqueProxy(HierarchicalState destino) => destino;

        // =================================================================
        //  ÁRBOL DE DECISIÓN
        //  Estructura lógica:
        //
        //  ¿Enrage (vida < 40%)?
        //    Sí → ¿En rango melee?
        //           Sí → AtaqueEnrage
        //           No → PerseguirEnrage
        //    No → ¿Jugador detectado (< 15u)?
        //           Sí → ¿En rango melee?
        //                  Sí → AtaqueMelee
        //                  No → Perseguir
        //           No → Patrullar / Idle
        // =================================================================

        public static DecisionTreeNode CrearArbolDecision(BossAgent agent)
        {
            // ── Nodos hoja (acciones) ─────────────────────────────────
            var nodoIdle = new IdleDTAction();
            var nodoPerseguir = new ChaseDTAction();
            var nodoAtaqueMelee = new MeleeAttackDTAction(danio: 20f, rangoAtaque: 2f);
            var nodoAtaqueDistancia = new RangedAttackDTAction();
            var nodoEnrage = new EnrageDTAction();

            // ── Condiciones ───────────────────────────────────────────
            bool EstaEnMelee() => agent.PlayerTransform != null &&
                                     UnityEngine.Vector3.Distance(
                                         agent.Transform.position,
                                         agent.PlayerTransform.position) <= 2.5f;

            bool EstaDetectado() => agent.PlayerTransform != null &&
                                     UnityEngine.Vector3.Distance(
                                         agent.Transform.position,
                                         agent.PlayerTransform.position) <= 15f;

            bool EstaEnEnrage() => agent.VidaPorcentaje <= 40f;

            // ── Construir el árbol de dentro hacia afuera ─────────────

            // Subárbol: jugador detectado
            var decisionMelee = new BoolDecision(EstaEnMelee,
                nodoTrue: nodoAtaqueMelee,
                nodoFalse: nodoPerseguir);

            var decisionDetectado = new BoolDecision(EstaDetectado,
                nodoTrue: decisionMelee,
                nodoFalse: nodoIdle);

            // Subárbol: fase enrage
            var decisionMeleeEnrage = new BoolDecision(EstaEnMelee,
                nodoTrue: nodoEnrage,
                nodoFalse: nodoPerseguir);

            // Raíz del árbol: ¿enrage?
            var raiz = new BoolDecision(EstaEnEnrage,
                nodoTrue: decisionMeleeEnrage,
                nodoFalse: decisionDetectado);

            return raiz;
        }
    }
}