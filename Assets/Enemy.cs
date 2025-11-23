using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System.Collections;
using System.Linq;


public class Enemy : MonoBehaviour
{

    [SerializeField] public bool damageable = true;
    [SerializeField] public bool executable = true;

    [SerializeField] public float health = 100f;
    [SerializeField] private SpriteRenderer[] spriteRenderers;
    private Sequence sequence;

    [SerializeField] public float speed = 10f;
    public float Speed => speed;
    public float originalSpeed;

    private Tween iceTween;
    private Tween poisonTween;
    private Tween burnTween;

    public List<Status> currentStatusEffects = new List<Status>();

    [SerializeField] private GameObject deathEffectPrefab;

    [SerializeField] private float destroyDelay = 0.7f;

    [SerializeField] private AudioClip[] hitSounds;
    [SerializeField] private AudioClip[] deathSounds;

    private Vector3 originalScale;
    public bool isDead => health <= 0f;


    [Header("Bounce Settings")]
    [SerializeField] private float scaleMultiplier = 0.2f;
    [SerializeField] private float punchDuration = 0.3f;
    [SerializeField] private int vibrato = 10;
    [SerializeField] private float elasticity = 10f;

    private Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();

        originalScale = transform.localScale;

        originalSpeed = speed;
        if (spriteRenderers == null || spriteRenderers.Length == 0)
            spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        spriteRenderers = spriteRenderers
        .Where(sr => !sr.GetComponent<Tag_Burn>() && !sr.GetComponent<Tag_Poison>() && !sr.GetComponent<TAG_Ice>() && !sr.GetComponent<TAG_Indicator>())
        .ToArray();
    }

    // Update is called once per frame
    void Update()
    {

        animator.enabled = !currentStatusEffects.Contains(Status.Frozen);
    }



    public void TakeDamage(float damage, Color damageColor = default)
    {

        if (!damageable || isDead)
            return;

        if (damageColor == default)
        {
            damageColor = Color.white;
        }

        health -= damage;

        if (health < 0)
            health = 0;

        if (health <= 0)
        {
            sequence?.Kill();
            KillEnemy();
        }
        {
            if (hitSounds.Length > 0)
                AudioManager.Instance.Play(hitSounds[Random.Range(0, hitSounds.Length)], loop: false, volume: 2f, pitch: Random.Range(0.9f, 1.1f));



            // Optional: Add feedback for taking damage (e.g., flash red) using do tween
            sequence?.Kill();

            //transform.localScale = originalScale;

            sequence = DOTween.Sequence();
            sequence.AppendCallback(() => transform.localScale = originalScale);
            transform.DOPunchScale(originalScale * scaleMultiplier, punchDuration, vibrato, elasticity); // exaggerated
            sequence.AppendCallback(() => speed = 0f);
            foreach (var sr in spriteRenderers)
            {
                sequence.Join(sr.DOColor(damageColor, 0.05f).SetEase(Ease.OutQuart));
            }

            foreach (var sr in spriteRenderers)
            {
                sequence.Join(sr.DOColor(Color.white, 0.05f).SetEase(Ease.InQuart));
            }


            if (currentStatusEffects.Contains(Status.Frozen))
            {
                speed = 0f;
                // If frozen, take extra damage or some special effect
                // For simplicity, we just return here
                return;
            }

            sequence.AppendCallback(() => speed = originalSpeed * 0.85f);
            sequence.AppendInterval(0.04f);
            sequence.AppendCallback(() => speed = originalSpeed);
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
                StartCoroutine(ApplyFrozenEffect(duration));
                break;
            case Status.Poison:
                // Apply poison effect
                StartCoroutine(ApplyPoisonEffect(duration));
                break;
            case Status.Burn:
                // Apply burn effect
                StartCoroutine(ApplyBurnEffect(duration));
                break;
            default:
                break;
        }
    }

    private System.Collections.IEnumerator ApplyBurnEffect(float duration)
    {
        Debug.Log("Burn");
        burnTween?.Kill();

        SpriteRenderer burn = gameObject.GetComponentInChildren<Tag_Burn>().GetComponent<SpriteRenderer>();

        Debug.Log("Burn Sprite: " + burn);

        currentStatusEffects.Add(Status.Burn);

        burn.color = new Color(1f, 0.64f, 0f);

        Vector3 burnOriginalScale = burn.transform.localScale;
        burn.transform.localScale = Vector3.zero;
        burn.transform.DOScale(burnOriginalScale, 0.1f).SetEase(Ease.OutBack);

        burnTween = burn.DOFade(0f, duration).SetEase(Ease.InQuart).OnComplete(() =>
        {
            burn.color = Color.clear;
            currentStatusEffects.Remove(Status.Poison);
        });

        float elapsed = 0f;
        float damagePerSecond = 10f; // Example damage per second

        while (elapsed < duration)
        {
            // Apply damage once per second
            TakeDamage(damagePerSecond / 2, new Color(1f, 0.64f, 0f));
            elapsed += 0.5f;
            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator ApplyFrozenEffect(float duration)
    {
        // Example: Reduce speed to 0
        // Assuming there's a movement component to modify

       


        iceTween?.Kill();



        SpriteRenderer ice = gameObject.GetComponentInChildren<TAG_Ice>().GetComponent<SpriteRenderer>();

        speed = 0f;
        ice.color = Color.white;
        currentStatusEffects.Add(Status.Frozen);

        Vector3 originalIceScale = ice.transform.localScale;

        // Use tween instead to scale the ice very fast in 0.1 seconds and then reduce the transparency over the duration
        ice.transform.localScale = Vector3.zero;
        ice.transform.DOScale(originalIceScale, 0.1f).SetEase(Ease.OutBack);

        iceTween = ice.DOFade(0f, duration).SetEase(Ease.InQuart).OnComplete(() =>
        {
            speed = originalSpeed;
            ice.color = Color.clear;
            currentStatusEffects.Remove(Status.Frozen);
        });
        yield return new WaitForEndOfFrame(); // Wait a frame to ensure we dont shoot icycles in the same frame we freeze an enemy
    }

    private System.Collections.IEnumerator ApplyPoisonEffect(float duration)
    {
        poisonTween?.Kill();

        SpriteRenderer poison = gameObject.GetComponentInChildren<Tag_Poison>().GetComponent<SpriteRenderer>();

        currentStatusEffects.Add(Status.Poison);

        poison.color = Color.white;

        Vector3 iceOriginalScale = poison.transform.localScale;
        poison.transform.localScale = Vector3.zero;
        poison.transform.DOScale(iceOriginalScale, 0.1f).SetEase(Ease.OutBack);

        poisonTween = poison.DOFade(0f, duration).SetEase(Ease.InQuart).OnComplete(() =>
        {
            poison.color = Color.clear;
            currentStatusEffects.Remove(Status.Poison);
        });

        float elapsed = 0f;
        float damagePerSecond = 10f; // Example damage per second

        while (elapsed < duration)
        {
            // Apply damage once per second
            TakeDamage(damagePerSecond, new Color(1.0f, 0.75f, 0.8f, 1.0f));
            elapsed += 1f;
            yield return new WaitForSeconds(1f);
        }
    }


    public void KillEnemy(float delay = -1f, bool execute = false)
    {

        if (execute && !executable)
        { return; }


        Animator animator = gameObject.GetComponent<Animator>();

        if (animator != null)
            animator.SetTrigger("Death");
        else
            delay = 0f;

        if (deathSounds.Length > 0)
            AudioManager.Instance.Play(deathSounds[Random.Range(0, deathSounds.Length)], loop: false, volume: 0.2f, pitch: Random.Range(0.9f, 1.1f));


        if (delay == -1)
            delay = destroyDelay;


        speed = 0f;
        originalSpeed = 0f;
        Collider2D[] colliders = GetComponentsInChildren<Collider2D>();

        foreach (var collider in colliders)
        {
            collider.enabled = false;
        }

        DOVirtual.DelayedCall(destroyDelay * 2 / 3, () =>
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);

        });

        Destroy(gameObject, destroyDelay);
    }
}
