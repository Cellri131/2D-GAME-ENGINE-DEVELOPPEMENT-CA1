using UnityEngine;

public class PlayerMouvementScriptLevel2 : MonoBehaviour
{
    public float movementSpeed = 5f;
    public Rigidbody2D rb;

    public Animator anim;

    public float jumpForce = 20f;
    public Transform feet;

    public LayerMask groundLayers;

    private float mx;

    // Pente
    private Vector2 groundNormal = Vector2.up;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        mx = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump") && IsGrounded())
            Jump();

        if (Mathf.Abs(mx) > 0.05f)
            anim.SetBool("IsWalking", true);
        else
            anim.SetBool("IsWalking", false);

        if (mx > 0f)
            transform.localScale = new Vector3(1f, 1f, 1f);
        else if (mx < 0f)
            transform.localScale = new Vector3(-1f, 1f, 1f);

        anim.SetBool("IsRunning", true);
        anim.SetBool("IsGrounded", IsGrounded());
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(
            mx * movementSpeed,
            rb.linearVelocity.y
        );

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


}