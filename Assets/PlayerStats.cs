using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Player Stats")]
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
        shootRate = Mathf.Max(0.1f, shootRate - value);
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
    }
}
