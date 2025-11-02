using UnityEngine;

public class Explosion : ProjectileEffect
{
    [SerializeField] private ExplosionProjectile bombPrefab;

    private void Start()
    {
        effectChance = 0.15f; // 30% chance to trigger on hit
    }

    public override void CallEffect(Enemy Enemy)
    {
        bool shouldApplyEffect = Random.value <= effectChance;
        if (!shouldApplyEffect)
            return;

        ExplosionProjectile poisonBomb = Instantiate(bombPrefab, Enemy.transform.position, Quaternion.identity);
        poisonBomb.Initialize(50f); 
    }

    public override void LevelUp()
    {
        base.LevelUp();
        effectChance += 0.15f; // Increase chance by 15% each level
    }
}
