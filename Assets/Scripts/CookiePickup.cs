using UnityEngine;

public class CookiePickup : MonoBehaviour
{
    public int amount = 1;

    [Header("Sonido de Recolección")]
    [Tooltip("Clip de audio que sonará al recoger la galleta.")]
    public AudioClip pickupSound;

    [Tooltip("Volumen del sonido (entre 0.0 y 1.0).")]
    [Range(0f, 1f)]
    public float soundVolume = 1f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement player = other.GetComponent<PlayerMovement>();
            if (player != null)
            {
                player.AddCookies(amount);

                // Reproduce el sonido en la posición del objeto antes de destruirlo
                if (pickupSound != null)
                {
                    AudioSource.PlayClipAtPoint(pickupSound, transform.position, soundVolume);
                }

                Destroy(gameObject);
            }
        }
    }
}