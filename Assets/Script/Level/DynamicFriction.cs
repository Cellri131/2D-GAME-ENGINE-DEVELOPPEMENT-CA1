using UnityEngine;

public class DynamicFriction : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D playerCollider; // Modifié pour Box Collider 2D

    [Header("Matériaux Physiques")]
    public PhysicsMaterial2D slipperyMaterial;
    public PhysicsMaterial2D fullFrictionMaterial;

    [Header("Détection Sol / Mur")]
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float checkRadius = 0.2f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<BoxCollider2D>(); // Récupère le Box Collider
    }

    void Update()
    {
        // Détecte si le joueur touche le sol ou une pente
        bool isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
        float moveInput = Input.GetAxisRaw("Horizontal");

        if (isGrounded && Mathf.Abs(moveInput) < 0.01f)
        {
            // Immobile au sol/pente : friction maximale
            playerCollider.sharedMaterial = fullFrictionMaterial;
        }
        else
        {
            // En mouvement ou contre un mur : aucune friction
            playerCollider.sharedMaterial = slipperyMaterial;
        }

        // Force Unity à actualiser la physique immédiatement (TRÈS IMPORTANT pour le Box Collider)
        playerCollider.enabled = false;
        playerCollider.enabled = true;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
            Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
    }
}