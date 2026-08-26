using UnityEngine;

public class Teletransporte : MonoBehaviour
{
    public Transform Target;
    [Tooltip("Distancia a desplazarse en el eje X del Target")]
    public float distanciaAdelante = 1.5f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Apaga el CharacterController si el personaje lo utiliza para evitar bloqueos
            CharacterController cc = other.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            // Avanza en el eje X local del Target (Target.right es la flecha roja)
            Vector3 posicionDestino = Target.position + (Target.right * distanciaAdelante);

            other.transform.position = posicionDestino;

            // Hace que el personaje rote mirando hacia donde apunta el eje X del Target
            other.transform.rotation = Quaternion.LookRotation(Target.right);

            if (cc != null) cc.enabled = true;
        }
    }
}