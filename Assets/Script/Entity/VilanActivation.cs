using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class VilanActivation : MonoBehaviour
{
    [SerializeField] private Enemie enemy;

    private void Awake()
    {
        if (enemy == null)
            Debug.LogError("Assigne l'ennemi a la zone d'activation.", this);
        if (!GetComponent<BoxCollider2D>().isTrigger)
            Debug.LogError("Active Is Trigger sur le BoxCollider2D de la zone d'activation.", this);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (enemy != null && other.GetComponentInParent<PlayerScript>() != null)
            enemy.Activate();
    }
}
