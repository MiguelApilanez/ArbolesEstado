// =====================================================================
//  BossActions.cs  —  Acciones concretas del boss 3D
//  Adapta y amplía estas acciones según las necesidades de tu boss.
// =====================================================================

using BossAI;
using UnityEngine;

namespace BossAI
{
    // =================================================================
    //  IdleAction  —  El boss espera sin moverse
    // =================================================================

    /// <summary>
    /// Acción de idle: detiene el NavMeshAgent y activa la animación de espera.
    /// </summary>
    public class IdleAction : AIAction
    {
        public override void Execute(BossAgent agent)
        {
            if (agent.NavMesh != null)
                agent.NavMesh.isStopped = true;

            agent.Animator?.SetFloat("Speed", 0f);
        }
    }

    // =================================================================
    //  PatrolAction  —  El boss patrulla entre puntos de ruta
    // =================================================================

    /// <summary>
    /// Acción de patrulla: mueve el boss hacia el siguiente waypoint en bucle.
    /// </summary>
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

                // Avanzar al siguiente waypoint si hemos llegado
                if (agent.NavMesh.remainingDistance <= _rangoLlegada)
                    _waypointActual = (_waypointActual + 1) % _waypoints.Length;
            }
        }
    }

    // =================================================================
    //  ChaseAction  —  El boss persigue al jugador
    // =================================================================

    /// <summary>
    /// Acción de persecución: el boss se mueve hacia la posición del jugador cada frame.
    /// </summary>
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

    // =================================================================
    //  MeleeAttackAction  —  Ataque cuerpo a cuerpo
    // =================================================================

    /// <summary>
    /// Acción de ataque melee: dispara un trigger de animación y aplica daño.
    /// </summary>
    public class MeleeAttackAction : AIAction
    {
        private readonly float _danio;
        private readonly float _rangoAtaque;

        private float _cooldownAtaque = 2f;
        private float _ultimoAtaque = -999f;

        public MeleeAttackAction(float danio, float rangoAtaque)
        {
            _danio = danio;
            _rangoAtaque = rangoAtaque;
        }

        public override void Execute(BossAgent agent)
        {
            if (agent.PlayerTransform == null)
                return;

            // Distancia al jugador
            float dist = Vector3.Distance(
                agent.Transform.position,
                agent.PlayerTransform.position
            );

            // Si está fuera de rango
            if (dist > _rangoAtaque)
                return;

            // Mirar al jugador
            Vector3 dir =
                (agent.PlayerTransform.position -
                 agent.Transform.position).normalized;

            dir.y = 0f;

            if (dir != Vector3.zero)
            {
                agent.Transform.rotation =
                    Quaternion.LookRotation(dir);
            }

            // Parar movimiento
            if (agent.NavMesh != null)
            {
                agent.NavMesh.isStopped = true;
            }

            // =========================
            // COOLDOWN ATAQUE
            // =========================

            if (Time.time < _ultimoAtaque + 2f)
                return;

            _ultimoAtaque = Time.time;

            // =========================
            // LLAMAR A BOSSCOMBAT
            // =========================

            BossCombat combat =
                agent.GameObject.GetComponent<BossCombat>();

            if (combat != null)
            {
                combat.Attack();
            }

            // =========================
            // DAÑO
            // =========================

            var playerHealth =
                agent.PlayerTransform.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(_danio);
            }
        }
    }

    // =================================================================
    //  EnrageEnterAction  —  Acción de entrada a la fase de enrage
    // =================================================================

    /// <summary>
    /// Acción de ENTRADA al estado de enrage.
    /// Se ejecuta una sola vez al entrar en la fase de enrage del boss.
    /// </summary>
    public class EnrageEnterAction : AIAction
    {
        public override void Execute(BossAgent agent)
        {
            Debug.Log("[Boss] ¡enrage activado!");
            agent.Animator?.SetTrigger("Enrage");

            // Aumentar velocidad del NavMesh al entrar en enrage
            if (agent.NavMesh != null)
                agent.NavMesh.speed *= 1.5f;
        }
    }

    // =================================================================
    //  LogAction  —  Acción de debug para testear la FSM
    // =================================================================

    /// <summary>
    /// Acción de debug que imprime un mensaje en consola.
    /// Útil para verificar que la FSM cambia de estado correctamente.
    /// </summary>
    public class LogAction : AIAction
    {
        private readonly string _mensaje;

        public LogAction(string mensaje) { _mensaje = mensaje; }

        public override void Execute(BossAgent agent)
        {
            Debug.Log($"[Boss FSM] {_mensaje}");
        }
    }
}


// ─────────────────────────────────────────────────────────────────────
// Stub de PlayerHealth — reemplaza por tu componente real
// ─────────────────────────────────────────────────────────────────────
namespace BossAI
{
    // Elimina esta clase si ya tienes tu propio sistema de vida del jugador
    public class PlayerHealth : UnityEngine.MonoBehaviour
    {
        public float vida = 100f;
        public void TakeDamage(float cantidad) => vida -= cantidad;
    }
}