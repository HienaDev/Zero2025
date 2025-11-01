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

    public void SetGrounded(bool isGrounded)
    {
        if (!isGrounded && grounded)
        {
            jumped = false;
            justGroundedTime = Time.time;
        }
        if (isGrounded)
        {
            jumpCount = 0;
        }
        grounded = isGrounded;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerStats = GetComponent<PlayerStats>();
        rb = GetComponent<Rigidbody2D>();

        originalGravityScale = rb.gravityScale;

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        sprite = spriteRenderer.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 velocity = rb.linearVelocity;

        // Handle horizontal movement
        float inputX = Input.GetAxis("Horizontal");

        velocity.x = Mathf.MoveTowards(velocity.x, inputX * playerStats.MaxSpeed, playerStats.Acceleration * Time.deltaTime);

        // Handle jumping
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
        {
            animator.SetFloat("SpeedY", velocity.y);
        }
        else
        {
            animator.SetFloat("SpeedY", 0);
        }

        //if (jumped && velocity.y < 0) jumped = false;

        animator.SetBool("Jumped", jumped); 

        if (Input.GetButton("Jump") && Time.time < justJumpedTime + jumpHoldTime)
        {
            velocity.y = playerStats.JumpForce;
        }

        // Handle fall gravity
        if (velocity.y < 0)
        {
            rb.gravityScale = originalGravityScale * fallGravityMultiplier;
        }
        else
        {
            rb.gravityScale = originalGravityScale;
        }

        rb.linearVelocity = velocity;

        if (grounded)
            animator.SetFloat("SpeedX", Mathf.Abs(velocity.x));
        else
            animator.SetFloat("SpeedX", 0);

        UpdateSprite();
    }

    public void ApplyExternalForce(Vector2 externalForce)
    {
        rb.AddForce(externalForce, ForceMode2D.Impulse);
    }

    void UpdateSprite()
    {
        // check mouse position to flip sprite
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (mousePosition.x < transform.position.x)
        {
            spriteRenderer.flipX = true;
            //cannonSprite.flipY = true;
            //cannonSprite.transform.eulerAngles = new Vector3(0, 180, cannonSprite.transform.eulerAngles.z);
        }
        else
        {
            spriteRenderer.flipX = false;
            //cannonSprite.flipY = false;
            //cannonSprite.transform.eulerAngles = new Vector3(0, 0, cannonSprite.transform.eulerAngles.z);
        }
    }
}
