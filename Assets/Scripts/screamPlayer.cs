using UnityEngine;

[RequireComponent(typeof(Collider))]
public class screamPlayer : MonoBehaviour
{
    [Header("Configuración del Susto")]
    [Tooltip("Cantidad de vida de asma que resta este objeto al tocarlo.")]
    public float scareDamageAmount = 25f;

    [Tooltip("Si está activado, el objeto se destruirá después de asustar una vez.")]
    public bool destroyOnUse = true;

    [Header("Sonido del Susto")]
    [Tooltip("Clip de audio que sonará al asustar.")]
    public AudioClip scareSound;

    [Tooltip("Volumen del sonido (entre 0.0 y 1.0).")]
    [Range(0f, 1f)]
    public float soundVolume = 1f;

    private void Start()
    {
        Collider col = GetComponent<Collider>();
        if (!col.isTrigger)
        {
            col.isTrigger = true;
            Debug.LogWarning($"El Collider en {gameObject.name} se activó como 'Is Trigger' automáticamente.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement player = other.GetComponent<PlayerMovement>();

            if (player != null)
            {
                player.DeductHealth(scareDamageAmount);
                player.GetScared();

                // Reproduce el sonido en la posición del objeto antes de destruirlo
                if (scareSound != null)
                {
                    AudioSource.PlayClipAtPoint(scareSound, transform.position, soundVolume);
                }

                Debug.Log($"<color=red>¡SUSTO!</color> {gameObject.name} asustó al jugador por {scareDamageAmount} de daño.");

                if (destroyOnUse)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}