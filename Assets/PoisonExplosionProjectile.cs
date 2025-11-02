using DG.Tweening;
using UnityEngine;

public class PoisonExplosionProjectile : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private float duration;

    [SerializeField] private AudioClip[] sounds;

    private void Start()
    {

        // Random rotation
        transform.Rotate(0f, 0f, Random.Range(0f, 360f));

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

        AudioManager.Instance.Play(sounds[Random.Range(0, sounds.Length)], loop: false, volume: 0.35f, pitch: Random.Range(1.1f, 1.3f));


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
        sequence.Append(spriteRenderer.DOColor(Color.white, 0.4f).SetEase(Ease.InQuart));
        sequence.Append(spriteRenderer.DOFade(0f, 0.4f).SetEase(Ease.InQuart));

        sequence.Join(spriteRenderer.DOColor(Color.clear, 0.4f).SetEase(Ease.InQuart));
        sequence.Join(transform.DOScale(0f, 0.4f).SetEase(Ease.InQuart));

        sequence.OnComplete(() => Destroy(gameObject));



    }
}
