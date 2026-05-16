namespace BossAI
{
    public abstract class FloatCondition : Condition
    {
        public float valorMinimo;
        public float valorMaximo;

        public abstract float TestValue();

        public override bool Test()
        {
            float valor = TestValue();
            return valorMinimo <= valor && valor <= valorMaximo;
        }
    }
}
