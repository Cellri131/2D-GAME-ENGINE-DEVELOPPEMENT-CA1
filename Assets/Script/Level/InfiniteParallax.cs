using UnityEngine;

public class InfiniteParallax : MonoBehaviour
{
    [Header("Camera Setup")]
    public Transform cameraTransform;

    [Header("Effect Intensity")]
    [Tooltip("0 = Moves perfectly with camera (infinite distance). 1 = Stays completely still in world.")]
    [Range(0f, 1f)]
    public float parallaxFactor;

    private float textureUnitSizeX;
    private Vector3 lastCameraPosition;

    void Start()
    {
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }

        if (cameraTransform != null)
        {
            lastCameraPosition = cameraTransform.position;
        }

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            textureUnitSizeX = spriteRenderer.sprite.bounds.size.x * transform.localScale.x;
        }
    }

    // Changé en Update() pour s'aligner parfaitement avec les mouvements fluides de Cinemachine
    void Update()
    {
        if (cameraTransform == null) return;

        // CORRECTION : On inverse le facteur pour que la logique corresponde à vos réglages
        // 0 = suit la caméra (ciel) | 1 = reste immobile dans le monde (premier plan)
        float invertedFactor = 1f - parallaxFactor;

        // 1. Applique l'effet de parallaxe (profondeur)
        Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;
        Vector3 movement = deltaMovement * invertedFactor;
        transform.position += new Vector3(movement.x, 0, 0);

        lastCameraPosition = cameraTransform.position;

        // 2. Logique de scrolling infini
        if (Mathf.Abs(cameraTransform.position.x - transform.position.x) >= textureUnitSizeX)
        {
            float offsetPositionX = (cameraTransform.position.x - transform.position.x) % textureUnitSizeX;
            transform.position = new Vector3(cameraTransform.position.x - offsetPositionX, transform.position.y, transform.position.z);
        }
    }

}
