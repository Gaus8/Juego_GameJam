using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.InputSystem; // Importante para el nuevo Input System

[RequireComponent(typeof(NavMeshAgent))]
public class ParrotController : MonoBehaviour
{
    [Header("UI")]
    public Button feedButton;

    [Header("Configuración de Capa / Raycast")]
    public LayerMask groundLayer;

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
        var mouse = Mouse.current;
        if (mouse == null) return;

        // Detectar clic izquierdo con el nuevo Input System
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