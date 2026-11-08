using UnityEngine;

public class Void : MonoBehaviour
{
    [SerializeField] private Enemie catFight;

    private void Awake()
    {
        if (catFight == null)
            Debug.LogError("Assign the CatFight enemy to the Void.", this);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandleContact(collision.collider);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        HandleContact(other);
    }

    private void HandleContact(Collider2D other)
    {
        PlayerScript player = other.GetComponentInParent<PlayerScript>();
        if (player == null)
            return;

        if (catFight != null)
            catFight.ReturnToStart();
        else
            Debug.LogError("Cannot return CatFight to its starting position: no enemy assigned.", this);

        player.TakeDamage();
    }
}
