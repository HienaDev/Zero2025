using DG.Tweening;
using UnityEngine;

public class ClusterShot : ProjectileEffect
{
    private void Start()
    {
        effectChance = 0.5f; // 30% chance to trigger on hit
    }

    public override void CallEffect(Enemy Enemy)
    {
        bool shouldApplyEffect = Random.value <= effectChance;
        if (!shouldApplyEffect)
            return;

        Vector3 enemyPosition = Enemy.transform.position;

        DOVirtual.DelayedCall(Time.deltaTime * 5, () =>
        {
            // After waiting one frame, check if enemy was destroyed
            if (Enemy.isDead)
            {
                PlayerShoot playerShoot = FindAnyObjectByType<PlayerShoot>();
                if (playerShoot != null)
                {
                    Debug.Log("Cluster Shot triggered via DOTween! Enemy position: " + enemyPosition);
                    playerShoot.ClusterShot(enemyPosition, 5);
                    // Debug.Log("Cluster Shot triggered via DOTween!");
                }
            }
        });

    }
}
