using DG.Tweening;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class PoisonExplosionProjectile : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private float duration;

    private void Start()
    {
        Initialize(3f);
    }

    public void Initialize(float duration)
    {
        this.duration = duration;
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
                    enemy.ApplyStatusEffect(Status.Poison, duration);
                }
            }
        }

        // Laser sprite goes from original color to white and then fades out
        Sequence sequence = DOTween.Sequence();
        sequence.Append(spriteRenderer.DOColor(new Color(0.75f, 0.0f, 0.75f), 0.2f).SetEase(Ease.InQuart));
        sequence.Append(spriteRenderer.DOFade(0f, 0.2f).SetEase(Ease.InQuart));

        sequence.Join(spriteRenderer.DOColor(Color.clear, 0.2f).SetEase(Ease.InQuart));
        sequence.Join(transform.DOScale(0f, 0.2f).SetEase(Ease.InQuart));

        sequence.OnComplete(() => Destroy(gameObject));

    }
}
