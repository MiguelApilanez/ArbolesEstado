

using System.Collections.Generic;

namespace BossAI
{
    /// <summary>
    /// Nodo decisión que soporta múltiples ramas mediante un diccionario.
    /// Útil para decisiones con más de dos salidas (ej: fase 1/2/3 del boss,
    /// tipo de ataque elegido, dirección de movimiento...).
    /// 
    /// Ejemplo de uso:
    ///   var multi = new MultiDecision();
    ///   multi.nodosHijo["fase1"] = nodoPatrulla;
    ///   multi.nodosHijo["fase2"] = nodoEnrage;
    ///   multi.nodosHijo["fase3"] = nodoFinal;
    ///   multi.ObtenerValor = () => agente.FaseActual;
    /// </summary>
    public class MultiDecision : DecisionTreeNode
    {
        /// <summary>
        /// Diccionario de nodos hijo indexados por clave.
        /// La clave puede ser string, int, enum convertido a string, etc.
        /// </summary>
        public Dictionary<string, DecisionTreeNode> nodosHijo
            = new Dictionary<string, DecisionTreeNode>();

        /// <summary>
        /// Función que devuelve la clave del nodo hijo a evaluar.
        /// Asignar desde el exterior con una lambda:
        ///   multi.ObtenerValor = () => boss.FaseActual.ToString();
        /// </summary>
        public System.Func<string> ObtenerValor;

        // -----------------------------------------------------------------

        public override DecisionTreeNode Evaluate()
        {
            string clave = ObtenerValor?.Invoke() ?? "";

            if (nodosHijo.TryGetValue(clave, out DecisionTreeNode hijo))
                return hijo.Evaluate();

            // Si la clave no existe, no hacer nada (devolver null-safe)
            UnityEngine.Debug.LogWarning($"[MultiDecision] Clave '{clave}' no encontrada.");
            return this;
        }
    }
}