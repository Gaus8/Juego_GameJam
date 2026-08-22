using UnityEngine;

public class camera : MonoBehaviour
{
    [Header("Objetivo")]
    public Transform target; // Arrastra a tu personaje aquí

    [Header("Configuración de Posición")]
    public Vector3 offset = new Vector3(0f, 2f, -10f); // Distancia relativa al jugador
    public float smoothSpeed = 5f;                    // Suavizado de movimiento

    [Header("Límites de Movimiento (Opcional)")]
    public bool useLimits = false;
    public float minX = -10f;
    public float maxX = 50f;
    public float minY = 0f;
    public float maxY = 10f;

    private void LateUpdate()
    {
        if (target == null) return;

        // Calcula la posición deseada sumando el offset al jugador
        Vector3 desiredPosition = target.position + offset;

        // Aplica límites si están activados
        if (useLimits)
        {
            desiredPosition.x = Mathf.Clamp(desiredPosition.x, minX, maxX);
            desiredPosition.y = Mathf.Clamp(desiredPosition.y, minY, maxY);
        }

        // Interpola suavemente entre la posición actual y la deseada
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // Aplica la posición final
        transform.position = smoothedPosition;
    }
}