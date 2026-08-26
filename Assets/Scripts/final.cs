using UnityEngine;

public class MostrarCreditos : MonoBehaviour
{
    [Tooltip("Arrastra aquí el panelCredits o el Canvas de tu jerarquía")]
    public GameObject panelCreditos;

    private void OnTriggerEnter(Collider other)
    {
        // Verifica si el objeto que toca es el jugador
        if (other.CompareTag("Player"))
        {
            if (panelCreditos != null)
            {
                panelCreditos.SetActive(true);
            }
        }
    }
}