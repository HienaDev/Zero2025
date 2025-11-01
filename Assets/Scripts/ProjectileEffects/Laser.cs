using UnityEngine;

public class Laser : ProjectileEffect
{

    [SerializeField] private LaserDamage laserHorizontalPrefab;

    private float damageMultiplier = 0.5f; // Percentage of projectile damage

    private void Start()
    {
        effectChance = 0.2f; // 30% chance to trigger on hit
    }

    public override void CallEffect(Enemy Enemy)
    {
        LaserDamage laser = Instantiate(laserHorizontalPrefab, Enemy.transform.position, Quaternion.identity);

        laser.Initialize(projectile.Damage * damageMultiplier);
    }

    public override void LevelUp()
    {
        base.LevelUp();
        damageMultiplier += 0.25f; // Increase damage multiplier by 25% each level
        effectChance += 0.15f; // Increase chance by 15% each level
    }
}

