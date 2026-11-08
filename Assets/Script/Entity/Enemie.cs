using UnityEngine;

//thanks to 3D game Engine dev courses
[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class Enemie : MonoBehaviour
{
    [SerializeField, Min(0f)] private float speed = 2f;
    [SerializeField] private Transform pathParent;

    private Rigidbody2D rb;
    private Vector2 initialPosition;
    private Transform[] path;
    private int nextPoint;
    private bool pathIsValid;
    private bool isActivated;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        initialPosition = rb.position;
        rb.bodyType = RigidbodyType2D.Kinematic;

        Animator animator = GetComponent<Animator>();
        if (animator.runtimeAnimatorController == null || !animator.HasState(0, Animator.StringToHash("CatFight")))
            Debug.LogError("L'Animator doit contenir l'etat CatFight.", this);
        else
            animator.Play("CatFight");

        if (pathParent == null || pathParent.IsChildOf(transform) || pathParent.childCount == 0)
        {
            Debug.LogError("Assigne un objet de chemin non vide et independant de l'ennemi.", this);
            return;
        }

        path = new Transform[pathParent.childCount];
        foreach (Transform point in pathParent)
        {
            string name = point.name;
            if (name.Length < 2 || name[0] != 'P' ||
                !int.TryParse(name.Substring(1), out int number) ||
                number < 1 || number > path.Length || name != "P" + number ||
                path[number - 1] != null)
            {
                Debug.LogError("Le chemin doit contenir exactement les points P1, P2, ... sans doublon.", this);
                return;
            }
            path[number - 1] = point;
        }

        pathIsValid = true;
    }

    private void FixedUpdate()
    {
        if (!isActivated || !pathIsValid)
            return;

        Vector2 target = path[nextPoint].position;
        Vector2 position = Vector2.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime);
        rb.MovePosition(position);

        if (position == target)
            nextPoint = (nextPoint + 1) % path.Length;
    }

    public void Activate()
    {
        isActivated = true;
    }

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

        rb.position = initialPosition;
        rb.linearVelocity = Vector2.zero;
        nextPoint = 0;
        player.TakeDamage();
    }
}
