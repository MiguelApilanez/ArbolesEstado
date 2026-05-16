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
        // ── Componentes ──────────────────────────────────────────────

        private Animator animator;
        private NavMeshAgent navAgent;

        private BossCombat bossCombat;
        private BossHealth bossHealth;

        // ── Internos ─────────────────────────────────────────────────
        private BossAgent _agent;
        private StateMachine _fsm;
        private DecisionTreeNode _arbolDecision;

        // =================================================================
        //  Unity Lifecycle
        // =================================================================

        private void Awake()
        {
            // Componentes Unity
            animator = GetComponent<Animator>();
            navAgent = GetComponent<NavMeshAgent>();

            // Sistemas gameplay
            bossCombat = GetComponent<BossCombat>();
            bossHealth = GetComponent<BossHealth>();

            //Agente IA
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
            // Sincronizar jugador
            if (_agent != null)
                _agent.PlayerTransform = playerTransform;

            // Ejecutar IA
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

            // Actualizar animación movimiento
            ActualizarAnimaciones();

            // Comprobar muerte
            if (EstaMuerto())
            {
                animator.SetTrigger("Die");

                enabled = false;
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

        //  Animaciones
        private void ActualizarAnimaciones()
        {
            if (animator == null || navAgent == null)
                return;

            float velocidad = navAgent.velocity.magnitude;

            animator.SetFloat("Speed", velocidad);
        }

        // =================================================================
        //  API Pública (llamar desde otros sistemas de juego)
        // =================================================================

        /// <summary>
        /// Aplica daño al boss. Llamar desde proyectiles, hitboxes, etc.
        /// </summary>
        public void RecibirDanio(float cantidad)
        {
            // Vida IA
            _agent.VidaActual = Mathf.Max(0f, _agent.VidaActual - cantidad);

            // Vida visual/UI
            if (bossHealth != null)
            {
                bossHealth.TakeDamage(cantidad);
            }

            // Animación hit
            animator.SetTrigger("Hit");

            Debug.Log($"[Boss] Vida: {_agent.VidaActual}/{_agent.VidaMaxima}");

            if (_agent.VidaPorcentaje <= 40f)
            {
                if (bossCombat != null)
                {
                    bossCombat.EnterPhase2();
                }
            }

            if (_agent.VidaActual <= 0f)
            {
                animator.SetTrigger("Die");
            }
        }

        /// <summary>Devuelve true si el boss ha muerto.</summary>
        public bool EstaMuerto()
        {
            return _agent.VidaActual <= 0f;
        }
    }
}