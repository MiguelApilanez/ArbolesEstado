namespace BossAI
{
    public class Transition
    {
        public AIAction[] acciones;
        public State estadoObjetivo;
        public Condition condicion;

        public Transition(State estadoObjetivo, Condition condicion, AIAction[] acciones = null)
        {
            this.estadoObjetivo = estadoObjetivo;
            this.condicion = condicion;
            this.acciones = acciones ?? new AIAction[0];
        }

        public bool IsTriggered()
        {
            return condicion.Test();
        }
    }
}
