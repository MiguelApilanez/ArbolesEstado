using UnityEngine;

namespace BossAI
{
    public class RandomDecision : Decision
    {
        private int ultimoFrame = -1;
        private bool ultimaDecision = false;
        public int tiempoLimite = 1000;
        private int tiempoLimiteFrame = -1;

        public RandomDecision(DecisionTreeNode nodoTrue, DecisionTreeNode nodoFalse, int tiempoLimite = 1000) : base(nodoTrue, nodoFalse)
        {
            this.tiempoLimite = tiempoLimite;
        }

        public override DecisionTreeNode Evaluate()
        {
            int frameActual = Time.frameCount;

            bool esFrameNuevo = frameActual > ultimoFrame + 1;
            bool haCaducado = frameActual > tiempoLimiteFrame;

            if (esFrameNuevo || haCaducado)
            {
                ultimaDecision = (Random.value > 0.5f);
                tiempoLimiteFrame = frameActual + tiempoLimite;
            }

            ultimoFrame = frameActual;

            return ultimaDecision ? nodoTrue.Evaluate() : nodoFalse.Evaluate();
        }

        protected override bool TestCondition() => ultimaDecision;
    }
}
