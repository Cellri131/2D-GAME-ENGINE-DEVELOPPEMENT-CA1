using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    //not for now but we will need it after
    //[SerializeField] private GameManager gameManger;

    #region Variable
    //movement var
    private Vector2 move;
    private Rigidbody2D rb;
    private int direction;
    private bool sprint = false;
    public int sprintSpeed = 6;
    public int walkSpeed = 3;

    //checkpoint
    private Vector2 lastPosition;
    
    //animation part
    private Animator animator;

    //jump part
        public int JumpHeight = 1;
        public bool isJumping = false;
        //we will need jumpCount after for the wall jump possibility
        private int jumpCount = 0;
        public int jumpLimit = 1;

    //for the live it's only one but with chekcpoint, you died you restart everything, no death screen at the moment
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


    // Update is called once per frame
    void Update()
    {
        move = InputSystem.actions["Move"].ReadValue<Vector2>();
        // Skip all the up and down keys | Ignore complètement les touches haut/bas
        move.y = 0;

        sprint = Keyboard.current != null && Keyboard.current.shiftKey.isPressed && move.x != 0;

        //animator.SetBool("Sprint", sprint);
        //animator.SetFloat("Move", move.x);
        //animator.SetBool("isJumping", isJumping);

        if (move.x != 0)
        {
            direction = move.x > 0 ? 1 : -1;
            //animator.SetInteger("Direction", direction);
        }
    }

    private void FixedUpdate()
    {
        int currentSpeed = sprint ? sprintSpeed : walkSpeed;

        Vector2 position = rb.transform.position;

        // Only the X axe | Seul l'axe X est contrôlé par le joueur
        position.x += move.x * currentSpeed * Time.fixedDeltaTime;

        // No Y axe | Le Y n'est jamais modifié ici
        rb.transform.position = position;
    }


}
