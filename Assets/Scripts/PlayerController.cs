using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float jumpForce = 5f;

    [SerializeField] private float jumpHoldTime = 0.1f;
    private float justJumpedTime;

    private Rigidbody2D rb;

    [SerializeField] private float coyoteTime = 0.2f;
    private float justGroundedTime;
    private bool grounded = false;

    private GameObject sprite;
    private SpriteRenderer spriteRenderer;

    [SerializeField] private float fallGravityMultiplier = 2.5f;
    private float originalGravityScale;
    
    public void SetGrounded(bool isGrounded)
    {
        if (!isGrounded && grounded)
        {
            justGroundedTime = Time.time;
        }
        grounded = isGrounded;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

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

        velocity.x = Mathf.MoveTowards(velocity.x, inputX * maxSpeed, acceleration * Time.deltaTime);

        // Handle jumping
        if (Input.GetButtonDown("Jump") && (grounded || justGroundedTime + coyoteTime > Time.time))
        {
            justJumpedTime = Time.time;
            grounded = false;
        }

        if(Input.GetButtonUp("Jump"))
        {
            justJumpedTime = -jumpHoldTime;
        }

        if (Input.GetButton("Jump") && Time.time < justJumpedTime + jumpHoldTime)
        {
            velocity.y = jumpForce;
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

        UpdateSprite();
    }

    void UpdateSprite()
    {
        // check mouse position to flip sprite
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
