using UnityEngine;

public class GroundMovement : MonoBehaviour
{
    private Enemy enemy;
    private PlayerController player;
    private ConveyorBelt conveyorBelt;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        player = FindAnyObjectByType<PlayerController>();
        enemy = GetComponent<Enemy>();
        conveyorBelt = FindAnyObjectByType<ConveyorBelt>(); // Get the global or nearest conveyor
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        MoveTowardsPlayer();
        FlipSprite();
    }

    public void MoveTowardsPlayer()
    {
        if (player == null || enemy == null) return;

        // Movement direction toward player (only X)
        Vector3 direction = (player.transform.position - transform.position).normalized;
        direction = new Vector3(direction.x, 0, 0);

        // Base speed from Enemy
        float baseSpeed = enemy.Speed;

        // Add conveyor effect if present
        float conveyorSpeed = 0f;
        if (conveyorBelt != null)
        {
            conveyorSpeed = conveyorBelt.conveyorBeltSpeed / -30f;
        }

        // Combine both for final X movement
        float totalSpeedX = direction.x * baseSpeed + conveyorSpeed;

        transform.position += new Vector3(totalSpeedX, 0, 0) * Time.deltaTime;
    }

    public void FlipSprite()
    {
        if (player == null) return;
        if (player.transform.position.x < transform.position.x)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
    }
}
