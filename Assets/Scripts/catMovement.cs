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
        // Obtiene el Animator del objeto o de sus hijos
        animator = GetComponentInChildren<Animator>();

        if (puntosDeDestino.Length > 0)
        {
            IrAlSiguientePunto();
        }
    }

    void Update()
    {
        // 1. Control automático de la animación Walk / Idle basado en movimiento real
        if (animator != null && agent != null)
        {
            bool estaCaminando = agent.velocity.sqrMagnitude > 0.05f;
            animator.SetBool("isWalking", estaCaminando);
        }

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
        if (puntosDeDestino.Length == 0) return;
        agent.SetDestination(puntosDeDestino[indiceActual].position);
    }

    void AvanzarAlSiguientePunto()
    {
        indiceActual = (indiceActual + 1) % puntosDeDestino.Length;
        IrAlSiguientePunto();
    }

    // --- Métodos públicos para invocar eventos específicos ---

    public void IrACama(bool dormido)
    {
        if (animator != null)
        {
            animator.SetBool("isSleeping", dormido);
            if (dormido) agent.isStopped = true; // Detiene el movimiento si se duerme
            else agent.isStopped = false;
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