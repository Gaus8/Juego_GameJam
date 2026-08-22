using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[RequireComponent(typeof(NavMeshAgent))]
public class ParrotController : MonoBehaviour
{
    [Header("UI")]
    public Button feedButton; // Asigna el botón UI de "Dar Galleta"

    [Header("Configuración de Capa / Raycast")]
    public LayerMask groundLayer; // Asigna la capa del suelo (p. ej. Default o Ground)

    private NavMeshAgent agent;
    private PlayerMovement playerInRange;
    private bool waitingForTargetClick = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (feedButton != null)
        {
            feedButton.onClick.AddListener(OnFeedButtonClicked);
            feedButton.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // Si ya se consumió la galleta y estamos esperando el clic en el suelo
        if (waitingForTargetClick && Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundLayer))
            {
                agent.SetDestination(hit.point);
                waitingForTargetClick = false;
                Debug.Log($"Loro desplazándose a: {hit.point}");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = other.GetComponent<PlayerMovement>();
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
                waitingForTargetClick = true;
                if (feedButton != null)
                {
                    feedButton.gameObject.SetActive(false);
                }
            }
            else
            {
                Debug.Log("No tienes galletas suficientes.");
            }
        }
    }
}