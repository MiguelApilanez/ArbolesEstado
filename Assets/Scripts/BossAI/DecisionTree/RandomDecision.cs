using UnityEngine;

namespace BossAI
{
    /// <summary>
    /// Nodo decisión que elige aleatoriamente entre la rama true y false.
    /// Para evitar cambiar de decisión en cada frame, cachea el resultado
    /// durante un número de frames configurable (tiempoLimite).
    /// 
    /// Flujo:
    ///  - Si es el mismo frame que el último → reutiliza ultima_decision.
    ///  - Si ha pasado el tiempo límite     → recalcula una nueva decisión.
    ///  - En cualquier otro caso             → reutiliza ultima_decision.
    /// </summary>
    public class RandomDecision : Decision
    {
        // -----------------------------------------------------------------
        //  Estado interno (Diapositiva 26)
        // -----------------------------------------------------------------

        /// <summary>Último frame en el que se evaluó este nodo.</summary>
        private int ultimoFrame = -1;

        /// <summary>Último valor booleano aleatorio generado.</summary>
        private bool ultimaDecision = false;

        /// <summary>Número de frames que debe durar una misma decisión aleatoria.</summary>
        public int tiempoLimite = 1000;

        /// <summary>Frame en el que caduca la decisión actual y se recalcula.</summary>
        private int tiempoLimiteFrame = -1;

        // -----------------------------------------------------------------

        public RandomDecision(DecisionTreeNode nodoTrue, DecisionTreeNode nodoFalse,int tiempoLimite = 1000) : base(nodoTrue, nodoFalse)
        {
            this.tiempoLimite = tiempoLimite;
        }

        // -----------------------------------------------------------------
        //  Evaluate (Diapositiva 27)
        // -----------------------------------------------------------------

        public override DecisionTreeNode Evaluate()
        {
            int frameActual = Time.frameCount;

            // ¿Hay que recalcular? Si es un frame nuevo Y además ha caducado el tiempo
            bool esFrameNuevo = frameActual > ultimoFrame + 1;
            bool haCaducado = frameActual > tiempoLimiteFrame;

            if (esFrameNuevo || haCaducado)
            {
                ultimaDecision = (Random.value > 0.5f);
                tiempoLimiteFrame = frameActual + tiempoLimite;
            }

            ultimoFrame = frameActual;

            // Delegar en la rama correspondiente
            return ultimaDecision ? nodoTrue.Evaluate() : nodoFalse.Evaluate();
        }

        /// <summary>
        /// No se usa porque Evaluate() lo gestiona directamente.
        /// </summary>
        protected override bool TestCondition() => ultimaDecision;
    }
}
