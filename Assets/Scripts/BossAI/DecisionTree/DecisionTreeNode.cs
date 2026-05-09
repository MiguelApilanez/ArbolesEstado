namespace BossAI
{
    /// <summary>
    /// Clase base abstracta para todos los nodos del árbol de decisión.
    /// Cada nodo puede ser una decisión (nodo interno) o una acción (hoja).
    /// La evaluación es recursiva: cada nodo evalúa a sí mismo y delega
    /// en sus hijos hasta llegar a una hoja (acción).
    /// </summary>
    public abstract class DecisionTreeNode
    {
        /// <summary>
        /// Evalúa este nodo del árbol recursivamente.
        /// Si es un nodo decisión, evalúa una condición y delega en un hijo.
        /// Si es un nodo acción (hoja), devuelve 'this'.
        /// </summary>
        /// <returns>El nodo acción (hoja) que debe ejecutarse.</returns>
        public abstract DecisionTreeNode Evaluate();
    }
}