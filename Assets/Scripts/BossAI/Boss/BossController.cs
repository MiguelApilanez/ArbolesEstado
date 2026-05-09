// =====================================================================
//  BossController.cs  —  MonoBehaviour principal del boss
//  Conecta la FSM generalista, la FSM jerárquica y el árbol de decisión
// =====================================================================

using UnityEngine;
using UnityEngine.AI;

namespace BossAI
{
    /// <summary>
    /// Componente principal del boss 3D.
    /// 
    /// Pon este script en el GameObject del boss junto con:
    ///   - NavMeshAgent
    ///   - Animator
    /// 
    /// Asigna en el Inspector:
    ///   - playerTransform
    ///   - waypoints (para la patrulla)
    ///   - vida máxima
    ///   - modo de IA (FSM generalista / FSM jerárquica / árbol de decisión)
    /// </summary>
    public class BossController : MonoBehaviour
    {
        // ── Inspector ────────────────────────────────────────────────
        [Header("Referencias")]
        public Transform playerTransform;
        public Transform[] waypoints;

        [Header("Stats")]
        public float vidaMaxima = 200f;

        [Header("Modo de IA")]
        public IAMode modoIA = IAMode.FSMGeneralista;

        public enum IAMode
        {
            FSMGeneralista,
            FSMJerarquica,
            ArbolDecision
        }

        // ── Internos ─────────────────────────────────────────────────
        private BossAgent _agent;
        private StateMachine _fsm;
        private DecisionTreeNode _arbolDecision;

        // =================================================================
        //  Unity Lifecycle
        // =================================================================

        private void Awake()
        {
            _agent = new BossAgent(gameObject, vidaMaxima)
            {
                PlayerTransform = playerTransform
            };
        }

        private void Start()
        {
            switch (modoIA)
            {
                case IAMode.FSMGeneralista:
                    _fsm = BossSetup.CrearFSMGeneralista(_agent, waypoints);
                    _fsm.Init();
                    break;

                case IAMode.FSMJerarquica:
                    _fsm = BossSetup.CrearFSMJerarquica(_agent, waypoints);
                    _fsm.Init();
                    break;

                case IAMode.ArbolDecision:
                    _arbolDecision = BossSetup.CrearArbolDecision(_agent);
                    break;
            }
        }

        private void Update()
        {
            // Sincronizar referencia al jugador (por si cambia en runtime)
            if (_agent != null) _agent.PlayerTransform = playerTransform;

            switch (modoIA)
            {
                case IAMode.FSMGeneralista:
                case IAMode.FSMJerarquica:
                    EjecutarFSM();
                    break;

                case IAMode.ArbolDecision:
                    EjecutarArbol();
                    break;
            }
        }

        // =================================================================
        //  Ejecución FSM
        // =================================================================

        private void EjecutarFSM()
        {
            if (_fsm == null) return;

            AIAction[] acciones = _fsm.Update();
            foreach (AIAction accion in acciones)
                accion.Execute(_agent);
        }

        // =================================================================
        //  Ejecución Árbol de Decisión
        // =================================================================

        private void EjecutarArbol()
        {
            if (_arbolDecision == null) return;

            DecisionTreeNode nodoResultado = _arbolDecision.Evaluate();

            if (nodoResultado is DTActionNode accionDT)
                accionDT.Execute(_agent);
        }

        // =================================================================
        //  API Pública (llamar desde otros sistemas de juego)
        // =================================================================

        /// <summary>
        /// Aplica daño al boss. Llamar desde proyectiles, hitboxes, etc.
        /// </summary>
        public void RecibirDanio(float cantidad)
        {
            _agent.VidaActual = Mathf.Max(0f, _agent.VidaActual - cantidad);
            Debug.Log($"[Boss] Vida: {_agent.VidaActual}/{_agent.VidaMaxima} " +
                      $"({_agent.VidaPorcentaje:F0}%)");
        }

        /// <summary>Devuelve true si el boss ha muerto.</summary>
        public bool EstaMuerto() => _agent.VidaActual <= 0f;
    }
}