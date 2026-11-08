using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class VilanActivation : MonoBehaviour
{
    [SerializeField] private Enemie enemy;

    private void Awake()
    {
        if (enemy == null)
            Debug.LogError("Assign the enemy to the activation zone.", this);
        if (!GetComponent<BoxCollider2D>().isTrigger)
            Debug.LogError("Enable Is Trigger on the activation zone's BoxCollider2D.", this);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (enemy != null && other.GetComponentInParent<PlayerScript>() != null)
            enemy.Activate();
    }
}
