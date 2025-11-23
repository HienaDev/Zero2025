using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerStats playerStats;

    [SerializeField] private float jumpHoldTime = 0.1f;
    private int jumpCount = 0;
    private float justJumpedTime;

    private Rigidbody2D rb;

    [SerializeField] private float coyoteTime = 0.2f;
    private float justGroundedTime;
    private bool grounded = false;
    private bool jumped = false;

    private GameObject sprite;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private SpriteRenderer cannonSprite;

    [SerializeField] private float fallGravityMultiplier = 2.5f;
    private float originalGravityScale;

    [SerializeField] private Animator animator;

    private float conveyorBeltSpeed = 0f;

    private bool canMove = true;

    // 🔹 NEW: reference to an object the player should follow when grabbed
    private Transform followTarget;
    [SerializeField] private float followSmoothness = 10f;

    public void SetConveyorBeltSpeed(float conveyorSpeed) => conveyorBeltSpeed = conveyorSpeed;

    public void SetGrounded(bool isGrounded)
    {
        if (!isGrounded && grounded)
        {
            justGroundedTime = Time.time;
        }
        if (isGrounded)
        {
            jumpCount = 0;
        }
        grounded = isGrounded;
    }

    void Start()
    {
        playerStats = GetComponent<PlayerStats>();
        rb = GetComponent<Rigidbody2D>();
        originalGravityScale = rb.gravityScale;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        sprite = spriteRenderer.gameObject;
    }

    void Update()
    {

        if(Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.G))
        {
            SetGrounded(true);
        }

        // 🔹 If movement is disabled but following a target, smoothly follow it
        if (!canMove && followTarget != null)
        {
            transform.position = Vector3.Lerp(
                transform.position,
                followTarget.position,
                Time.deltaTime * followSmoothness
            );
            return;
        }

        if (!canMove) return;

        Vector2 velocity = rb.linearVelocity;
        float inputX = Input.GetAxis("Horizontal");
        float targetX = inputX * playerStats.MaxSpeed;

        if (grounded)
            targetX += conveyorBeltSpeed;

        velocity.x = Mathf.MoveTowards(velocity.x, targetX, playerStats.Acceleration * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && ((grounded || justGroundedTime + coyoteTime > Time.time) || playerStats.JumpNumber > jumpCount))
        {
            jumpCount++;
            justJumpedTime = Time.time;
            jumped = true;
            grounded = false;
        }

        if (Input.GetButtonUp("Jump"))
        {
            justJumpedTime = -jumpHoldTime;
        }

        if (jumped)
            animator.SetFloat("SpeedY", velocity.y);
        else
            animator.SetFloat("SpeedY", 0);

        animator.SetBool("Jumped", jumped);

        if (Input.GetButton("Jump") && Time.time < justJumpedTime + jumpHoldTime)
            velocity.y = playerStats.JumpForce;

        if (velocity.y < 0)
            rb.gravityScale = originalGravityScale * fallGravityMultiplier;
        else
            rb.gravityScale = originalGravityScale;

        rb.linearVelocity = velocity;

        if (grounded)
            animator.SetFloat("SpeedX", Mathf.Abs(velocity.x) - Mathf.Abs(conveyorBeltSpeed));
        else
            animator.SetFloat("SpeedX", 0);

        UpdateSprite();
    }

    public void EnablePlayerMovement(bool toggle)
    {
        if (!toggle)
        {
            rb.linearVelocity = Vector2.zero;
            canMove = false;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
        else
        {
            canMove = true;
            rb.bodyType = RigidbodyType2D.Dynamic;
            followTarget = null; // 🔹 stop following when regaining control
        }
    }

    // 🔹 NEW: assign a target for the player to follow (e.g., claw)
    public void FollowTarget(Transform target)
    {
        followTarget = target;
        EnablePlayerMovement(false);
    }

    public void ApplyExternalForce(Vector2 externalForce)
    {
        rb.AddForce(externalForce, ForceMode2D.Impulse);
    }

    void UpdateSprite()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (mousePosition.x < transform.position.x)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }
    }
}
