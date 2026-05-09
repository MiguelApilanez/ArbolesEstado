namespace BossAI
{
    /// <summary>
    /// Nodo hoja del árbol de decisión.
    /// Cuando la evaluación recursiva llega aquí, devuelve 'this',
    /// indicando que esta es la acción que debe ejecutarse.
    /// 
    /// El árbol NO ejecuta acciones, solo elige cuál ejecutar.
    /// La ejecución la hace el sistema externo (BossController).
    /// </summary>
    public abstract class DTActionNode : DecisionTreeNode
    {
        /// <summary>
        /// Un nodo acción siempre se devuelve a sí mismo.
        /// Es el punto de llegada de la evaluación recursiva.
        /// </summary>
        public override DecisionTreeNode Evaluate()
        {
            return this;
        }

        /// <summary>
        /// Ejecuta la acción concreta sobre el agente boss.
        /// Implementar en subclases con el comportamiento deseado.
        /// </summary>
        public abstract void Execute(BossAgent agent);
    }
}