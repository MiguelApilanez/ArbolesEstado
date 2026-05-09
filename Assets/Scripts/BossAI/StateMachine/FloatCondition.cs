namespace BossAI
{
    /// <summary>
    /// Condición abstracta para valores numéricos float.
    /// Devuelve true si el valor evaluado está dentro del rango [valorMinimo, valorMaximo].
    /// Hereda de esta clase para leer cualquier valor del mundo de juego (vida, distancia, etc.).
    /// </summary>
    public abstract class FloatCondition : Condition
    {
        /// <summary>Valor mínimo del rango aceptado (inclusive).</summary>
        public float valorMinimo;

        /// <summary>Valor máximo del rango aceptado (inclusive).</summary>
        public float valorMaximo;

        /// <summary>
        /// Obtiene el valor actual que se desea evaluar.
        /// Implementar en subclases para leer el dato de juego correspondiente.
        /// </summary>
        public abstract float TestValue();

        /// <summary>
        /// Comprueba si el valor actual está dentro del rango [valorMinimo, valorMaximo].
        /// </summary>
        public override bool Test()
        {
            float valor = TestValue();
            return valorMinimo <= valor && valor <= valorMaximo;
        }
    }
}
