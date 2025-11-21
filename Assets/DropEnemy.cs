using UnityEngine;

public class DropEnemy : MonoBehaviour
{
    [Header("References")]
    public Transform enemy;       // Parent object
    public Transform claw;        // Child object

    [Header("Settings")]
    public float initialDropDistance = 3f;

    [Tooltip("Speed while the claw is still attached (initial drop).")]
    public float attachedDropSpeed = 2f;

    [Tooltip("Speed of the enemy after the claw begins rising.")]
    public float enemyDropSpeed = 3f;

    public float clawRiseSpeed = 3f;

    [Header("Ground Detection")]
    public Vector2 groundCheckSize = new Vector2(1f, 0.2f);
    public Vector2 groundCheckOffset = new Vector2(0f, -0.5f);
    public LayerMask groundMask;          // Layers that CAN be ground

    private float startY;
    private bool initialDropping = true;
    private bool secondaryDropping = false;

    [SerializeField] private Animator animator;

    void Start()
    {
        if (enemy == null) enemy = this.transform;
        startY = enemy.position.y;

        attachedDropSpeed += Random.Range(-0.5f, 0.5f);
    }

    void Update()
    {
        if (initialDropping)
            HandleInitialDrop();
        else if (secondaryDropping)
            HandleSecondaryDrop();

        CheckGroundHit();
    }

    void HandleInitialDrop()
    {
        enemy.position += Vector3.down * attachedDropSpeed * Time.deltaTime;

        if (startY - enemy.position.y >= initialDropDistance)
        {
            initialDropping = false;
            secondaryDropping = true;
        }
    }

    void HandleSecondaryDrop()
    {
        enemy.position += Vector3.down * enemyDropSpeed * Time.deltaTime;
        claw.localPosition += Vector3.up * clawRiseSpeed * Time.deltaTime;
    }

    void CheckGroundHit()
    {
        // World position for overlap box
        Vector2 checkPos = (Vector2)enemy.position + groundCheckOffset;

        Collider2D hit = Physics2D.OverlapBox(checkPos, groundCheckSize, 0f, groundMask);

        if (hit == null)
            return;

        // Must have TAG_Ground
        TAG_Ground groundTag = hit.GetComponent<TAG_Ground>();
        if (groundTag == null)
            return;

        // Must NOT have TAG_Box
        TAG_Box boxTag = hit.GetComponent<TAG_Box>();
        if (boxTag != null)
            return;

        // Valid ground hit → enable movement
        GroundMovement gm = enemy.GetComponent<GroundMovement>();
        if (gm != null)
            gm.enabled = true;

        secondaryDropping = false;
        animator.enabled = true;
    }

    // Draw the overlap box in the editor for debugging
    void OnDrawGizmosSelected()
    {
        if (enemy == null) return;

        Gizmos.color = Color.green;
        Vector2 checkPos = (Vector2)enemy.position + groundCheckOffset;
        Gizmos.DrawWireCube(checkPos, groundCheckSize);
    }
}
