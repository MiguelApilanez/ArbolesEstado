using System.Collections.Generic;

namespace BossAI
{
    /// <summary>
    /// Máquina de estados finita generalista.
    /// Gestiona el estado actual del boss, evalúa transiciones cada frame
    /// y devuelve la lista de acciones a ejecutar.
    /// </summary>
    public class StateMachine
    {
        /// <summary>Estado inicial al que se vuelve al reiniciar la FSM.</summary>
        public State estadoInicial;

        /// <summary>Estado en el que se encuentra el boss actualmente.</summary>
        protected State estadoActual;

        // -----------------------------------------------------------------
        //  Inicialización
        // -----------------------------------------------------------------

        /// <summary>
        /// Inicializa la máquina colocando el estado actual en el inicial.
        /// Llamar una vez al arrancar el boss.
        /// </summary>
        public virtual void Init()
        {
            estadoActual = estadoInicial;
        }


        /// <summary>
        /// Actualiza la máquina de estados.
        /// Debe llamarse cada frame desde el BossController.
        /// 
        /// Lógica:
        ///  1. Busca la primera transición disparada del estado actual.
        ///  2. Si no hay transición → devuelve las acciones de estado (tick normal).
        ///  3. Si hay transición   → ejecuta salida + transición + entrada y cambia de estado.
        /// </summary>
        /// <returns>Array de AIAction a ejecutar este frame.</returns>
        public virtual AIAction[] Update()
        {
            Transition triggered = null;

            foreach (Transition transicion in estadoActual.transiciones)
            {
                if (transicion.IsTriggered())
                {
                    triggered = transicion;
                    break;   // primera transición válida gana
                }
            }

            if (triggered == null)
            {
                // Sin transición → devolver acciones de estado (tick normal)
                return estadoActual.accionesEstado;
            }

            
            State estadoFuturo = triggered.estadoObjetivo; // AQUI NO SE MUY BIEN PQ claro si el triggered no es nulo el estado futuro es el objetivo de la transcion. Pero no lo esoty difiniedo con el else
            List<AIAction> acciones = new List<AIAction>();

            // 1. Acciones de salida del estado actual
            acciones.AddRange(estadoActual.accionesSalida);

            // 2. Acciones propias de la transición
            acciones.AddRange(triggered.acciones);

            // 3. Acciones de entrada del nuevo estado
            acciones.AddRange(estadoFuturo.accionesEntrada);

            // Cambiar de estado
            estadoActual = estadoFuturo;

            return acciones.ToArray();
        }



     // Devuelve el estado en el que se encuentra actualmente el boss
        public State GetEstadoActual() => estadoActual;
    }
}