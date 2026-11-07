using UnityEngine;

public class InfiniteParallax : MonoBehaviour
{
    [Header("Camera Setup")]
    public Transform cameraTransform;

    [Header("Effect Intensity")]
    [Tooltip("0 = Moves perfectly with camera (infinite distance). 1 = Stays completely still in world.")]
    [Range(0f, 1f)]
    public float parallaxFactor;

    [Tooltip("Frames during which the layer is re-anchored to the camera, to let a follow camera settle on the player.")]
    public int settleFrames = 2;

    [Tooltip("ON: the layer follows the camera vertically (keeps its editor offset). OFF: vertical parallax using the factor.")]
    public bool followVertically = true;

    private float textureUnitSizeX;
    private Vector3 lastCameraPosition;
    private Vector2 editorOffsetFromCamera;
    private int framesLeft;

    void Awake()
    {
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }

        // Offset between this layer and the camera exactly as placed in the editor,
        // captured before any follow camera moves.
        editorOffsetFromCamera = transform.position - cameraTransform.position;
        lastCameraPosition = cameraTransform.position;
        framesLeft = settleFrames;

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            textureUnitSizeX = spriteRenderer.sprite.bounds.size.x * transform.localScale.x;
        }
    }

    void LateUpdate()
    {
        if (cameraTransform == null) return;

        Vector3 camPos = cameraTransform.position;

        // While the camera settles, keep the editor layout relative to it.
        if (framesLeft > 0)
        {
            framesLeft--;
            transform.position = new Vector3(
                camPos.x + editorOffsetFromCamera.x,
                camPos.y + editorOffsetFromCamera.y,
                transform.position.z);
            lastCameraPosition = camPos;
            return;
        }

        // 0 = follows camera (sky) | 1 = stays static in world (foreground)
        float invertedFactor = 1f - parallaxFactor;
        Vector3 delta = camPos - lastCameraPosition;
        lastCameraPosition = camPos;

        transform.position += new Vector3(delta.x * invertedFactor, 0f, 0f);

        if (followVertically)
            transform.position = new Vector3(transform.position.x, camPos.y + editorOffsetFromCamera.y, transform.position.z);
        else
            transform.position += new Vector3(0f, delta.y * invertedFactor, 0f);

        // Horizontal infinite scrolling
        float diff = camPos.x - (transform.position.x - editorOffsetFromCamera.x);
        if (textureUnitSizeX > 0f && Mathf.Abs(diff) >= textureUnitSizeX)
        {
            float shift = diff - (diff % textureUnitSizeX);
            transform.position += new Vector3(shift, 0f, 0f);
        }
    }
}
