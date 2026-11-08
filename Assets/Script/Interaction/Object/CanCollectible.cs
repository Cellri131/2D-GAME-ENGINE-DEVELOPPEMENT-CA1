using UnityEngine;

public class CanCollectible : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        PlayerScript player = collision.GetComponentInParent<PlayerScript>();
        if (player == null) return;

        player.AddCollectible();
        Destroy(gameObject);
    }
}
