using UnityEngine;
using UnityEngine.AI;

public class CatMovement : MonoBehaviour
{
    [Header("Configuración de Ruta")]
    public Transform[] puntosDeDestino;

    private NavMeshAgent agent;
    private Animator animator;
    private int indiceActual = 0;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        // Si no hay puntos válidos asignados, se activa el estado de dormir por defecto
        if (!TienePuntosValidos())
        {
            IrACama(true);
            return;
        }

        IrAlSiguientePunto();
    }

    void Update()
    {
        // 1. Control automático de la animación Walk basado en el movimiento del NavMeshAgent
        if (animator != null && agent != null)
        {
            bool estaCaminando = agent.velocity.sqrMagnitude > 0.05f && !agent.isStopped;
            animator.SetBool("isWalking", estaCaminando);
        }

        // Si no hay puntos válidos o el agente está detenido, no procesa patrullaje
        if (!TienePuntosValidos() || agent.isStopped) return;

        // 2. Lógica de patrullaje
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
        if (!TienePuntosValidos()) return;

        // Si el punto actual en el arreglo es null, busca el siguiente que sea válido
        if (puntosDeDestino[indiceActual] == null)
        {
            AvanzarAlSiguientePunto();
            return;
        }

        agent.isStopped = false;
        agent.SetDestination(puntosDeDestino[indiceActual].position);
    }

    void AvanzarAlSiguientePunto()
    {
        if (!TienePuntosValidos()) return;

        int intentos = 0;

        // Ciclo para ignorar elementos nulos dentro del array
        do
        {
            indiceActual = (indiceActual + 1) % puntosDeDestino.Length;
            intentos++;

            // Evita un bucle infinito si todos los elementos del array son null
            if (intentos >= puntosDeDestino.Length)
            {
                IrACama(true);
                return;
            }
        }
        while (puntosDeDestino[indiceActual] == null);

        IrAlSiguientePunto();
    }

    // Comprueba si el array existe y si al menos contiene un elemento que no sea null
    private bool TienePuntosValidos()
    {
        if (puntosDeDestino == null || puntosDeDestino.Length == 0) return false;

        foreach (Transform punto in puntosDeDestino)
        {
            if (punto != null) return true;
        }

        return false;
    }

    // --- Métodos públicos para invocar eventos específicos ---

    public void IrACama(bool dormido)
    {
        if (animator != null)
        {
            animator.SetBool("isSleeping", dormido);
        }

        if (agent != null)
        {
            agent.isStopped = dormido; // Detiene el NavMeshAgent si se duerme
            if (dormido)
            {
                agent.ResetPath();
            }
        }
    }

    public void Gritar()
    {
        if (animator != null)
        {
            animator.SetTrigger("scream");
        }
    }
}