using UnityEngine;

[RequireComponent(typeof(Collider))]
public class InhalerRefill : MonoBehaviour
{
    [Header("Configuración de Recarga")]
    [Tooltip("Porcentaje de carga que otorga al jugador al recogerlo.")]
    public float refillAmount = 25f;

    [Tooltip("Si está activado, el objeto se destruirá tras usarse.")]
    public bool destroyOnUse = true;

    [Header("Efectos Opcionales")]
    [Tooltip("Sonido opcional al recoger el cargador.")]
    public AudioClip pickupSound;

    [Range(0f, 1f)]
    public float soundVolume = 1f;

    private void Start()
    {
        // Forzar a que el Collider actúe como Trigger
        Collider col = GetComponent<Collider>();
        if (!col.isTrigger)
        {
            col.isTrigger = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement player = other.GetComponent<PlayerMovement>();

            if (player != null)
            {
                // Aplica la recarga de inhalador al jugador
                player.RechargeInhaler(refillAmount);

                // Sonido opcional
                if (pickupSound != null)
                {
                    AudioSource.PlayClipAtPoint(pickupSound, transform.position, soundVolume);
                }

                if (destroyOnUse)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}