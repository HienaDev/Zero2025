using UnityEngine;

public class Poison : ProjectileEffect
{
    [SerializeField] private float effectChanceOverride = 0.2f;
    [SerializeField] private float poisonDuration = 3.0f;


    private void Start()
    {
        effectChance = effectChanceOverride; // 30% chance to trigger on hit
    }

    public override void CallEffect(Enemy Enemy)
    {
        bool shouldApplyEffect = Random.value <= effectChance;

        if (!shouldApplyEffect)
            return;

        //Debug.Log("Freeze proc!");
        Enemy.ApplyStatusEffect(Status.Poison, poisonDuration);
    }

    public override void LevelUp()
    {
        base.LevelUp();
        effectChance += 0.15f; // Increase chance by 15% each level
    }
}
