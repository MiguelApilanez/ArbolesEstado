using UnityEngine;

namespace BossAI
{
    public class IdleDTAction : DTActionNode
    {
        public override void Execute(BossAgent agent)
        {
            if (agent.NavMesh != null) agent.NavMesh.isStopped = true;
            agent.Animator?.SetBool("isMoving", false);
        }
    }

    public class ChaseDTAction : DTActionNode
    {
        public override void Execute(BossAgent agent)
        {
            if (agent.PlayerTransform == null) return;
            if (agent.NavMesh != null)
            {
                agent.NavMesh.isStopped = false;
                agent.NavMesh.destination = agent.PlayerTransform.position;
                agent.Animator?.SetBool("isMoving", true);
            }
        }
    }

    public class MeleeAttackDTAction : DTActionNode
    {
        private readonly float _danio;
        private readonly float _rangoAtaque;
        private readonly string _animTrigger;

        public MeleeAttackDTAction(float danio, float rangoAtaque, string animTrigger = "attack")
        {
            _danio = danio;
            _rangoAtaque = rangoAtaque;
            _animTrigger = animTrigger;
        }

        public override void Execute(BossAgent agent)
        {
            if (agent.NavMesh != null) agent.NavMesh.isStopped = true;
            agent.Animator?.SetTrigger(_animTrigger);
        }
    }

    public class RangedAttackDTAction : DTActionNode
    {
        private readonly string _animTrigger;

        public RangedAttackDTAction(string animTrigger = "rangedAttack")
        {
            _animTrigger = animTrigger;
        }

        public override void Execute(BossAgent agent)
        {
            if (agent.NavMesh != null) agent.NavMesh.isStopped = true;
            agent.Animator?.SetTrigger(_animTrigger);
            Debug.Log("[Boss DT] Ataque a distancia ejecutado.");
        }
    }

    public class EnrageDTAction : DTActionNode
    {
        public override void Execute(BossAgent agent)
        {
            agent.Animator?.SetTrigger("enrage");
            if (agent.NavMesh != null) agent.NavMesh.speed *= 1.5f;
            Debug.Log("[Boss DT] ENRAGE activado desde arbol de decision.");
        }
    }
}
