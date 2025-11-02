using UnityEngine;

public class IcycleExplosion : ProjectileEffect
{
    [SerializeField] private Projectile icyclePrefab;
    [SerializeField] private int numberOfIcycles = 3;

    [SerializeField] private AudioClip[] sounds;

    private void Start()
    {
        effectChance = 0.5f; // 30% chance to trigger on hit
    }

    public override void CallEffect(Enemy Enemy)
    {
        bool shouldApplyEffect = Random.value <= effectChance;
        if (!shouldApplyEffect)
            return;
        if (Enemy.currentStatusEffects.Contains(Status.Frozen) == false)
            return;

        Enemy.KillEnemy(0f); // Instantly kill the enemy if frozen

        AudioManager.Instance.Play(sounds[Random.Range(0, sounds.Length)], loop: false, volume: 0.75f, pitch: Random.Range(0.9f, 1.1f));


        float initialOffset = Random.Range(0f, 360f); // Randomize starting angle for variety

        //Debug.Log("Icycle Explosion proc!");
        for (int i = 0; i < numberOfIcycles; i++)
        {
            // Rotation divides 360 by number of icycles to spread them evenly
            float angle = i * (360f / numberOfIcycles) + initialOffset;
            Quaternion rotation = Quaternion.Euler(0, 0, angle);
            Projectile icycle = Instantiate(icyclePrefab, Enemy.transform.position, Quaternion.identity);
            icycle.transform.rotation = rotation;
            icycle.Initialize(-projectile.Speed, projectile.Damage * 0.5f, 10); // Icycles deal 50% damage and have no pierce
        }

    }

    public override void LevelUp()
    {
        base.LevelUp();
        effectChance += 0.15f; // Increase chance by 15% each level
        numberOfIcycles += 2;
    }
}
