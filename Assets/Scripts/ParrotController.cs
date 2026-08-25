using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AudioSource))]
public class ParrotController : MonoBehaviour
{
    [Header("UI")]
    public Button feedButton;

    [Header("Configuración de Capa / Raycast")]
    public LayerMask groundLayer;

    [Header("Audio Clips")]
    public AudioClip moveSound;
    public AudioClip triggerSound;
    public AudioClip feedSound;      // Clip al alimentar con éxito
    public AudioClip noCookiesSound; // Clip cuando no hay galletas

    private NavMeshAgent agent;
    private AudioSource audioSource;
    private PlayerMovement playerInRange;
    private bool waitingForTargetClick = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();

        if (feedButton != null)
        {
            feedButton.onClick.AddListener(OnFeedButtonClicked);
            feedButton.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // Controla la reproducción del audio de desplazamiento
        UpdateMovementAudio();

        var mouse = Mouse.current;
        if (mouse == null) return;

        // Detectar clic izquierdo con el Input System
        if (waitingForTargetClick && mouse.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePosition = mouse.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundLayer))
            {
                agent.SetDestination(hit.point);
                waitingForTargetClick = false;
                Debug.Log($"Loro desplazándose a: {hit.point}");
            }
        }
    }

    private void UpdateMovementAudio()
    {
        // Si se está reproduciendo un sonido OneShot, se espera a que termine
        if (audioSource.isPlaying && !audioSource.loop) return;

        // Comprueba si el NavMeshAgent está caminando activamente
        bool isMoving = agent.hasPath && agent.velocity.sqrMagnitude > 0.1f && agent.remainingDistance > agent.stoppingDistance;

        if (isMoving)
        {
            if (moveSound != null && (audioSource.clip != moveSound || !audioSource.isPlaying))
            {
                audioSource.clip = moveSound;
                audioSource.loop = true;
                audioSource.Play();
            }
        }
        else
        {
            // Detiene el audio en bucle cuando se queda quieto
            if (audioSource.isPlaying && audioSource.loop)
            {
                audioSource.Stop();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = other.GetComponent<PlayerMovement>();

            // Reproducir sonido al entrar al rango del jugador
            if (triggerSound != null)
            {
                audioSource.PlayOneShot(triggerSound);
            }

            if (feedButton != null)
            {
                feedButton.gameObject.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = null;
            if (feedButton != null)
            {
                feedButton.gameObject.SetActive(false);
            }
        }
    }

    private void OnFeedButtonClicked()
    {
        if (playerInRange != null)
        {
            if (playerInRange.UseCookie())
            {
                Debug.Log("¡Loro alimentado! Haz clic en el suelo para indicarle a dónde ir.");

                // Reproduce el sonido de éxito al comer la galleta
                if (feedSound != null)
                {
                    audioSource.PlayOneShot(feedSound);
                }

                waitingForTargetClick = true;
                if (feedButton != null)
                {
                    feedButton.gameObject.SetActive(false);
                }
            }
            else
            {
                Debug.Log("No tienes galletas suficientes.");

                // Reproduce el sonido de advertencia cuando no hay galletas
                if (noCookiesSound != null)
                {
                    audioSource.PlayOneShot(noCookiesSound);
                }
            }
        }
    }
}