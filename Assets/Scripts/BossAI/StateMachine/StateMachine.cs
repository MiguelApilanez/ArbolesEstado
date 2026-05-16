using System.Collections.Generic;

namespace BossAI
{
    public class StateMachine
    {
        public State estadoInicial;
        protected State estadoActual;

        public virtual void Init()
        {
            estadoActual = estadoInicial;
        }

        public virtual AIAction[] Update()
        {
            Transition triggered = null;

            foreach (Transition transicion in estadoActual.transiciones)
            {
                if (transicion.IsTriggered())
                {
                    triggered = transicion;
                    break;
                }
            }

            if (triggered == null)
                return estadoActual.accionesEstado;

            State estadoFuturo = triggered.estadoObjetivo;
            List<AIAction> acciones = new List<AIAction>();

            acciones.AddRange(estadoActual.accionesSalida);
            acciones.AddRange(triggered.acciones);
            acciones.AddRange(estadoFuturo.accionesEntrada);

            estadoActual = estadoFuturo;

            return acciones.ToArray();
        }

        public State GetEstadoActual() => estadoActual;
    }
}
