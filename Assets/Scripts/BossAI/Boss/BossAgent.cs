// =====================================================================
//  BossAgent.cs  —  Contexto del boss que se pasa a las acciones
// =====================================================================

using UnityEngine;
using UnityEngine.AI;

namespace BossAI
{
    /// <summary>
    /// Contenedor de referencias del boss que las acciones necesitan para operar.
    /// Evita que cada acción busque componentes individualmente con GetComponent.
    /// Añade aquí las referencias que vayan necesitando tus acciones concretas.
    /// </summary>
    public class BossAgent
    {
        // ── Componentes Unity ────────────────────────────────────────────
        public GameObject GameObject { get; }
        public Transform Transform { get; }
        public NavMeshAgent NavMesh { get; }
        public Animator Animator { get; }

        // ── Referencias del mundo de juego ───────────────────────────────
        public Transform PlayerTransform { get; set; }

        // ── Estado del boss ───────────────────────────────────────────────
        public float VidaActual { get; set; }
        public float VidaMaxima { get; }

        /// <summary>Vida del boss normalizada en tanto por ciento [0–100].</summary>
        public float VidaPorcentaje => (VidaMaxima > 0f) ? (VidaActual / VidaMaxima * 100f) : 0f;

        // -----------------------------------------------------------------

        public BossAgent(GameObject go, float vidaMaxima)
        {
            GameObject = go;
            Transform = go.transform;
            NavMesh = go.GetComponent<NavMeshAgent>();
            Animator = go.GetComponent<Animator>();
            VidaMaxima = vidaMaxima;
            VidaActual = vidaMaxima;
        }
    }
}