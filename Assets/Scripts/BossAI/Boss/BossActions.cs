using BossAI;
using UnityEngine;

namespace BossAI
{
    public class IdleAction : AIAction
    {
        public override void Execute(BossAgent agent)
        {
            if (agent.NavMesh != null)
                agent.NavMesh.isStopped = true;
            agent.Animator?.SetFloat("Speed", 0f);
        }
    }

    public class PatrolAction : AIAction
    {
        private readonly Transform[] _waypoints;
        private int _waypointActual = 0;
        private readonly float _rangoLlegada;

        public PatrolAction(Transform[] waypoints, float rangoLlegada = 1f)
        {
            _waypoints = waypoints;
            _rangoLlegada = rangoLlegada;
        }

        public override void Execute(BossAgent agent)
        {
            if (_waypoints == null || _waypoints.Length == 0) return;

            if (agent.NavMesh != null)
            {
                agent.NavMesh.isStopped = false;
                agent.NavMesh.destination = _waypoints[_waypointActual].position;
                agent.Animator?.SetFloat("Speed", 1f);

                float umbral = Mathf.Max(_rangoLlegada, agent.NavMesh.stoppingDistance + 0.5f);
                if (!agent.NavMesh.pathPending &&
                    agent.NavMesh.hasPath &&
                    agent.NavMesh.remainingDistance <= umbral)
                {
                    _waypointActual = (_waypointActual + 1) % _waypoints.Length;
                }
            }
        }
    }

    public class ChaseAction : AIAction
    {
        public override void Execute(BossAgent agent)
        {
            if (agent.PlayerTransform == null) return;

            if (agent.NavMesh != null)
            {
                agent.NavMesh.isStopped = false;
                agent.NavMesh.destination = agent.PlayerTransform.position;
                agent.Animator?.SetFloat("Speed", 1f);
            }
        }
    }

    public class MeleeAttackAction : AIAction
    {
        private readonly float _danio;
        private readonly float _rangoAtaque;
        private float _ultimoAtaque = -999f;

        public MeleeAttackAction(float danio, float rangoAtaque)
        {
            _danio = danio;
            _rangoAtaque = rangoAtaque;
        }

        public override void Execute(BossAgent agent)
        {
            if (agent.PlayerTransform == null) return;

            float dist = Vector3.Distance(agent.Transform.position, agent.PlayerTransform.position);
            if (dist > _rangoAtaque) return;

            Vector3 dir = (agent.PlayerTransform.position - agent.Transform.position).normalized;
            dir.y = 0f;
            if (dir != Vector3.zero)
                agent.Transform.rotation = Quaternion.LookRotation(dir);

            if (agent.NavMesh != null)
                agent.NavMesh.isStopped = true;

            if (Time.time < _ultimoAtaque + 2f) return;
            _ultimoAtaque = Time.time;

            BossCombat combat = agent.GameObject.GetComponent<BossCombat>();
            if (combat != null)
            {
                if (combat.IsEnraging()) return;
                combat.Attack();
            }
        }
    }

    public class EnrageEnterAction : AIAction
    {
        public override void Execute(BossAgent agent)
        {
            agent.Animator?.SetTrigger("Enrage");
            if (agent.NavMesh != null)
                agent.NavMesh.speed *= 1.5f;
        }
    }

    public class LogAction : AIAction
    {
        private readonly string _mensaje;
        public LogAction(string mensaje) { _mensaje = mensaje; }

        public override void Execute(BossAgent agent)
        {
            Debug.Log($"[Boss FSM] {_mensaje}");
        }
    }

    public class RangedAttackAction : AIAction
    {
        private readonly float _rangoMaximo;
        private float _ultimoAtaque = -999f;
        private const float _cooldown = 3f;

        public RangedAttackAction(float danio = 15f, float rangoMaximo = 10f)
        {
            _rangoMaximo = rangoMaximo;
        }

        public override void Execute(BossAgent agent)
        {
            if (agent.PlayerTransform == null) return;

            float dist = Vector3.Distance(agent.Transform.position, agent.PlayerTransform.position);
            if (dist > _rangoMaximo) return;

            Vector3 dir = (agent.PlayerTransform.position - agent.Transform.position).normalized;
            dir.y = 0f;
            if (dir != Vector3.zero)
                agent.Transform.rotation = Quaternion.LookRotation(dir);

            if (agent.NavMesh != null) agent.NavMesh.isStopped = true;

            if (Time.time < _ultimoAtaque + _cooldown) return;
            _ultimoAtaque = Time.time;

            agent.Animator?.SetTrigger("Attack3");
        }
    }

    public class CounterAttackAction : AIAction
    {
        private float _ultimoContra = -999f;
        private const float _cooldown = 4f;

        public override void Execute(BossAgent agent)
        {
            if (!agent.WasHit) return;
            if (Time.time < _ultimoContra + _cooldown) return;

            _ultimoContra = Time.time;
            agent.WasHit = false;

            if (agent.NavMesh != null) agent.NavMesh.isStopped = true;

            if (agent.PlayerTransform != null)
            {
                Vector3 dir = (agent.PlayerTransform.position - agent.Transform.position).normalized;
                dir.y = 0f;
                if (dir != Vector3.zero)
                    agent.Transform.rotation = Quaternion.LookRotation(dir);
            }

            agent.Animator?.SetTrigger("Attack2");
            Debug.Log("¡CONTRAATAQUE!");
        }
    }

    public class StopNavAction : AIAction
    {
        public override void Execute(BossAgent agent)
        {
            if (agent.NavMesh != null) agent.NavMesh.isStopped = true;
            agent.Animator?.SetFloat("Speed", 0f);
        }
    }

    public class ResetHitFlagAction : AIAction
    {
        public override void Execute(BossAgent agent)
        {
            agent.WasHit = false;
        }
    }
}
