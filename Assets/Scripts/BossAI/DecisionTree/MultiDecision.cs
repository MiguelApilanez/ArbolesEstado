using System.Collections.Generic;

namespace BossAI
{
    public class MultiDecision : DecisionTreeNode
    {
        public Dictionary<string, DecisionTreeNode> nodosHijo = new Dictionary<string, DecisionTreeNode>();
        public System.Func<string> ObtenerValor;

        public override DecisionTreeNode Evaluate()
        {
            string clave = ObtenerValor?.Invoke() ?? "";

            if (nodosHijo.TryGetValue(clave, out DecisionTreeNode hijo))
                return hijo.Evaluate();

            UnityEngine.Debug.LogWarning($"[MultiDecision] Clave '{clave}' no encontrada.");
            return this;
        }
    }
}
