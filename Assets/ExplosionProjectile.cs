using DG.Tweening;
using UnityEngine;

public class ExplosionProjectile : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private float damage;

    [SerializeField] private bool burn = false;

    [SerializeField] private AudioClip[] sounds;

    private void Start()
    {
        // Random rotation
        transform.Rotate(0f, 0f, Random.Range(0f, 360f));

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
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, transform.localScale.x);

        AudioManager.Instance.Play(sounds[Random.Range(0, sounds.Length)], loop: false, volume: 0.35f, pitch: Random.Range(0.7f, 0.9f));

        Debug.Log("Bobm Hits length: " + hits.Length);

        foreach (var hit in hits)
        {
            Debug.Log("Bomb Hit: " + hit.name);
            if (hit != null)
            {
                Enemy enemy = hit.GetComponent<Enemy>();
                if (enemy != null)
                {
                    if(burn)
                    {
                        // Apply burn effect for 3.1 seconds
                        Debug.Log(enemy.name + " burned!");
                        enemy.ApplyStatusEffect(Status.Burn, 3.1f);
                    }
                    

                    enemy.TakeDamage(damage);
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
