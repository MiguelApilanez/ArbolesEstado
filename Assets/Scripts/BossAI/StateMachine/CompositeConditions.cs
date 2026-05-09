// =====================================================================
//  CompositeConditions.cs  —  Condiciones compuestas (Diapositivas 23 y 24)
// =====================================================================

namespace BossAI
{
    /// <summary>
    /// Condición compuesta AND: se cumple si AMBAS condiciones se cumplen.
    /// Permite encadenar condiciones (ej: "boss tiene poca vida Y jugador está cerca").
    /// </summary>
    public class AndCondition : Condition
    {
        /// <summary>Primera condición a evaluar.</summary>
        public Condition condicionA;

        /// <summary>Segunda condición a evaluar.</summary>
        public Condition condicionB;

        public AndCondition(Condition a, Condition b)
        {
            condicionA = a;
            condicionB = b;
        }

        /// <summary>
        /// Devuelve true solo si tanto condicionA como condicionB son verdaderas.
        /// </summary>
        public override bool Test()
        {
            return condicionA.Test() && condicionB.Test();
        }
    }

    // -----------------------------------------------------------------

    /// <summary>
    /// Condición compuesta OR: se cumple si AL MENOS UNA condición se cumple.
    /// Permite encadenar condiciones (ej: "boss en fase 2 O jugador ha atacado").
    /// </summary>
    public class OrCondition : Condition
    {
        /// <summary>Primera condición a evaluar.</summary>
        public Condition condicionA;

        /// <summary>Segunda condición a evaluar.</summary>
        public Condition condicionB;

        public OrCondition(Condition a, Condition b)
        {
            condicionA = a;
            condicionB = b;
        }

        /// <summary>
        /// Devuelve true si condicionA o condicionB (o ambas) son verdaderas.
        /// </summary>
        public override bool Test()
        {
            return condicionA.Test() || condicionB.Test();
        }
    }
}