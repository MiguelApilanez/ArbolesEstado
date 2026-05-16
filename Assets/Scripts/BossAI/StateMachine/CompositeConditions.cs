namespace BossAI
{
    public class AndCondition : Condition
    {
        public Condition condicionA;
        public Condition condicionB;

        public AndCondition(Condition a, Condition b)
        {
            condicionA = a;
            condicionB = b;
        }

        public override bool Test()
        {
            return condicionA.Test() && condicionB.Test();
        }
    }

    public class OrCondition : Condition
    {
        public Condition condicionA;
        public Condition condicionB;

        public OrCondition(Condition a, Condition b)
        {
            condicionA = a;
            condicionB = b;
        }

        public override bool Test()
        {
            return condicionA.Test() || condicionB.Test();
        }
    }
}
