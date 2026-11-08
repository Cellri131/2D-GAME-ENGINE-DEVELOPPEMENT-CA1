using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviour
{
    #region Variables
    public float movementSpeed = 5f;
    public float sprintSpeed = 5f;
    public Rigidbody2D rb;

    public Animator anim;

    public float jumpForce = 20f;
    public Transform feet;

    public LayerMask groundLayers;

    bool isBouncing = false;
    private float mx;

    // Slope
    private Vector2 groundNormal = Vector2.up;

    private int direction = 1;
    private Vector2 lastVelocity;
    private float bounceTimer;
    public float bounceLockTime = 0.3f;
    // Surfaces with a normal Y above this value (slopes) do not cause a bounce.
    public float maxWallNormalY = 0.5f;
    private bool sprint;

    // hp part
    private Vector2 lastPosition;

    private int lives = 3;
    private LevelAudio levelAudio;
    
    #endregion

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        lastPosition = rb.transform.position;
        levelAudio = GetComponent<LevelAudio>();
    }

    private void Start()
    {
        // Initialize UI values at the start of the game
        if (GameUIManager.Instance != null)
        {
            GameUIManager.Instance.UpdateLives(lives);
            GameUIManager.Instance.UpdateCollectibles(collectibleCount);
            GameUIManager.Instance.UpdateSpeed(SpeedMultiplier);
        }
    }


    private void Update()
    {
        mx = Input.GetAxisRaw("Horizontal");
        sprint = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        bool grounded = IsGrounded();

        if ((Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.UpArrow)) && grounded)
            Jump();

        if (mx > 0.05f)
            direction = 1;
        else if (mx < -0.05f)
            direction = -1;

        anim.SetFloat("Move", Mathf.Abs(mx) > 0.05f ? mx : 0f);
        anim.SetInteger("Direction", direction);
        anim.SetBool("Sprint", sprint && Mathf.Abs(mx) > 0.05f);
        anim.SetBool("isJumping", !grounded);
    }

    private void FixedUpdate()
    {
        if (bounceTimer > 0f)
            bounceTimer -= Time.fixedDeltaTime;
        else
        {
            float speed = (sprint ? movementSpeed + sprintSpeed : movementSpeed) * SpeedMultiplier;
            rb.linearVelocity = new Vector2(mx * speed, rb.linearVelocity.y);
        }

        lastVelocity = rb.linearVelocity;

        HandleSlopeRotation();
    }
    private void HandleSlopeRotation()
    {
        if (!IsGrounded())
        {
            transform.rotation = Quaternion.identity;
            return;
        }

        RaycastHit2D hit = Physics2D.Raycast(
            feet.position,
            Vector2.down,
            1f,
            groundLayers
        );

        if (hit.collider != null)
        {
            groundNormal = hit.normal;
            float angle = Vector2.SignedAngle(Vector2.up, groundNormal);

            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }

    #region Collectible Speed Bonus
    // Each collectible permanently adds this fraction of the base speed.
    public float speedBonusPerCollectible = 0.1f;
    private int collectibleCount;
    private float SpeedMultiplier => 1f + collectibleCount * speedBonusPerCollectible;

    public void AddCollectible()
    {
        collectibleCount++;
        if (levelAudio != null)
            levelAudio.PlayCollectible();

        // Update UI collectible counter
        if (GameUIManager.Instance != null)
        {
            GameUIManager.Instance.UpdateSpeed(SpeedMultiplier);
            GameUIManager.Instance.UpdateCollectibles(collectibleCount);
        }
    }

    #endregion

    void Jump()
    {
        rb.linearVelocity = new Vector2(
            rb.linearVelocity.x,
            jumpForce
        );
    }



    public bool IsGrounded()
    {
        Collider2D groundCheck = Physics2D.OverlapCircle(
            feet.position,
            0.9f,
            groundLayers
        );

        return groundCheck != null;
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.GetComponentInParent<Enemie>() != null)
            return;

        for (int i = 0; i < collision.contactCount; i++)
        {
            ContactPoint2D contact = collision.GetContact(i);
            if (Mathf.Abs(contact.normal.y) > maxWallNormalY)
                continue;

            float xSpeed = Mathf.Max(Mathf.Abs(lastVelocity.x), movementSpeed);
            rb.linearVelocity = new Vector2(Mathf.Sign(contact.normal.x) * xSpeed, lastVelocity.y);
            bounceTimer = bounceLockTime;
            return;
        }

        if (collision.gameObject.tag == "Vilan")
        {
            TakeDamage();
        }
    }

    public void TakeDamage()
    {
        lives--;
        if (levelAudio != null)
            levelAudio.PlayHit(lives <= 0);
        Debug.Log(lives);

        if (GameUIManager.Instance != null)
        {
            GameUIManager.Instance.UpdateLives(lives);
        }

        rb.position = lastPosition;
        rb.linearVelocity = Vector2.zero;
        bounceTimer = 0f;
        if (lives <= 0)
            SceneManager.LoadScene("Menu");
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        Collider2D collision = other;
        if(collision.gameObject.tag == "Checkpoint")
        {
            lastPosition = collision.transform.position;
            collision.gameObject.GetComponent<Checkpoint>().Hit();
        }
    }
}