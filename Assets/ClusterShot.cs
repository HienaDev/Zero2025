using UnityEngine;

public class ClusterShot : ProjectileEffect
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

        Enemy.KillEnemy(); // Instantly kill the enemy

        //Debug.Log("Cluster Shot proc!");
        PlayerShoot playerShoot = FindAnyObjectByType<PlayerShoot>();

        if (playerShoot != null)
        {
            playerShoot.ClusterShot(Enemy.transform, 5);
        }
    }
}
