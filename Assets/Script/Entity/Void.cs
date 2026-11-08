using UnityEngine;

public class Void : MonoBehaviour
{
    [SerializeField] private Enemie catFight;

    private void Awake()
    {
        if (catFight == null)
            Debug.LogError("Assigne l'ennemi CatFight au Void.", this);
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
            Debug.LogError("Impossible de remettre CatFight au depart : aucun ennemi assigne.", this);

        player.TakeDamage();
    }
}
