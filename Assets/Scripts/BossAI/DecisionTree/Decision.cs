namespace BossAI
{
    public abstract class Decision : DecisionTreeNode
    {
        public DecisionTreeNode nodoTrue;
        public DecisionTreeNode nodoFalse;
        public object valor;

        protected Decision(DecisionTreeNode nodoTrue, DecisionTreeNode nodoFalse)
        {
            this.nodoTrue = nodoTrue;
            this.nodoFalse = nodoFalse;
        }

        public override DecisionTreeNode Evaluate()
        {
            if (TestCondition())
                return nodoTrue.Evaluate();
            else
                return nodoFalse.Evaluate();
        }

        protected abstract bool TestCondition();
    }
}
