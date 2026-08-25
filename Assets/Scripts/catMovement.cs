using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AudioSource))]
public class CatMovement : MonoBehaviour
{
    [Header("Configuración de Ruta")]
    public Transform[] puntosDeDestino;

    [Header("Velocidades del NavMeshAgent")]
    public float walkSpeed = 2.5f;
    public float chaseSpeed = 5.0f;

    [Header("Persecución / Ataque")]
    public PlayerMovement targetPlayer;
    public bool isAttack = false;
    public bool attackLess90 = false;

    [Header("Audio Clips")]
    public AudioClip attackSound;

    private NavMeshAgent agent;
    private Animator animator;
    private AudioSource audioSource;
    private int indiceActual = 0;
    private bool haAtrapado = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();

        if (targetPlayer == null)
        {
            targetPlayer = FindFirstObjectByType<PlayerMovement>();
        }

        EvaluarEstadoGato();
    }

    void Update()
    {
        bool debePerseguir = DebePerseguirJugador();

        // 1. Control de velocidad y animaciones de movimiento
        if (agent != null)
        {
            bool estaMoviendose = agent.velocity.sqrMagnitude > 0.05f && !agent.isStopped;

            if (debePerseguir && targetPlayer != null)
            {
                // Modo Persecución (Correr)
                agent.speed = chaseSpeed;

                if (animator != null)
                {
                    animator.SetBool("isRunning", estaMoviendose);
                    animator.SetBool("isWalking", false);
                }

                IrACama(false);
                agent.isStopped = false;
                agent.SetDestination(targetPlayer.transform.position);
                return;
            }
            else
            {
                // Modo Patrullaje (Caminar)
                agent.speed = walkSpeed;

                if (animator != null)
                {
                    animator.SetBool("isWalking", estaMoviendose);
                    animator.SetBool("isRunning", false);
                }
            }
        }

        // 2. Si no hay puntos válidos y no persigue -> Dormir
        if (!TienePuntosValidos())
        {
            IrACama(true);
            return;
        }

        // 3. Patrullaje por puntos
        if (!agent.isStopped && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
            {
                AvanzarAlSiguientePunto();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            EjecutarAtaque();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            EjecutarAtaque();
        }
    }

    private void EjecutarAtaque()
    {
        if (!haAtrapado)
        {
            haAtrapado = true;
            Gritar();

            if (attackSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(attackSound);
            }
        }
    }

    private bool DebePerseguirJugador()
    {
        if (targetPlayer == null) return false;

        if (isAttack) return true;

        if (attackLess90)
        {
            float porcentajeSalud = (targetPlayer.currentHealth / targetPlayer.maxHealth) * 100f;
            if (porcentajeSalud < 90f)
            {
                return true;
            }
        }

        return false;
    }

    private void EvaluarEstadoGato()
    {
        if (DebePerseguirJugador())
        {
            agent.speed = chaseSpeed;
            IrACama(false);
        }
        else if (TienePuntosValidos())
        {
            agent.speed = walkSpeed;
            IrAlSiguientePunto();
        }
        else
        {
            IrACama(true);
        }
    }

    void IrAlSiguientePunto()
    {
        if (!TienePuntosValidos()) return;

        if (puntosDeDestino[indiceActual] == null)
        {
            AvanzarAlSiguientePunto();
            return;
        }

        IrACama(false);
        agent.isStopped = false;
        agent.SetDestination(puntosDeDestino[indiceActual].position);
    }

    void AvanzarAlSiguientePunto()
    {
        if (!TienePuntosValidos()) return;

        int intentos = 0;

        do
        {
            indiceActual = (indiceActual + 1) % puntosDeDestino.Length;
            intentos++;

            if (intentos >= puntosDeDestino.Length)
            {
                IrACama(true);
                return;
            }
        }
        while (puntosDeDestino[indiceActual] == null);

        IrAlSiguientePunto();
    }

    private bool TienePuntosValidos()
    {
        if (puntosDeDestino == null || puntosDeDestino.Length == 0) return false;

        foreach (Transform punto in puntosDeDestino)
        {
            if (punto != null) return true;
        }

        return false;
    }

    public void IrACama(bool dormido)
    {
        if (animator != null)
        {
            animator.SetBool("isSleeping", dormido);
            if (dormido)
            {
                animator.SetBool("isWalking", false);
                animator.SetBool("isRunning", false);
            }
        }

        if (agent != null)
        {
            agent.isStopped = dormido;
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