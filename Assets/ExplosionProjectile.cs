using DG.Tweening;
using UnityEngine;

public class ExplosionProjectile : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private float damage;

    private void Start()
    {
        Initialize(3f);
    }

    public void Initialize(float damage)
    {
        this.damage = damage;
        spriteRenderer = GetComponent<SpriteRenderer>();

        ApplyDamage();
    }

    public void ApplyDamage()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, transform.localScale.x, 0);

        foreach (var hit in hits)
        {
            if (hit != null)
            {
                Enemy enemy = hit.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                }
            }
        }

        // Laser sprite goes from original color to white and then fades out
        Sequence sequence = DOTween.Sequence();
        sequence.Append(spriteRenderer.DOColor(new Color(0.6f,0.2f, 0f, 0.5f), 0.2f).SetEase(Ease.InQuart));
        sequence.Append(spriteRenderer.DOFade(0f, 0.2f).SetEase(Ease.InQuart));

        sequence.Join(spriteRenderer.DOColor(Color.clear, 0.2f).SetEase(Ease.InQuart));
        sequence.Join(transform.DOScale(0f, 0.2f).SetEase(Ease.InQuart));

        sequence.OnComplete(() => Destroy(gameObject));

    }
}
