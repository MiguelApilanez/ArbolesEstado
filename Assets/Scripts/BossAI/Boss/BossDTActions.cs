// =====================================================================
//  BossDTActions.cs  �  Nodos hoja (acciones) concretos del �rbol
//  para el boss 3D. Cada clase representa una decisi�n final del �rbol.
// =====================================================================

using UnityEngine;

namespace BossAI
{
    // =================================================================
    //  IdleDTAction
    // =================================================================
    public class IdleDTAction : DTActionNode
    {
        public override void Execute(BossAgent agent)
        {
            if (agent.NavMesh != null) agent.NavMesh.isStopped = true;
            agent.Animator?.SetBool("isMoving", false);
        }
    }

    // =================================================================
    //  ChaseDTAction
    // =================================================================
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

    // =================================================================
    //  MeleeAttackDTAction
    // =================================================================
    public class MeleeAttackDTAction : DTActionNode
    {
        private readonly float _danio;
        private readonly float _rangoAtaque;
        private readonly string _animTrigger;

        public MeleeAttackDTAction(float danio, float rangoAtaque,
                                   string animTrigger = "attack")
        {
            _danio = danio;
            _rangoAtaque = rangoAtaque;
            _animTrigger = animTrigger;
        }

        public override void Execute(BossAgent agent)
        {
            if (agent.NavMesh != null) agent.NavMesh.isStopped = true;
            agent.Animator?.SetTrigger(_animTrigger);

            // El daño lo aplica BossAttack.DealDamage() desde el Animation Event
        }
    }

    // =================================================================
    //  RangedAttackDTAction
    // =================================================================
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
            // Aqu� lanzar�as tu proyectil, raycast, etc.
            Debug.Log("[Boss DT] Ataque a distancia ejecutado.");
        }
    }

    // =================================================================
    //  EnrageDTAction
    // =================================================================
    public class EnrageDTAction : DTActionNode
    {
        public override void Execute(BossAgent agent)
        {
            agent.Animator?.SetTrigger("enrage");
            if (agent.NavMesh != null) agent.NavMesh.speed *= 1.5f;
            Debug.Log("[Boss DT] �ENRAGE activado desde �rbol de decisi�n!");
        }
    }
}