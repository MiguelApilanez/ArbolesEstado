

namespace BossAI
{
    /// <summary>
    /// Representa un estado de la máquina de estados finita.
    /// Contiene tres colecciones de acciones (estado, entrada, salida)
    /// y la lista de transiciones posibles desde este estado.
    /// </summary>
    public class State
    {
        /// <summary>
        /// Acciones que se ejecutan CADA FRAME mientras el boss permanece en este estado.
        /// Ej: seguir moviéndose al patrullar, mantener una animación de idle, etc.
        /// </summary>
        public AIAction[] accionesEstado;

        /// <summary>
        /// Acciones que se ejecutan UNA VEZ al ENTRAR en este estado.
        /// Ej: activar partículas de enrage, reproducir sonido de aparición, etc.
        /// </summary>
        public AIAction[] accionesEntrada;

        /// <summary>
        /// Acciones que se ejecutan UNA VEZ al SALIR de este estado.
        /// Ej: detener una animación, desactivar un escudo, etc.
        /// </summary>
        public AIAction[] accionesSalida;

        /// <summary>
        /// Lista de transiciones posibles desde este estado.
        /// Se evalúan en orden; la primera que se dispare gana.
        /// </summary>
        public Transition[] transiciones;

        public State(AIAction[] accionesEstado = null, AIAction[] accionesEntrada = null,AIAction[] accionesSalida = null,Transition[] transiciones = null)
        {
            this.accionesEstado = accionesEstado ?? new AIAction[0];
            this.accionesEntrada = accionesEntrada ?? new AIAction[0];
            this.accionesSalida = accionesSalida ?? new AIAction[0];
            this.transiciones = transiciones ?? new Transition[0];
        }
    }
}