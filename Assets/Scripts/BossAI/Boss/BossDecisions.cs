// =====================================================================
//  BossDecisions.cs  —  Nodos decisión concretos para el boss 3D
// =====================================================================

using UnityEngine;

namespace BossAI
{
    // =================================================================
    //  DistanceDecision  —  ¿Está el jugador a menos de X unidades?
    // =================================================================

    /// <summary>
    /// Decisión basada en la distancia entre el boss y el jugador.
    /// → nodoTrue  si la distancia es MENOR O IGUAL que el umbral.
    /// → nodoFalse si la distancia es MAYOR que el umbral.
    /// 
    /// Ejemplo:
    ///   new DistanceDecision(agent, 5f, atacarCuerpo, atacarDistancia)
    /// </summary>
    public class DistanceDecision : Decision
    {
        private readonly BossAgent _agent;
        private readonly float _umbral;

        public DistanceDecision(BossAgent agent, float umbral,
                                DecisionTreeNode nodoTrue,
                                DecisionTreeNode nodoFalse)
            : base(nodoTrue, nodoFalse)
        {
            _agent = agent;
            _umbral = umbral;
            valor = umbral;
        }

        protected override bool TestCondition()
        {
            if (_agent.PlayerTransform == null) return false;
            float dist = Vector3.Distance(
                _agent.Transform.position,
                _agent.PlayerTransform.position);
            return dist <= _umbral;
        }
    }

    // =================================================================
    //  HealthDecision  —  ¿Tiene el boss menos del X% de vida?
    // =================================================================

    /// <summary>
    /// Decisión basada en el porcentaje de vida del boss.
    /// → nodoTrue  si la vida es MENOR O IGUAL que el umbral (%).
    /// → nodoFalse si la vida es MAYOR que el umbral (%).
    /// 
    /// Ejemplo (enrage al 50% de vida):
    ///   new HealthDecision(agent, 50f, nodoEnrage, nodoNormal)
    /// </summary>
    public class HealthDecision : Decision
    {
        private readonly BossAgent _agent;
        private readonly float _umbralPorcentaje;

        public HealthDecision(BossAgent agent, float umbralPorcentaje,
                              DecisionTreeNode nodoTrue,
                              DecisionTreeNode nodoFalse)
            : base(nodoTrue, nodoFalse)
        {
            _agent = agent;
            _umbralPorcentaje = umbralPorcentaje;
            valor = umbralPorcentaje;
        }

        protected override bool TestCondition()
        {
            return _agent.VidaPorcentaje <= _umbralPorcentaje;
        }
    }

    // =================================================================
    //  BoolDecision  —  Decisión por función booleana arbitraria
    // =================================================================

    /// <summary>
    /// Decisión genérica evaluada por una función lambda.
    /// Útil para cualquier condición de juego.
    /// 
    /// Ejemplo:
    ///   new BoolDecision(() => boss.EstaEnRango, nodoAtacar, nodoEsperar)
    /// </summary>
    public class BoolDecision : Decision
    {
        private readonly System.Func<bool> _condicion;

        public BoolDecision(System.Func<bool> condicion,
                            DecisionTreeNode nodoTrue,
                            DecisionTreeNode nodoFalse)
            : base(nodoTrue, nodoFalse)
        {
            _condicion = condicion;
        }

        protected override bool TestCondition() => _condicion();
    }
}