using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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

    // Pente
    private Vector2 groundNormal = Vector2.up;
    #endregion

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    private int direction = 1;
    private Vector2 lastVelocity;
    private float bounceTimer;
    public float bounceLockTime = 0.3f;
    private bool sprint;

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
            float speed = sprint ? movementSpeed + sprintSpeed : movementSpeed;
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
        for (int i = 0; i < collision.contactCount; i++)
        {
            ContactPoint2D contact = collision.GetContact(i);
            if (Mathf.Abs(contact.normal.x) <= Mathf.Abs(contact.normal.y))
                continue;

            float xSpeed = Mathf.Max(Mathf.Abs(lastVelocity.x), movementSpeed);
            rb.linearVelocity = new Vector2(Mathf.Sign(contact.normal.x) * xSpeed, lastVelocity.y);
            bounceTimer = bounceLockTime;
            return;
        }
    }
}