using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    [Header("Configuration")]
    public Transform cameraTransform; // Drag your Main Camera here
    
    public enum ParallaxAxis { Both, HorizontalOnly, VerticalOnly }
    [Header("Axis Setup")]
    public ParallaxAxis axisToApply = ParallaxAxis.HorizontalOnly;

    [Header("Effect Intensity")]
    [Tooltip("0 = Moves perfectly with the camera (stays fixed on screen). 1 = Stays completely still in the world (maximum depth effect).")]
    [Range(0f, 1f)]
    public float parallaxFactor; 

    private Vector3 lastCameraPosition;

    void Start()
    {
        // If the camera is not assigned, automatically target the Main Camera
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }
        
        lastCameraPosition = cameraTransform.position;
    }

    void LateUpdate()
    {
        // Calculate how much the camera moved since the last frame
        Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;
        
        // Calculate the movement to apply to this specific background layer
        Vector3 movement = deltaMovement * parallaxFactor;

        // Apply movement based on the selected axis
        if (axisToApply == ParallaxAxis.HorizontalOnly)
        {
            transform.position += new Vector3(movement.x, 0, 0);
        }
        else if (axisToApply == ParallaxAxis.VerticalOnly)
        {
            transform.position += new Vector3(0, movement.y, 0);
        }
        else
        {
            transform.position += movement;
        }

        // Save the camera position for the next frame
        lastCameraPosition = cameraTransform.position;
    }
}
