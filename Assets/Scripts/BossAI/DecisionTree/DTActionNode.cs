namespace BossAI
{
    public abstract class DTActionNode : DecisionTreeNode
    {
        public override DecisionTreeNode Evaluate()
        {
            return this;
        }

        public abstract void Execute(BossAgent agent);
    }
}
