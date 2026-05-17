using System.Collections.Generic;

namespace BossAI
{
    public class HierarchicalState : State
    {
        public HierarchicalState estadoPadre;

        public HierarchicalState(AIAction[] accionesEstado = null, AIAction[] accionesEntrada = null, AIAction[] accionesSalida = null, Transition[] transiciones = null, HierarchicalState estadoPadre = null) : base(accionesEstado, accionesEntrada, accionesSalida, transiciones)
        {
            this.estadoPadre = estadoPadre;
        }

        public List<HierarchicalState> GetHierarchy()
        {
            List<HierarchicalState> lista = new List<HierarchicalState>();
            HierarchicalState estadoActual = this;

            while (estadoActual != null)
            {
                lista.Add(estadoActual);
                estadoActual = estadoActual.estadoPadre;
            }

            lista.Reverse();
            return lista;
        }

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
