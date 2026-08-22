using UnityEngine;
using UnityEngine.AI;

public class CatMovement : MonoBehaviour
{
    [Header("Configuración de Ruta")]
    public Transform[] puntosDeDestino; // Arreglo de puntos a recorrer

    private NavMeshAgent agent;
    private int indiceActual = 0;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Inicia el movimiento hacia el primer punto si la lista no está vacía
        if (puntosDeDestino.Length > 0)
        {
            IrAlSiguientePunto();
        }
    }

    void Update()
    {
        // Verifica si el agente llegó a su destino actual
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
            {
                AvanzarAlSiguientePunto();
            }
        }
    }

    void IrAlSiguientePunto()
    {
        if (puntosDeDestino.Length == 0) return;

        // Asigna la posición del punto actual
        agent.SetDestination(puntosDeDestino[indiceActual].position);
    }

    void AvanzarAlSiguientePunto()
    {
        // Pasa al siguiente punto de la lista (ciclo continuo)
        indiceActual = (indiceActual + 1) % puntosDeDestino.Length;
        IrAlSiguientePunto();
    }
}