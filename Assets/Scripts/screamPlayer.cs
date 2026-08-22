using UnityEngine;

[RequireComponent(typeof(Collider))] // Obliga a que haya un Collider en el objeto
public class screamPlayer : MonoBehaviour
{
    [Header("Configuración del Susto")]
    [Tooltip("Cantidad de vida de asma que resta este objeto al tocarlo.")]
    public float scareDamageAmount = 25f;

    [Tooltip("Si está activado, el objeto se destruirá después de asustar una vez.")]
    public bool destroyOnUse = true;

    private void Start()
    {
        // Asegurarnos de que el Collider esté configurado correctamente como Trigger
        Collider col = GetComponent<Collider>();
        if (!col.isTrigger)
        {
            col.isTrigger = true;
            Debug.LogWarning($"El Collider en {gameObject.name} se activó como 'Is Trigger' automáticamente.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 1. Verificar si lo que entró en el trigger es el Jugador
        // Para esto, el objeto del Jugador DEBE tener el Tag "Player"
        if (other.CompareTag("Player"))
        {
            // 2. Intentar obtener el script PlayerMovement del objeto que entró
            PlayerMovement player = other.GetComponent<PlayerMovement>();

            // 3. Si el script existe, aplicar el daño
            if (player != null)
            {
                player.DeductHealth(scareDamageAmount);
                Debug.Log($"<color=red>¡SUSTO!</color> {gameObject.name} asustó al jugador por {scareDamageAmount} de daño.");

                // 4. Opcional: Destruir este objeto para que no asuste dos veces
                if (destroyOnUse)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}