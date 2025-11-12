using DG.Tweening;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{

    private PlayerStats playerStats;
    private Sequence sequence;
    [SerializeField]private SpriteRenderer spriteRenderer;

    [SerializeField] private float gracePeriod = 1f;
    [SerializeField] private float blinkInterval = 0.1f;
    private float justGotDamaged;

    private CameraTweenToPlayer cameraTween;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        justGotDamaged = -gracePeriod;
        playerStats = GetComponent<PlayerStats>();
        cameraTween = FindAnyObjectByType<CameraTweenToPlayer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemy = collision.GetComponentInParent<Enemy>();
        if (enemy != null)
        {

            if(justGotDamaged + gracePeriod > Time.time)
            {
                return;
            }
            PlayerTakeDamage();

            if (playerStats.IsAlive())
                enemy.KillEnemy(execute: true);
        }

        EnemyProjectile enemyProjectile = collision.GetComponent<EnemyProjectile>();

        if (enemyProjectile != null)
        {
            if (justGotDamaged + gracePeriod > Time.time)
            {
                return;
            }
            PlayerTakeDamage();
            if (playerStats.IsAlive())
                Destroy(enemyProjectile.gameObject);
        }
    }

    public void PlayerTakeDamage()
    {
        if (!playerStats.IsAlive())
            return;

        playerStats.TakeDamage(1);
        justGotDamaged = Time.time;



        if (!playerStats.IsAlive())
        {
            // Handle player death (e.g., trigger game over sequence)
            Debug.Log("Player has died. Game Over.");
            cameraTween.TweenIn();
        }
        else
        {
            sequence?.Kill();
            sequence = DOTween.Sequence();
            sequence.Append(spriteRenderer.DOColor(Color.red, 0.05f).SetEase(Ease.OutQuart));
            sequence.Append(spriteRenderer.DOColor(Color.white, 0.05f).SetEase(Ease.InQuart));

            // Dotween that blinks transparency for grace period

            Sequence graceSequence = DOTween.Sequence();


            int blinkCount = Mathf.FloorToInt(gracePeriod / blinkInterval);
            for (int i = 0; i < blinkCount; i++)
            {
                graceSequence.Append(spriteRenderer.DOFade(0f, blinkInterval / 2));
                graceSequence.Append(spriteRenderer.DOFade(1f, blinkInterval / 2));
            }
            graceSequence.Play();

        }
    }
}
