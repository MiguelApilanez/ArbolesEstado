using UnityEngine;

namespace BossAI
{
    public enum BTStatus { Success, Failure, Running }

    public abstract class BTNode
    {
        public abstract BTStatus Execute(BossAgent agent);
    }

    public class BTSelector : BTNode
    {
        private readonly BTNode[] _hijos;

        public BTSelector(params BTNode[] hijos) { _hijos = hijos; }

        public override BTStatus Execute(BossAgent agent)
        {
            foreach (BTNode hijo in _hijos)
            {
                BTStatus resultado = hijo.Execute(agent);
                if (resultado != BTStatus.Failure) return resultado;
            }
            return BTStatus.Failure;
        }
    }

    public class BTSequence : BTNode
    {
        private readonly BTNode[] _hijos;

        public BTSequence(params BTNode[] hijos) { _hijos = hijos; }

        public override BTStatus Execute(BossAgent agent)
        {
            foreach (BTNode hijo in _hijos)
            {
                BTStatus resultado = hijo.Execute(agent);
                if (resultado != BTStatus.Success) return resultado;
            }
            return BTStatus.Success;
        }
    }

    public class BTCondition : BTNode
    {
        private readonly System.Func<BossAgent, bool> _condicion;

        public BTCondition(System.Func<BossAgent, bool> condicion) { _condicion = condicion; }

        public override BTStatus Execute(BossAgent agent)
            => _condicion(agent) ? BTStatus.Success : BTStatus.Failure;
    }

    public class BTCooldownDecorator : BTNode
    {
        private readonly BTNode _hijo;
        private readonly float _cooldown;
        private float _ultimaEjecucion = -999f;

        public BTCooldownDecorator(BTNode hijo, float cooldown)
        {
            _hijo = hijo;
            _cooldown = cooldown;
        }

        public override BTStatus Execute(BossAgent agent)
        {
            if (Time.time < _ultimaEjecucion + _cooldown)
                return BTStatus.Running;

            BTStatus resultado = _hijo.Execute(agent);

            if (resultado == BTStatus.Success)
                _ultimaEjecucion = Time.time;

            return resultado;
        }
    }

    public class BTInverter : BTNode
    {
        private readonly BTNode _hijo;

        public BTInverter(BTNode hijo) { _hijo = hijo; }

        public override BTStatus Execute(BossAgent agent)
        {
            BTStatus resultado = _hijo.Execute(agent);
            return resultado switch
            {
                BTStatus.Success => BTStatus.Failure,
                BTStatus.Failure => BTStatus.Success,
                _ => BTStatus.Running
            };
        }
    }
}
