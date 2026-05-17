using UnityEngine;

namespace BossAI
{
    public class DistanceCondition : FloatCondition
    {
        private readonly BossAgent _agent;

        public DistanceCondition(BossAgent agent, float min, float max)
        {
            _agent = agent;
            valorMinimo = min;
            valorMaximo = max;
        }

        public override float TestValue()
        {
            if (_agent.PlayerTransform == null) return float.MaxValue;
            return Vector3.Distance(_agent.Transform.position, _agent.PlayerTransform.position);
        }
    }

    public class HealthCondition : FloatCondition
    {
        private readonly BossAgent _agent;

        public HealthCondition(BossAgent agent, float min, float max)
        {
            _agent = agent;
            valorMinimo = min;
            valorMaximo = max;
        }

        public override float TestValue()
        {
            return _agent.VidaPorcentaje;
        }
    }

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
