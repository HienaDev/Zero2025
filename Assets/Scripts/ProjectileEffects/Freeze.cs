using UnityEngine;

public class Freeze : ProjectileEffect
{
    [SerializeField] private float effectChanceOverride = 0.2f;
    [SerializeField] private float freezeDuration = 2.0f;

    [SerializeField] private AudioClip[] sounds;

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
        Enemy.ApplyStatusEffect(Status.Frozen, freezeDuration);
        AudioManager.Instance.Play(sounds[Random.Range(0, sounds.Length)],loop: false, volume: 0.35f, pitch: Random.Range(0.9f, 1.1f));
    }

    public override void LevelUp()
    {
        base.LevelUp();
        effectChance += 0.15f; // Increase chance by 15% each level
    }
}
