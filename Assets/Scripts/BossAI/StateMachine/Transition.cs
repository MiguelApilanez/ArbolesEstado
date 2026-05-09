// =====================================================================
//  Transition.cs  —  Clase de transición entre estados (Diapositiva 20)
// =====================================================================

namespace BossAI
{
    /// <summary>
    /// Representa una transición entre dos estados de la FSM.
    /// Contiene la condición de disparo, el estado destino y las acciones
    /// que se ejecutan durante la transición.
    /// </summary>
    public class Transition
    {
        /// <summary>
        /// Acciones que se ejecutan en el momento exacto de la transición
        /// (después de salir del estado actual y antes de entrar al nuevo).
        /// </summary>
        public AIAction[] acciones;

        /// <summary>Estado al que se transiciona si la condición se cumple.</summary>
        public State estadoObjetivo;

        /// <summary>Condición que debe cumplirse para que la transición se active.</summary>
        public Condition condicion;

        public Transition(State estadoObjetivo, Condition condicion, AIAction[] acciones = null)
        {
            this.estadoObjetivo = estadoObjetivo;
            this.condicion = condicion;
            this.acciones = acciones ?? new AIAction[0];
        }

        /// <summary>
        /// Comprueba si la transición ha sido disparada evaluando su condición.
        /// </summary>
        /// <returns>True si la condición se cumple y la transición debe ejecutarse.</returns>
        public bool IsTriggered()
        {
            return condicion.Test();
        }
    }
}