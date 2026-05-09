namespace BossAI
{
    /// <summary>
    /// Clase abstracta que representa una condición que puede ser evaluada.
    /// Implementa subclases concretas para cada tipo de chequeo de juego.
    /// </summary>
    public abstract class Condition
    {
        /// <summary>
        /// Evalúa si la condición se cumple.
        /// </summary>
        /// <returns>True si la condición se cumple, false en caso contrario.</returns>
        public abstract bool Test();
    }
}