namespace BossAI
{
    /// <summary>
    /// Nodo decisión abstracto con dos ramas: verdadero y falso.
    /// Evalúa una condición sobre 'valor' y delega en el hijo
    /// correspondiente. Las subclases concretas implementan
    /// la lógica de evaluación de la condición.
    /// </summary>
    public abstract class Decision : DecisionTreeNode
    {
        /// <summary>Nodo hijo que se evalúa si la condición es verdadera.</summary>
        public DecisionTreeNode nodoTrue;

        /// <summary>Nodo hijo que se evalúa si la condición es falsa.</summary>
        public DecisionTreeNode nodoFalse;

        /// <summary>
        /// Valor sobre el que se evalúa la condición.
        /// Su tipo concreto lo determina cada subclase.
        /// </summary>
        public object valor;

        protected Decision(DecisionTreeNode nodoTrue, DecisionTreeNode nodoFalse)
        {
            this.nodoTrue = nodoTrue;
            this.nodoFalse = nodoFalse;
        }

        // -----------------------------------------------------------------

        /// <summary>
        /// Evalúa la condición y delega recursivamente en el hijo correcto.
        /// </summary>
        public override DecisionTreeNode Evaluate()
        {
            if (TestCondition())
                return nodoTrue.Evaluate();
            else
                return nodoFalse.Evaluate();
        }

        /// <summary>
        /// Evalúa si la condición del nodo se cumple para el valor actual.
        /// Implementar en cada subclase con la lógica concreta.
        /// </summary>
        protected abstract bool TestCondition();
    }
}


