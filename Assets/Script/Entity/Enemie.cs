using UnityEngine;

// Based on the 3D Game Engine Development course.
[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class Enemie : MonoBehaviour
{
    #region Configuration and State

    [SerializeField, Min(0f)] private float speed = 2f;
    [SerializeField] private Transform pathParent;

    private Rigidbody2D rb;
    private Vector2 initialPosition;
    private Transform[] path;
    private int nextPoint;
    private bool pathIsValid;
    private bool isActivated;

    public bool IsActivated => isActivated;

    #endregion

    #region Initialization

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        initialPosition = rb.position;
        rb.bodyType = RigidbodyType2D.Kinematic;

        Animator animator = GetComponent<Animator>();
        if (animator.runtimeAnimatorController == null || !animator.HasState(0, Animator.StringToHash("CatFight")))
            Debug.LogError("The Animator must contain the CatFight state.", this);
        else
            animator.Play("CatFight");

        if (pathParent == null || pathParent.IsChildOf(transform) || pathParent.childCount == 0)
        {
            Debug.LogError("Assign a non-empty path object outside the enemy hierarchy.", this);
            return;
        }

        // Index points by number rather than hierarchy order and reject missing or duplicate indices.
        path = new Transform[pathParent.childCount];
        foreach (Transform point in pathParent)
        {
            string name = point.name;
            if (name.Length < 2 || name[0] != 'P' ||
                !int.TryParse(name.Substring(1), out int number) ||
                number < 1 || number > path.Length || name != "P" + number ||
                path[number - 1] != null)
            {
                Debug.LogError("The path must contain exactly P1, P2, ... with no duplicates or gaps.", this);
                return;
            }
            path[number - 1] = point;
        }

        pathIsValid = true;
    }

    #endregion

    #region Patrol

    private void FixedUpdate()
    {
        if (!isActivated || !pathIsValid)
            return;

        Vector2 target = path[nextPoint].position;
        Vector2 position = Vector2.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime);
        rb.MovePosition(position);

        // Return to P1 after the last point without teleporting.
        if (position == target)
            nextPoint = (nextPoint + 1) % path.Length;
    }

    public void Activate()
    {
        isActivated = true;
    }

    public void ReturnToStart()
    {
        rb.position = initialPosition;
        rb.linearVelocity = Vector2.zero;
        nextPoint = 0;
    }

    #endregion

    #region Player Contact

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandleContact(collision.collider);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        HandleContact(other);
    }

    private void HandleContact(Collider2D other)
    {
        PlayerScript player = other.GetComponentInParent<PlayerScript>();
        if (player == null)
            return;

        ReturnToStart();
        player.TakeDamage();
    }

    #endregion
}
