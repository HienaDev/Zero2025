using DG.Tweening;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{

    private PlayerStats playerStats;
    private Sequence sequence;
    [SerializeField]private SpriteRenderer spriteRenderer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerStats = GetComponent<PlayerStats>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy != null)
        {


            PlayerTakeDamage();


            enemy.KillEnemy();
        }
    }

    public void PlayerTakeDamage()
    {
        playerStats.TakeDamage(1);

        sequence?.Kill();
        sequence = DOTween.Sequence();
        sequence.Append(spriteRenderer.DOColor(Color.red, 0.05f).SetEase(Ease.OutQuart));
        sequence.Append(spriteRenderer.DOColor(Color.white, 0.05f).SetEase(Ease.InQuart));

        if (!playerStats.IsAlive())
        {
            // Handle player death (e.g., trigger game over sequence)
            Debug.Log("Player has died. Game Over.");
        }
    }
}
