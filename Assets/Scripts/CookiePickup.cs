using UnityEngine;

public class CookiePickup : MonoBehaviour
{
    public int amount = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement player = other.GetComponent<PlayerMovement>();
            if (player != null)
            {
                player.AddCookies(amount);
                Destroy(gameObject);
            }
        }
    }
}