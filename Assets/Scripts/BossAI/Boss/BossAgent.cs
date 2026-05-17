using UnityEngine;
using UnityEngine.AI;

namespace BossAI
{
    public class BossAgent
    {
        public GameObject GameObject { get; }
        public Transform Transform { get; }
        public NavMeshAgent NavMesh { get; }
        public Animator Animator { get; }

        public Transform PlayerTransform { get; set; }

        public float VidaActual { get; set; }
        public float VidaMaxima { get; }
        public bool WasHit { get; set; }

        public float VidaPorcentaje => (VidaMaxima > 0f) ? (VidaActual / VidaMaxima * 100f) : 0f;

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
