

using System.Collections.Generic;

namespace BossAI
{
    /// <summary>
    /// Extiende State con soporte jerárquico.
    /// Añade referencia al estado padre y métodos para recorrer la jerarquía.
    /// La máquina jerárquica permite que transiciones definidas en estados
    /// padre sean heredadas por sus estados hijo.
    /// </summary>
    public class HierarchicalState : State
    {
       
        /// <summary>
        /// Estado padre en la jerarquía. null si es el estado raíz.
        /// </summary>
        public HierarchicalState estadoPadre;

        // -----------------------------------------------------------------

        public HierarchicalState( AIAction[] accionesEstado = null, AIAction[] accionesEntrada = null,AIAction[] accionesSalida = null,Transition[] transiciones = null, HierarchicalState estadoPadre = null)  : base(accionesEstado, accionesEntrada, accionesSalida, transiciones)
        {
            this.estadoPadre = estadoPadre;
        }

      

        /// <summary>
        /// Recorre la jerarquía de abajo a arriba y devuelve la lista de estados
        /// ordenada de raíz a hoja (de arriba a abajo).
        /// 
        /// Algoritmo:
        ///  1. Comienza desde este estado.
        ///  2. Sube por padres hasta llegar a null.
        ///  3. Invierte la lista (para tener raíz primero).
        /// </summary>
        public List<HierarchicalState> GetHierarchy()
        {
            List<HierarchicalState> lista = new List<HierarchicalState>();
            HierarchicalState estadoActual = this;

            while (estadoActual != null)
            {
                lista.Add(estadoActual);
                estadoActual = estadoActual.estadoPadre;
            }

            lista.Reverse();   // ahora va de raíz → hoja
            return lista;
        }

       

        /// <summary>
        /// Devuelve el nivel jerárquico de este estado.
        /// El estado raíz (sin padre) tiene nivel 1.
        /// Cada nivel adicional suma 1.
        /// 
        /// Se usa para que la FSM jerárquica prefiera las transiciones
        /// del estado más específico (más profundo en el árbol).
        /// </summary>
        public int GetLevel()
        {
            int contador = 0;
            HierarchicalState estadoActual = this;

            while (estadoActual != null)
            {
                contador++;
                estadoActual = estadoActual.estadoPadre;
            }

            return contador;
        }
    }
}
