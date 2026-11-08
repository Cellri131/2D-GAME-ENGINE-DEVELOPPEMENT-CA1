using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Checkpoint : MonoBehaviour
{
    [SerializeField] private AnimationClip checkpointActivate;

    private static Checkpoint activeCheckpoint;
    private SpriteRenderer spriteRenderer;
    private Sprite baseSprite;
    private float animationTime;
    private bool isAnimating;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        baseSprite = spriteRenderer.sprite;

        Animator animator = GetComponent<Animator>();
        if (animator != null)
            animator.enabled = false;

        if (checkpointActivate == null)
            Debug.LogError("Assigne le clip CheckpointActivate au checkpoint.", this);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponentInParent<PlayerScript>() != null)
            Hit();

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.GetComponentInParent<PlayerScript>() != null)
            Hit();
    }

    public void Hit()
    {
        if (activeCheckpoint == this)
            return;

        if (checkpointActivate == null)
        {
            Debug.LogError("Impossible d'activer le checkpoint sans clip d'animation.", this);
            return;
        }

        if (activeCheckpoint != null)
            activeCheckpoint.ResetVisual();

        activeCheckpoint = this;
        animationTime = 0f;
        isAnimating = true;
        checkpointActivate.SampleAnimation(gameObject, 0f);
    }

    private void LateUpdate()
    {
        if (!isAnimating)
            return;

        animationTime += Time.deltaTime;
        float lastFrameTime = Mathf.Max(0f, checkpointActivate.length - 0.0001f);
        checkpointActivate.SampleAnimation(gameObject, Mathf.Min(animationTime, lastFrameTime));

        if (animationTime >= checkpointActivate.length)
            isAnimating = false;
    }

    private void ResetVisual()
    {
        isAnimating = false;
        spriteRenderer.sprite = baseSprite;
    }

    private void OnDisable()
    {
        if (activeCheckpoint == this)
        {
            ResetVisual();
            activeCheckpoint = null;
        }
    }
}
