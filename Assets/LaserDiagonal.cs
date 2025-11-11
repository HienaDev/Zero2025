using UnityEngine;

public class LaserDiagonal : ProjectileEffect
{

    [SerializeField] private LaserDamage laserHorizontalPrefab;

    private float damageMultiplier = 0.5f; // Percentage of projectile damage

    private void Start()
    {
        effectChance = 0.2f; // 30% chance to trigger on hit

    }

    public override void CallEffect(Enemy Enemy)
    {

        bool shouldApplyEffect = Random.value <= effectChance;

        if (!shouldApplyEffect)
            return;

        LaserDamage laser = Instantiate(laserHorizontalPrefab, Enemy.transform.position, Quaternion.identity);

        laser.Initialize(projectile.Damage * damageMultiplier, true, new Vector3(0f, 0f, 135f));

        laser = Instantiate(laserHorizontalPrefab, Enemy.transform.position, Quaternion.identity);

        laser.Initialize(projectile.Damage * damageMultiplier, true, new Vector3(0f, 0f, 45f));

    }

    public override void LevelUp()
    {
        base.LevelUp();
        damageMultiplier += 0.25f; // Increase damage multiplier by 25% each level
        effectChance += 0.15f; // Increase chance by 15% each level
    }
}

