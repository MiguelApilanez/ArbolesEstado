using UnityEngine;
using UnityEngine.AI;

namespace BossAI
{
    public class BossController : MonoBehaviour
    {
        [Header("Referencias")]
        public Transform playerTransform;
        public Transform[] waypoints;

        [Header("Modo de IA")]
        public IAMode modoIA = IAMode.FSMGeneralista;

        [Header("Muerte")]
        [SerializeField] private float duracionAnimMuerte = 3f;

        public enum IAMode
        {
            FSMGeneralista,
            FSMJerarquica,
            ArbolDecision,
            ArbolComportamiento
        }

        private Animator animator;
        private NavMeshAgent navAgent;
        private BossCombat bossCombat;
        private BossHealth bossHealth;

        private BossAgent _agent;
        private StateMachine _fsm;
        private DecisionTreeNode _arbolDecision;
        private BTNode _behaviorTree;
        private bool _muriendo = false;

        private void Awake()
        {
            animator   = GetComponent<Animator>();
            navAgent   = GetComponent<NavMeshAgent>();
            bossCombat = GetComponent<BossCombat>();
            bossHealth = GetComponent<BossHealth>();

            float vidaMaxima = bossHealth != null ? bossHealth.maxHealth : 200f;

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

                case IAMode.ArbolComportamiento:
                    _behaviorTree = BossSetup.CrearBehaviorTree(_agent, waypoints);
                    break;
            }
        }

        private void Update()
        {
            if (_muriendo) return;

            if (_agent != null)
                _agent.PlayerTransform = playerTransform;

            switch (modoIA)
            {
                case IAMode.FSMGeneralista:
                case IAMode.FSMJerarquica:
                    EjecutarFSM();
                    break;

                case IAMode.ArbolDecision:
                    EjecutarArbolDecision();
                    break;

                case IAMode.ArbolComportamiento:
                    EjecutarBT();
                    break;
            }

            ActualizarAnimaciones();

            if (EstaMuerto())
                StartCoroutine(SecuenciaMuerte());
        }

        private System.Collections.IEnumerator SecuenciaMuerte()
        {
            _muriendo = true;

            if (navAgent != null)
            {
                navAgent.isStopped = true;
                navAgent.enabled = false;
            }

            if (bossCombat != null) bossCombat.enabled = false;

            animator?.SetTrigger("Die");

            if (animator != null)
            {
                yield return null;
                yield return new WaitUntil(() =>
                    animator.GetCurrentAnimatorStateInfo(0).IsName("Die"));
                yield return new WaitUntil(() =>
                    animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f);
            }
            else
            {
                yield return new WaitForSeconds(duracionAnimMuerte);
            }

            gameObject.SetActive(false);
        }

        private void EjecutarFSM()
        {
            if (_fsm == null) return;

            AIAction[] acciones = _fsm.Update();
            foreach (AIAction accion in acciones)
                accion.Execute(_agent);
        }

        private void EjecutarArbolDecision()
        {
            if (_arbolDecision == null) return;

            DecisionTreeNode nodo = _arbolDecision.Evaluate();
            if (nodo is DTActionNode accionDT)
                accionDT.Execute(_agent);
        }

        private void EjecutarBT()
        {
            _behaviorTree?.Execute(_agent);
        }

        private void ActualizarAnimaciones()
        {
            if (animator == null || navAgent == null) return;
            animator.SetFloat("Speed", navAgent.velocity.magnitude);
        }

        public void RecibirDanio(float cantidad)
        {
            if (_muriendo) return;
            if (_agent == null) return;

            _agent.VidaActual = Mathf.Max(0f, _agent.VidaActual - cantidad);
            _agent.WasHit = true;

            bossHealth?.TakeDamage(cantidad);
            animator?.SetTrigger("Hit");

            if (_agent.VidaPorcentaje <= 40f && bossCombat != null && !bossCombat.HasEnraged())
                bossCombat.EnterPhase2();
        }

        public bool EstaMuerto() => _agent != null && _agent.VidaActual <= 0f;
    }
}
