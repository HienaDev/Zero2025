using UnityEngine;
using DG.Tweening;

public class LaserDamage : MonoBehaviour
{
    private float damage = 20f;
    private SpriteRenderer spriteRenderer;

    [SerializeField] private bool horizontal = true;

    [SerializeField] private AudioClip[] sounds;

    public void Initialize(float damage, bool diagonal = false, Vector3 initialRotation = default)
    {
        this.damage = damage;
        spriteRenderer = GetComponent<SpriteRenderer>();

        if(diagonal)
        {
            transform.eulerAngles = initialRotation;
        }
        else
            transform.eulerAngles = new Vector3(0, 0, horizontal ? 0 : 90);

        ApplyDamage();  
    }

    public void ApplyDamage()
    {
        Vector2 size = new Vector2(transform.localScale.x, transform.localScale.y);
        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, size, transform.eulerAngles.z);

        AudioManager.Instance.Play(sounds[Random.Range(0, sounds.Length)], loop: false, volume: 0.5f, pitch: Random.Range(0.9f, 1.1f));


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
        sequence.Append(spriteRenderer.DOColor(Color.white, 0.2f).SetEase(Ease.InQuart));
        sequence.Append(spriteRenderer.DOFade(0f, 0.2f).SetEase(Ease.InQuart));
        if (!horizontal)
            sequence.Join(transform.DOScaleX(0f, 0.2f).SetEase(Ease.InQuart));
        else
            sequence.Join(transform.DOScaleY(0f, 0.2f).SetEase(Ease.InQuart));
        sequence.OnComplete(() => Destroy(gameObject));

    }
}
