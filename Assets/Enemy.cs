using UnityEngine;
using DG.Tweening;

public class Enemy : MonoBehaviour
{

    

    [SerializeField] private float health = 100f;
    private SpriteRenderer spriteRenderer;
    private Sequence sequence;

    [SerializeField] private float speed = 10f;
    private float originalSpeed;

    private Tween iceTween;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originalSpeed = speed;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            sequence?.Kill();
            Destroy(gameObject);
        }
        else
        {
            // Optional: Add feedback for taking damage (e.g., flash red) using do tween
            sequence?.Kill();

            sequence = DOTween.Sequence();
            sequence.Append(spriteRenderer.DOColor(Color.red, 0.05f).SetEase(Ease.OutQuart));
            sequence.Append(spriteRenderer.DOColor(Color.white, 0.05f).SetEase(Ease.InQuart));
        }
    }

    public void ApplyStatusEffect(Status status, float duration)
    {
        // Implement status effect application logic here
        // For example, if status is Frozen, reduce movement speed
        // If status is Poison, apply damage over time

        switch (status)
        {
            case Status.Frozen:
                // Apply frozen effect
                ApplyFrozenEffect(duration);
                break;
            case Status.Poison:
                // Apply poison effect
                StartCoroutine(ApplyPoisonEffect(duration));
                break;
            default:
                break;
        }
    }

    private void ApplyFrozenEffect(float duration)
    {
        // Example: Reduce speed to 0
        // Assuming there's a movement component to modify

        iceTween?.Kill();

        SpriteRenderer ice = gameObject.GetComponentInChildren<TAG_Ice>().GetComponent<SpriteRenderer>();

        speed = 0f;
        ice.color = Color.white;

        // Use tween instead to scale the ice very fast in 0.1 seconds and then reduce the transparency over the duration
        ice.transform.localScale = Vector3.zero;
        ice.transform.DOScale(Vector3.one, 0.1f).SetEase(Ease.OutBack);

        iceTween = ice.DOFade(0f, duration).SetEase(Ease.InQuart).OnComplete(() =>
        {
            speed = originalSpeed;
            ice.color = Color.clear;
        });
    }

    private System.Collections.IEnumerator ApplyPoisonEffect(float duration)
    {
        float elapsed = 0f;
        float damagePerSecond = 5f; // Example damage per second
        spriteRenderer.color = Color.green;
        while (elapsed < duration)
        {
            TakeDamage(damagePerSecond * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
        spriteRenderer.color = Color.white;

        yield return 0.1f;
        spriteRenderer.color = Color.white;
    }
}
