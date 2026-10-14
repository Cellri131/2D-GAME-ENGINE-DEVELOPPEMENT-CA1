using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    //not for now but we will need it after
    //[SerializeField] private GameManager gameManger;

    #region Variable

    //movement var
    private Vector2 move;
    private Vector2 groundNormal = Vector2.up;
    private Rigidbody2D rb;
    private int direction;
    private bool sprint = false;

    public int sprintSpeed = 6;
    public int walkSpeed = 3;

    private bool isGrounded = false;

    //wall jump
    private bool isTouchingWall = false;
    private int wallDirection = 0; // -1 = mur à gauche, 1 = mur à droite

    public float wallJumpForceX = 8f;
    public float wallJumpForceY = 6f;

    //checkpoint
    private Vector2 lastPosition;

    //animation part
    private Animator animator;

    //jump part
    public int JumpHeight = 1;
    public bool isJumping = false;

    //for the live it's only one but with checkpoint, you died you restart everything, no death screen at the moment
    private int lives = 1;

    private int score = 0;
    private int comboTypes = 0;

    #endregion

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        lastPosition = rb.transform.position;
    }

    void Update()
    {
        move = InputSystem.actions["Move"].ReadValue<Vector2>();

        // Ignore les touches haut/bas
        move.y = 0;

        sprint = Keyboard.current != null
                 && Keyboard.current.shiftKey.isPressed
                 && move.x != 0;

        animator.SetBool("Sprint", sprint);
        animator.SetFloat("Move", move.x);

        // Jump / Wall Jump
        if (InputSystem.actions["Jump"].WasPressedThisFrame())
        {
            // Saut normal
            if (isGrounded)
            {
                isJumping = true;
                isGrounded = false;

                rb.linearVelocity = new Vector2(
                    rb.linearVelocity.x,
                    0f
                );

                rb.AddForce(
                    new Vector2(
                        0,
                        Mathf.Sqrt(-2 * Physics2D.gravity.y * JumpHeight)
                    ),
                    ForceMode2D.Impulse
                );
            }
            // Wall Jump
            else if (isTouchingWall)
            {
                isJumping = true;

                rb.linearVelocity = Vector2.zero;

                rb.AddForce(
                    new Vector2(
                        -wallDirection * wallJumpForceX,
                        wallJumpForceY
                    ),
                    ForceMode2D.Impulse
                );
            }
        }

        // Direction
        if (move.x != 0)
        {
            direction = move.x > 0 ? 1 : -1;
            animator.SetInteger("Direction", direction);
        }
    }

    #region Collision

    private void OnCollisionEnter2D(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            // Sol
            if (contact.normal.y > 0.5f)
            {
                isGrounded = true;
                isJumping = false;
                groundNormal = contact.normal;
            }

            // Mur à droite
            if (contact.normal.x < -0.5f)
            {
                isTouchingWall = true;
                wallDirection = 1;
            }

            // Mur à gauche
            if (contact.normal.x > 0.5f)
            {
                isTouchingWall = true;
                wallDirection = -1;
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        isTouchingWall = false;

        foreach (ContactPoint2D contact in collision.contacts)
        {
            // Sol / Pentes
            if (contact.normal.y > 0.1f)
            {
                isGrounded = true;
                groundNormal = contact.normal;
            }

            // Mur à droite
            if (contact.normal.x < -0.5f)
            {
                isTouchingWall = true;
                wallDirection = 1;
            }

            // Mur à gauche
            if (contact.normal.x > 0.5f)
            {
                isTouchingWall = true;
                wallDirection = -1;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        isGrounded = false;
        isTouchingWall = false;
    }

    #endregion

    private void FixedUpdate()
    {
        int currentSpeed = sprint ? sprintSpeed : walkSpeed;

        Vector2 position = rb.transform.position;

        if (isGrounded)
        {
            float angle = Vector2.SignedAngle(Vector2.up, groundNormal);

            rb.rotation = angle;

            // Déplacement horizontal
            position.x += move.x * currentSpeed * Time.fixedDeltaTime;

            // Compensation verticale pour suivre la pente
            position.y += move.x * (-groundNormal.x) * currentSpeed * Time.fixedDeltaTime;
        }
        else
        {
            rb.rotation = 0f;

            position.x += move.x * currentSpeed * Time.fixedDeltaTime;
        }

        rb.transform.position = position;
    }
}