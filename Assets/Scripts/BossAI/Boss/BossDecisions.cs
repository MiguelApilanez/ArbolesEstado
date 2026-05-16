using UnityEngine;

namespace BossAI
{
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
