using UnityEngine;

public class PoisonExplosion : ProjectileEffect
{
    [SerializeField] private PoisonExplosionProjectile poisonPrefab;

    private void Start()
    {
        effectChance = 0.3f; // 30% chance to trigger on hit
    }

    public override void CallEffect(Enemy Enemy)
    {
        bool shouldApplyEffect = Random.value <= effectChance;
        if (!shouldApplyEffect)
            return;
        if (Enemy.currentStatusEffects.Contains(Status.Poison) == false)
            return;

        Enemy.KillEnemy(0f); // Instantly kill the enemy if frozen

        PoisonExplosionProjectile poisonBomb = Instantiate(poisonPrefab, Enemy.transform.position, Quaternion.identity);
        poisonBomb.Initialize(3f); // Poison explosion lasts for 3 seconds
    }

    public override void LevelUp()
    {
        base.LevelUp();
        effectChance += 0.15f; // Increase chance by 15% each level
    }
}
