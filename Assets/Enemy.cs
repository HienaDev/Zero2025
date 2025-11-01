using UnityEngine;
using DG.Tweening;

public class Enemy : MonoBehaviour
{

    [SerializeField] private float health = 100f;
    private SpriteRenderer spriteRenderer;
    private Sequence sequence;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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

}
