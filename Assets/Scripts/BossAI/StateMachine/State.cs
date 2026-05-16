namespace BossAI
{
    public class State
    {
        public AIAction[] accionesEstado;
        public AIAction[] accionesEntrada;
        public AIAction[] accionesSalida;
        public Transition[] transiciones;

        public State(AIAction[] accionesEstado = null, AIAction[] accionesEntrada = null, AIAction[] accionesSalida = null, Transition[] transiciones = null)
        {
            this.accionesEstado = accionesEstado ?? new AIAction[0];
            this.accionesEntrada = accionesEntrada ?? new AIAction[0];
            this.accionesSalida = accionesSalida ?? new AIAction[0];
            this.transiciones = transiciones ?? new Transition[0];
        }
    }
}
