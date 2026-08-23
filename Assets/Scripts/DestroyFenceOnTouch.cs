using UnityEngine;

public class DestroyFenceOnTouch : MonoBehaviour
{
    [Header("Configuración de Destrucción")]
    [Tooltip("Tag del objeto que debe ser destruido al tocarlo.")]
    public string targetTag = "valla";

    [Header("Efectos Opcionales")]
    [Tooltip("Sonido que se reproduce cuando la valla se destruye.")]
    public AudioClip destroySound;

    [Range(0f, 1f)]
    public float soundVolume = 1f;

    private void OnTriggerEnter(Collider other)
    {
        // Comprueba si el objeto con el que chocó tiene el tag "valla"
        if (other.CompareTag(targetTag))
        {
            // Reproduce el sonido en la posición del choque si hay uno asignado
            if (destroySound != null)
            {
                AudioSource.PlayClipAtPoint(destroySound, other.transform.position, soundVolume);
            }

            // Destruye el objeto de la valla
            Destroy(other.gameObject);
            Debug.Log($"El loro destruyó la valla: {other.gameObject.name}");
        }
    }
}