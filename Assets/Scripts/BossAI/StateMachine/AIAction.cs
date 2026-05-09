// =====================================================================
//  AIAction.cs  —  Clase base abstracta para las acciones de la FSM
//  Equivale a "Action" del pseudocódigo (renombrado para evitar
//  conflicto con System.Action de C#)
// =====================================================================

namespace BossAI
{
    /// <summary>
    /// Clase base abstracta que representa una acción que el boss puede ejecutar.
    /// Crear subclases concretas para cada comportamiento (atacar, patrullar, etc.).
    /// </summary>
    public abstract class AIAction
    {
        /// <summary>
        /// Ejecuta la acción sobre el agente (el boss).
        /// </summary>
        /// <param name="agent">Contexto del agente boss.</param>
        public abstract void Execute(BossAgent agent);
    }
}
