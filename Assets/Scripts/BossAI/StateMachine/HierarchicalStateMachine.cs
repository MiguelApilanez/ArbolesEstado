using System.Collections.Generic;

namespace BossAI
{
    public class HierarchicalStateMachine : StateMachine
    {
        public override AIAction[] Update()
        {
            HierarchicalState estadoActualH = estadoActual as HierarchicalState;

            if (estadoActualH == null)
                return new AIAction[0];

            List<HierarchicalState> jerarquia = estadoActualH.GetHierarchy();

            Transition mejorTransicion = null;
            int mejorNivel = int.MaxValue;

            foreach (HierarchicalState estado in jerarquia)
            {
                foreach (Transition transicion in estado.transiciones)
                {
                    if (transicion.IsTriggered())
                    {
                        int nivelEstado = estado.GetLevel();

                        if (nivelEstado > mejorNivel || mejorTransicion == null)
                        {
                            mejorNivel = nivelEstado;
                            mejorTransicion = transicion;
                        }
                        break;
                    }
                }
            }

            if (mejorTransicion == null)
                return estadoActualH.accionesEstado;

            return ApplyTransition(mejorTransicion);
        }

        protected AIAction[] ApplyTransition(Transition mejorTransicion)
        {
            HierarchicalState estadoActualH = estadoActual as HierarchicalState;
            HierarchicalState estadoDestino = mejorTransicion.estadoObjetivo as HierarchicalState;

            List<HierarchicalState> jerarquiaOrigen = estadoActualH?.GetHierarchy() ?? new List<HierarchicalState>();
            List<HierarchicalState> jerarquiaDestino = estadoDestino?.GetHierarchy() ?? new List<HierarchicalState>();

            HierarchicalState ancestroComun = EncontrarAncestroComun(jerarquiaOrigen, jerarquiaDestino);

            List<AIAction> acciones = new List<AIAction>();

            for (int i = jerarquiaOrigen.Count - 1; i >= 0; i--)
            {
                HierarchicalState estado = jerarquiaOrigen[i];
                if (estado == ancestroComun) break;
                acciones.AddRange(estado.accionesSalida);
            }

            acciones.AddRange(mejorTransicion.acciones);

            bool dentroRango = false;
            foreach (HierarchicalState estado in jerarquiaDestino)
            {
                if (estado == ancestroComun) { dentroRango = true; continue; }
                if (dentroRango) acciones.AddRange(estado.accionesEntrada);
            }

            estadoActual = mejorTransicion.estadoObjetivo;

            return acciones.ToArray();
        }

        private HierarchicalState EncontrarAncestroComun(List<HierarchicalState> origen, List<HierarchicalState> destino)
        {
            HierarchicalState comun = null;
            int limite = System.Math.Min(origen.Count, destino.Count);

            for (int i = 0; i < limite; i++)
            {
                if (origen[i] == destino[i])
                    comun = origen[i];
                else
                    break;
            }

            return comun;
        }
    }
}
