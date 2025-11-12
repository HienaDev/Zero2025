using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System.Collections;


public class Enemy : MonoBehaviour
{

    [SerializeField] public float health = 100f;
    private SpriteRenderer spriteRenderer;
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originalScale = transform.localScale;

        originalSpeed = speed;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public void TakeDamage(float damage, Color damageColor = default)
    {

        if(damageColor == default)
        {
            damageColor = Color.white;
        }

        health -= damage;
        if (health <= 0)
        {
            sequence?.Kill();
            KillEnemy();
        }
        else
        {
            AudioManager.Instance.Play(hitSounds[Random.Range(0, hitSounds.Length)], loop: false, volume: 2f, pitch: Random.Range(0.9f, 1.1f));


            if (currentStatusEffects.Contains(Status.Frozen))
            {
                speed = 0f;
                // If frozen, take extra damage or some special effect
                // For simplicity, we just return here
                return;
            }
            // Optional: Add feedback for taking damage (e.g., flash red) using do tween
            sequence?.Kill();

            transform.localScale = originalScale;

            sequence = DOTween.Sequence();
            transform.DOPunchScale(originalScale * 0.2f, 0.3f, 10, 10f); // exaggerated
            sequence.AppendCallback(() => speed = 0f);
            sequence.Append(spriteRenderer.DOColor(damageColor, 0.05f).SetEase(Ease.OutQuart));
            sequence.Append(spriteRenderer.DOColor(Color.white, 0.05f).SetEase(Ease.InQuart));

            

            sequence.AppendInterval(0.1f);
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

        Vector3 originalScale = burn.transform.localScale;
        //burn.transform.localScale = Vector3.zero;
        //burn.transform.DOScale(originalScale, 0.1f).SetEase(Ease.OutBack);

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


        yield return new WaitForEndOfFrame(); // Wait a frame to ensure we dont shoot icycles in the same frame we freeze an enemy

        iceTween?.Kill();

        

        SpriteRenderer ice = gameObject.GetComponentInChildren<TAG_Ice>().GetComponent<SpriteRenderer>();

        speed = 0f;
        ice.color = Color.white;
        currentStatusEffects.Add(Status.Frozen);

        // Use tween instead to scale the ice very fast in 0.1 seconds and then reduce the transparency over the duration
        ice.transform.localScale = Vector3.zero;
        ice.transform.DOScale(Vector3.one, 0.1f).SetEase(Ease.OutBack);

        iceTween = ice.DOFade(0f, duration).SetEase(Ease.InQuart).OnComplete(() =>
        {
            speed = originalSpeed;
            ice.color = Color.clear;
            currentStatusEffects.Remove(Status.Frozen);
        });
    }

    private System.Collections.IEnumerator ApplyPoisonEffect(float duration)
    {
        poisonTween?.Kill();

        SpriteRenderer poison = gameObject.GetComponentInChildren<Tag_Poison>().GetComponent<SpriteRenderer>();

        currentStatusEffects.Add(Status.Poison);

        poison.color = Color.white;

        Vector3 originalScale = poison.transform.localScale;
        poison.transform.localScale = Vector3.zero;
        poison.transform.DOScale(originalScale, 0.1f).SetEase(Ease.OutBack);

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
            TakeDamage(damagePerSecond, Color.magenta);
            elapsed += 1f;
            yield return new WaitForSeconds(1f);
        }
    }


    public void KillEnemy(float delay = -1f )
    {
        GetComponent<Animator>()?.SetTrigger("Death");

        AudioManager.Instance.Play(deathSounds[Random.Range(0, deathSounds.Length)], loop: false, volume: 0.2f, pitch: Random.Range(0.9f, 1.1f));


        if (delay == -1)
            delay = destroyDelay;


        speed = 0f;
        originalSpeed = 0f;
        GetComponent<Collider2D>().enabled = false;

        DOVirtual.DelayedCall(destroyDelay * 2/3, () =>
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);

        });

        Destroy(gameObject, destroyDelay);
    }
}
