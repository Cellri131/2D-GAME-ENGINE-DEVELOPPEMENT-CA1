using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Script.Player
{
    //not for now but we will need it after
    //[SerializeField] private GameManager gameManger;

    public class PlayerScriptSave : MonoBehaviour
    {
        
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
        private int wallDirection = 0; // -1 = leftwall, 1 = rightwall

        public float wallJumpForceX = 5f;
        public float wallJumpForceY = 5f;

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

            rb.freezeRotation = true;
        }

        void Update()
        {
            move = InputSystem.actions["Move"].ReadValue<Vector2>();

            // Ignore the up and down keys.
            move.y = 0;

            sprint = Keyboard.current != null
                     && Keyboard.current.shiftKey.isPressed
                     && move.x != 0;

            animator.SetBool("Sprint", sprint);
            animator.SetFloat("Move", Mathf.Abs(move.x));

            // Jump
            if (InputSystem.actions["Jump"].WasPressedThisFrame() && isGrounded)
            {
                isJumping = true;
                isGrounded = false;

                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);

                rb.AddForce(
                    new Vector2(
                        0,
                        Mathf.Sqrt(-2 * Physics2D.gravity.y * JumpHeight)
                    ),
                    ForceMode2D.Impulse
                );
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
                if (contact.normal.y > 0.5f)
                {
                    isGrounded = true;
                    isJumping = false;
                    groundNormal = contact.normal;
                    break;
                }
            }
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.y > 0.1f)
                {
                    isGrounded = true;
                    groundNormal = contact.normal;
                    return;
                }
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            isGrounded = false;
        }

        #endregion

        private void FixedUpdate()
        {
            int currentSpeed = sprint ? sprintSpeed : walkSpeed;

            Vector2 position = rb.transform.position;

            if (isGrounded)
            {
                Vector2 slopeDirection = new Vector2(
                    groundNormal.y,
                    -groundNormal.x
                ).normalized;

                position += slopeDirection * move.x * currentSpeed * Time.fixedDeltaTime;
            }
            else
            {
                position.x += move.x * currentSpeed * Time.fixedDeltaTime;
            }

            /*
            // Rotate according to the slope.
            if (isGrounded)
            {

                float angle = Vector2.SignedAngle(Vector2.up, groundNormal);

                rb.MoveRotation(angle);
            }
            else
            {
                rb.rotation = 0f;
            }*/
            rb.transform.position = position;
        }
    }
}