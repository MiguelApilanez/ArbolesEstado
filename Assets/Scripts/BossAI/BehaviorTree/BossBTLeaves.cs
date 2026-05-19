using UnityEngine;

namespace BossAI
{
    public class BTIdleLeaf : BTNode
    {
        public override BTStatus Execute(BossAgent agent)
        {
            if (agent.NavMesh != null) agent.NavMesh.isStopped = true;
            agent.Animator?.SetFloat("Speed", 0f);
            return BTStatus.Running;
        }
    }

    public class BTPatrolLeaf : BTNode
    {
        private readonly Transform[] _waypoints;
        private int _actual = 0;

        public BTPatrolLeaf(Transform[] waypoints) { _waypoints = waypoints; }

        public override BTStatus Execute(BossAgent agent)
        {
            if (_waypoints == null || _waypoints.Length == 0) return BTStatus.Failure;

            if (agent.NavMesh != null)
            {
                agent.NavMesh.isStopped = false;
                agent.NavMesh.destination = _waypoints[_actual].position;

                float umbral = Mathf.Max(1f, agent.NavMesh.stoppingDistance + 0.5f);
                if (!agent.NavMesh.pathPending &&
                    agent.NavMesh.hasPath &&
                    agent.NavMesh.remainingDistance <= umbral)
                {
                    _actual = (_actual + 1) % _waypoints.Length;
                }
            }
            return BTStatus.Running;
        }
    }

    public class BTChaseLeaf : BTNode
    {
        public override BTStatus Execute(BossAgent agent)
        {
            if (agent.PlayerTransform == null) return BTStatus.Failure;

            if (agent.NavMesh != null)
            {
                agent.NavMesh.isStopped = false;
                agent.NavMesh.destination = agent.PlayerTransform.position;
                agent.Animator?.SetFloat("Speed", 1f);
            }
            return BTStatus.Running;
        }
    }

    public class BTMeleeLeaf : BTNode
    {
        public BTMeleeLeaf(float danio = 20f) { }

        public override BTStatus Execute(BossAgent agent)
        {
            if (agent.PlayerTransform == null) return BTStatus.Failure;

            if (agent.NavMesh != null) agent.NavMesh.isStopped = true;

            Vector3 dir = (agent.PlayerTransform.position - agent.Transform.position).normalized;
            dir.y = 0f;
            if (dir != Vector3.zero)
                agent.Transform.rotation = Quaternion.LookRotation(dir);

            BossCombat combat = agent.GameObject.GetComponent<BossCombat>();
            if (combat != null && !combat.IsEnraging())
                combat.Attack();

            return BTStatus.Success;
        }
    }

    public class BTMeleePhase2Leaf : BTNode
    {
        public BTMeleePhase2Leaf(float danio = 30f) { }

        public override BTStatus Execute(BossAgent agent)
        {
            if (agent.PlayerTransform == null) return BTStatus.Failure;

            if (agent.NavMesh != null) agent.NavMesh.isStopped = true;

            Vector3 dir = (agent.PlayerTransform.position - agent.Transform.position).normalized;
            dir.y = 0f;
            if (dir != Vector3.zero)
                agent.Transform.rotation = Quaternion.LookRotation(dir);

            BossCombat combat = agent.GameObject.GetComponent<BossCombat>();
            if (combat != null && !combat.IsEnraging())
                combat.Attack();

            return BTStatus.Success;
        }
    }

    public class BTRangedLeaf : BTNode
    {
        public BTRangedLeaf(float danio = 15f) { }

        public override BTStatus Execute(BossAgent agent)
        {
            if (agent.PlayerTransform == null) return BTStatus.Failure;

            if (agent.NavMesh != null) agent.NavMesh.isStopped = true;

            Vector3 dir = (agent.PlayerTransform.position - agent.Transform.position).normalized;
            dir.y = 0f;
            if (dir != Vector3.zero)
                agent.Transform.rotation = Quaternion.LookRotation(dir);

            agent.Animator?.SetTrigger("Attack3");

            return BTStatus.Success;
        }
    }

    public class BTCounterLeaf : BTNode
    {
        public override BTStatus Execute(BossAgent agent)
        {
            if (!agent.WasHit) return BTStatus.Failure;

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

            return BTStatus.Success;
        }
    }

    public class BTEnrageLeaf : BTNode
    {
        public override BTStatus Execute(BossAgent agent)
        {
            BossCombat combat = agent.GameObject.GetComponent<BossCombat>();
            if (combat == null || combat.HasEnraged()) return BTStatus.Success;

            combat.EnterPhase2();
            return BTStatus.Success;
        }
    }
}
