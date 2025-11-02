using UnityEngine;
using DG.Tweening;

public class SpecialMovement : MonoBehaviour
{
    private Enemy enemy;
    private PlayerController player;

    private SpriteRenderer spriteRenderer;

    [SerializeField] private float offSetY = 0.33f;
    private bool attacking = false;

    [SerializeField] private float dashDelay = 0.5f;
    [SerializeField] private float dashDistance = 30f;
    [SerializeField] private float dashSpeed = 50f;
    [SerializeField] private float dashCooldown = 3f;
    [SerializeField] private int flashCount = 5;
    private float justAttacked;

    [SerializeField] private SpriteRenderer indicator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FindAnyObjectByType<PlayerController>();
        enemy = GetComponent<Enemy>();

        spriteRenderer = GetComponent<SpriteRenderer>();

        justAttacked = -dashCooldown; // so it can attack immediately
    }

    // Update is called once per frame
    void Update()
    {
        if (attacking)
            return;

        MoveTowardsPlayer();
        FlipSprite();
        CheckHeightForAttack();
    }

    private void MoveTowardsPlayer()
    {
        if (player == null) return;
        Vector3 direction = (player.transform.position - transform.position).normalized;
        direction = new Vector3(direction.x * 0.33f, direction.y, 0); 
        transform.position += direction * enemy.Speed * Time.deltaTime;
    }

    private void FlipSprite()
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

    private void CheckHeightForAttack()
    {
        if (player == null) return;

        float heightDifference = Mathf.Abs(player.transform.position.y - transform.position.y);
        Debug.Log(heightDifference);

        if (heightDifference < offSetY && justAttacked + dashCooldown < Time.time)
        {
            DashAttack();
        }
    }

    private void DashAttack()
    {
        if (player == null) return;

        attacking = true;

        Vector3 direction = (player.transform.position - transform.position).normalized;
        direction = new Vector3(direction.x, 0, 0).normalized; // only dash in x direction
        Vector3 targetPosition = transform.position + direction * dashDistance;

        // Reset indicator alpha to fully visible (1)
        Color startColor = indicator.color;
        startColor.a = 1f;
        indicator.color = startColor;

        // Create the flashing tween (fade in/out repeatedly)
        Tween flashTween = indicator.DOFade(0.2f, dashDelay / (flashCount * 2))
            .SetLoops(flashCount * 2, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);

        // Create the main sequence
        Sequence dashSequence = DOTween.Sequence();

        dashSequence
            .Append(flashTween) // flash during delay
            .Append(transform.DOMove(targetPosition, dashDistance / dashSpeed)) // then dash
            .Join(indicator.DOFade(0f, 0.2f)) // fade indicator out during dash
            .OnComplete(() =>
            {
                justAttacked = Time.time;
                attacking = false;
            });

    }
}
