// =====================================================================
//  BossConditions.cs  —  Condiciones concretas para el boss 3D
// =====================================================================

using UnityEngine;

namespace BossAI
{
    // =================================================================
    //  DistanceCondition  —  ¿Está el jugador a cierta distancia?
    // =================================================================

    /// <summary>
    /// Condición que comprueba la distancia 3D entre el boss y el jugador.
    /// 
    /// Ejemplo de uso:
    ///   // True cuando el jugador está entre 0 y 5 unidades (rango cuerpo a cuerpo)
    ///   new DistanceCondition(agent) { valorMinimo = 0f, valorMaximo = 5f }
    /// </summary>
    public class DistanceCondition : FloatCondition
    {
        private readonly BossAgent _agent;

        public DistanceCondition(BossAgent agent, float min, float max)
        {
            _agent = agent;
            valorMinimo = min;
            valorMaximo = max;
        }

        /// <summary>Devuelve la distancia actual entre el boss y el jugador.</summary>
        public override float TestValue()
        {
            if (_agent.PlayerTransform == null) return float.MaxValue;
            return Vector3.Distance(_agent.Transform.position, _agent.PlayerTransform.position);
        }
    }

    // =================================================================
    //  HealthCondition  —  ¿Está la vida del boss en cierto rango (%)?
    // =================================================================

    /// <summary>
    /// Condición que comprueba el porcentaje de vida del boss.
    /// 
    /// Ejemplo de uso:
    ///   // True cuando el boss tiene menos del 50% de vida
    ///   new HealthCondition(agent) { valorMinimo = 0f, valorMaximo = 50f }
    /// </summary>
    public class HealthCondition : FloatCondition
    {
        private readonly BossAgent _agent;

        public HealthCondition(BossAgent agent, float min, float max)
        {
            _agent = agent;
            valorMinimo = min;
            valorMaximo = max;
        }

        /// <summary>Devuelve la vida actual del boss como porcentaje [0–100].</summary>
        public override float TestValue()
        {
            return _agent.VidaPorcentaje;
        }
    }

    // =================================================================
    //  BoolCondition  —  Condición booleana simple (flag de juego)
    // =================================================================

    /// <summary>
    /// Condición que evalúa una función booleana arbitraria.
    /// Útil para flags como "el jugador está atacando", "fase 2 activa", etc.
    /// 
    /// Ejemplo de uso:
    ///   new BoolCondition(() => bossData.fase2Activa)
    /// </summary>
    public class BoolCondition : Condition
    {
        private readonly System.Func<bool> _evaluador;

        public BoolCondition(System.Func<bool> evaluador)
        {
            _evaluador = evaluador;
        }

        public override bool Test() => _evaluador();
    }
}