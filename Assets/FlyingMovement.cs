using UnityEngine;

public class FlyingMovement : MonoBehaviour
{
    private Enemy enemy;
    private PlayerController player;

    private SpriteRenderer spriteRenderer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FindAnyObjectByType<PlayerController>();
        enemy = GetComponent<Enemy>();

        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        MoveTowardsPlayer();
        FlipSprite();
    }

    public void MoveTowardsPlayer()
    {
        if (player == null) return;
        Vector3 direction = (player.transform.position - transform.position).normalized;
        transform.position += direction * enemy.Speed * Time.deltaTime;
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
