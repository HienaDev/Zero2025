using UnityEngine;

public class Freeze : ProjectileEffect
{
    private void Start()
    {
        effectChance = 0.2f; // 30% chance to trigger on hit
    }

    public override void CallEffect(Enemy Enemy)
    {
        bool shouldApplyEffect = Random.value <= effectChance;

        if (!shouldApplyEffect)
            return;

        //Debug.Log("Freeze proc!");
        Enemy.ApplyStatusEffect(Status.Frozen, 2.0f); 
    }

    public override void LevelUp()
    {
        base.LevelUp();
        effectChance += 0.15f; // Increase chance by 15% each level
    }
}
