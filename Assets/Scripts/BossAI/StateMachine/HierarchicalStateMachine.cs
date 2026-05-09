using System.Collections.Generic;

namespace BossAI
{
    /// <summary>
    /// Extiende StateMachine con soporte jerárquico (HSM).
    /// 
    /// Diferencias clave respecto a la generalista:
    ///  - Recorre TODA la jerarquía del estado actual buscando transiciones.
    ///  - Prioriza las transiciones del estado MÁS ESPECÍFICO (más profundo).
    ///  - Al cambiar de estado, ejecuta salidas/entradas de todos los estados
    ///    padre implicados hasta el ancestro común.
    /// </summary>
    public class HierarchicalStateMachine : StateMachine
    {
       

        /// <summary>
        /// Actualiza la HSM.
        /// 
        /// Algoritmo:
        ///  1. Obtiene la jerarquía completa del estado actual (raíz → hoja).
        ///  2. Itera sobre todos los estados buscando la transición disparada
        ///     del estado MÁS PROFUNDO (mayor nivel jerárquico = más específico).
        ///  3. Si hay transición → ApplyTransition.
        ///  4. Si no             → devuelve acciones de estado del estado actual.
        /// </summary>
        public override AIAction[] Update()
        {
            HierarchicalState estadoActualH = estadoActual as HierarchicalState;

            // Si no hay estado actual, no hacer nada
            if (estadoActualH == null)
            {
                return new AIAction[0];
            }

            // La jerarquía va de raíz (índice 0) a hoja (último índice)
            List<HierarchicalState> jerarquia = estadoActualH.GetHierarchy();

            Transition mejorTransicion = null;
            int mejorNivel = int.MaxValue;   // "infinito" inicial

            
            foreach (HierarchicalState estado in jerarquia)
            {
                foreach (Transition transicion in estado.transiciones)
                {
                    if (transicion.IsTriggered())
                    {
                        int nivelEstado = estado.GetLevel();

                        // Preferimos el estado con el NIVEL MÁS ALTO (más específico).
                        // Como nivel más alto = número más grande, buscamos el máximo,
                        // pero para mantener coherencia con el pseudocódigo del profesor
                        // ("si el nivel es menor que el mejor_nivel") usamos el nivel
                        // INVERTIDO: el estado hoja tiene el nivel más alto, y
                        // "menor_nivel" almacena el nivel del estado padre menos
                        // específico. Así que invertimos la comparación:
                        if (nivelEstado > mejorNivel || mejorTransicion == null)
                        {
                            mejorNivel = nivelEstado;
                            mejorTransicion = transicion;
                        }
                        break;   // una transición por estado es suficiente
                    }
                }
            }

          
            if (mejorTransicion == null)
            {
                // Sin transición → acciones de estado del estado hoja (actual)
                return estadoActualH.accionesEstado;
            }

            return ApplyTransition(mejorTransicion);
        }

       
        /// <summary>
        /// Aplica la transición ganadora teniendo en cuenta la jerarquía completa.
        /// Ejecuta salidas desde el estado actual hasta el ancestro común,
        /// luego las acciones de la transición,
        /// y finalmente las entradas desde el ancestro común hasta el estado destino.
        /// </summary>
        protected AIAction[] ApplyTransition(Transition mejorTransicion)
        {
            HierarchicalState estadoActualH = estadoActual as HierarchicalState;
            HierarchicalState estadoDestino = mejorTransicion.estadoObjetivo as HierarchicalState;

            // obtener jerarquías
            List<HierarchicalState> jerarquiaOrigen = estadoActualH?.GetHierarchy() ?? new List<HierarchicalState>();
            List<HierarchicalState> jerarquiaDestino = estadoDestino?.GetHierarchy() ?? new List<HierarchicalState>();

            // Encontrar el ancestro común recorriendo ambas listas
            HierarchicalState ancestroComun = EncontrarAncestroComun(jerarquiaOrigen, jerarquiaDestino);

            List<AIAction> acciones = new List<AIAction>();

            // acciones de SALIDA (origen → ancestro) ─
            // Recorremos desde el final (hoja) hacia el ancestro común
            for (int i = jerarquiaOrigen.Count - 1; i >= 0; i--)
            {
                HierarchicalState estado = jerarquiaOrigen[i];
                if (estado == ancestroComun) 
                {
                    break;
                }
                acciones.AddRange(estado.accionesSalida);
            }

            // acciones de la transición 
            acciones.AddRange(mejorTransicion.acciones);

            // acciones de ENTRADA (ancestro → destino)
            bool dentroRango = false;
            foreach (HierarchicalState estado in jerarquiaDestino)
            {
                if (estado == ancestroComun) 
                { 
                    dentroRango = true; continue;
                }
                if (dentroRango)
                {
                    acciones.AddRange(estado.accionesEntrada);

                }
            }

            // actualizar estado actual
            estadoActual = mejorTransicion.estadoObjetivo;

            return acciones.ToArray();
        }

        

        private HierarchicalState EncontrarAncestroComun(List<HierarchicalState> origen,List<HierarchicalState> destino)
        {
            // Recorrer ambas listas simultáneamente hasta que difieran
            HierarchicalState comun = null;
            int limite = System.Math.Min(origen.Count, destino.Count);

            for (int i = 0; i < limite; i++)
            {
                if (origen[i] == destino[i])
                    comun = origen[i];
                else
                {
                    break;
                }
                    
            }

            return comun;
        }
    }
}