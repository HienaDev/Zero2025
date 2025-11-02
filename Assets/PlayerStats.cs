using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class PlayerStats : MonoBehaviour
{

    [SerializeField] private Animator animator;
    [SerializeField] private Collider2D col;
    [SerializeField] private SpriteRenderer gunSprite;
    [SerializeField] private GameObject guySoul;

    [SerializeField] private GameObject soulUI;

    [SerializeField] private float shakeDuration = 0.3f;
    [SerializeField] private float shakeMagnitude = 0.5f;


    [SerializeField] private GameObject deathScreen;
    [Header("UI Elements")]

    public Image image;
    public GameObject text1;
    public GameObject text2;

    [Header("Player Stats")]
    // Player Health
    [SerializeField] private int maxHealth = 3;
    private int currentHealth;
    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);
        soulUI.transform.localScale = Vector3.one * currentHealth / maxHealth;
        

        Debug.Log($"Player took {damage} damage. Current health: {currentHealth}/{maxHealth}");
        if (currentHealth <= 0)
        {
            Debug.Log("Player has died.");
            animator.SetTrigger("Death");  
            GetComponent<PlayerController>().enabled = false;
            GetComponent<PlayerShoot>().enabled = false;
            gunSprite.gameObject.SetActive(false);
            DOVirtual.DelayedCall(1f, () =>
            {
                guySoul.SetActive(true);
                Debug.Log("Target enabled!");
                deathScreen.SetActive(true);
                text1.SetActive(true);
                text2.SetActive(true);
                image.DOFade(0.8f, 1f).SetEase(Ease.InOutCirc);

            });
            //col.enabled = false;
            // Implement player death logic here
            
        }
        else
        {
            FindAnyObjectByType<CameraShake>().Shake(shakeDuration, shakeMagnitude);
        }
    }



    public bool IsAlive() => currentHealth > 0;
    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        Debug.Log($"Player healed {amount}. Current health: {currentHealth}/{maxHealth}");
    }


    // Player Speed
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float acceleration = 10f;
    private List<float> speedModifiers = new List<float>();
    public float MaxSpeed => maxSpeed * speedModifiers.Sum();
    public float Acceleration => acceleration * speedModifiers.Sum();
    public void AddSpeedModifiers(float modifier) => speedModifiers.Add(modifier);
    public void IncreaseBaseMaxSpeed(float value)
    {
        maxSpeed += value;
        acceleration += value * 20;
    }

    // Jump
    [SerializeField] private float jumpForce = 5f;
    private List<float> jumpModifiers = new List<float>();
    public float JumpForce => jumpForce * jumpModifiers.Sum();
    public void AddJumpModifier(float modifier) => jumpModifiers.Add(modifier);
    public void IncreaseBaseJumpForce(float value)
    {
        jumpForce += value;
    }

    // Jump Number
    [SerializeField] private int jumpNumber = 1;
    private List<int> jumpNumberModifiers = new List<int>();
    public int JumpNumber => jumpNumber + jumpNumberModifiers.Sum();
    public void AddJumpNumberModifier(int modifier) => jumpNumberModifiers.Add(modifier);

    [Header("Projectile Stats")]
    // Damage
    [SerializeField] private float damage = 20f;
    private List<float> damageModifiers = new List<float>();
    public float Damage => damage * damageModifiers.Sum();
    public void AddDamageModifier(float modifier) => damageModifiers.Add(modifier);
    public void IncreaseBaseDamage(float value)
    {
        damage += value;
        damage = Mathf.Max(5f, damage);
    }

    // Projectile Speed
    [SerializeField] private float projectileSpeed = 0f;
    private List<float> projectileSpeedModifiers = new List<float>();
    public float ProjectileSpeed => projectileSpeed * projectileSpeedModifiers.Sum();
    public void AddProjectileSpeedModifier(float modifier) => projectileSpeedModifiers.Add(modifier);
    public void IncreaseBaseProjectileSpeed(float value)
    {
        projectileSpeed += value;
    }

    // Shooting Rate
    [SerializeField] private float shootRate = 0.5f;
    private List<float> shootRateModifiers = new List<float>();
    public float ShootRate => shootRate / shootRateModifiers.Sum();
    public void AddShootRateModifier(float modifier) => shootRateModifiers.Add(modifier);
    public void DecreaseBaseShootRate(float value)
    {
        shootRate = Mathf.Max(0.05f, shootRate - value);
    }
    public void MultiplyBaseShootRate(float value)
    {
        shootRate *= value;
    }

    // Number of Projectiles
    [SerializeField] private int numberOfProjectiles = 1;
    private List<int> numberOfProjectilesModifiers = new List<int>();
    public int NumberOfProjectiles => numberOfProjectiles + numberOfProjectilesModifiers.Sum();
    public void AddNumberOfProjectilesModifier(int modifier) => numberOfProjectilesModifiers.Add(modifier);
    [SerializeField] private float spreadAngle = 15f;
    public float SpreadAngle => spreadAngle;

    // Self knockback
    [SerializeField] private float selfKnockbackForce = 0f;
    public float SelfKnockbackForce => selfKnockbackForce;
    public void IncreaseBaseSelfKnockbackForce(float value)
    {
        selfKnockbackForce += value;
    }


    // Pierce Count
    [SerializeField] private int pierceCount = 0;
    private List<int> pierceModifiers = new List<int>();
    public int PierceCount => pierceCount + pierceModifiers.Sum();
    public void AddPierceModifier(int modifier) => pierceModifiers.Add(modifier);

    // Projectile Effects
    [SerializeField] private List<ProjectileEffect> projectileEffects;
    public List<ProjectileEffect> ProjectileEffects => projectileEffects;
    public void AddProjectileEffect(ProjectileEffect effect) => projectileEffects.Add(effect);

    private void Start()
    {
        AddSpeedModifiers(1.0f);
        AddJumpModifier(1.0f);
        AddProjectileSpeedModifier(1.0f);
        AddShootRateModifier(1.0f);
        AddDamageModifier(1.0f);
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            damage *= 50f;
        }
    }
}
