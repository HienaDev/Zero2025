using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class ChainLightning : ProjectileEffect
{

    [SerializeField] private ChainProjectile lightningPrefab;

    private float damageMultiplier = 1f; // Percentage of projectile damage

    [SerializeField] private float ligthtningRange = 5f;
    [SerializeField] private int numberOfEenemies = 3;
    [SerializeField] private float speed = 10f;

    private void Start()
    {
        effectChance = 0.2f; // 20% chance to trigger on hit
    }


    public override void CallEffect(Enemy Enemy)
    {
        bool shouldApplyEffect = Random.value <= effectChance;
        if (!shouldApplyEffect)
            return;

        List<Enemy> enemiesToBeHit = new List<Enemy>();

        Enemy currentEnemy = Enemy;
        enemiesToBeHit.Add(currentEnemy);

        for (int i = 0; i < numberOfEenemies - 1; i++)
        {
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(currentEnemy.transform.position, ligthtningRange);

            // Distance of all objects to current enemy
            float closestDistance = Mathf.Infinity;
            Enemy closestEnemy = null;
            foreach (var hitCollider in hitColliders)
            {
                Enemy enemy = hitCollider.GetComponentInParent<Enemy>();
                if (enemy != null && enemy != currentEnemy && !enemiesToBeHit.Contains(enemy))
                {
                    if(Random.value <= 0.25f)
                    {
                        // 50% chance to chain to this enemy
                        closestEnemy = enemy;
                        continue;
                    }

                    float distance = Vector2.Distance(currentEnemy.transform.position, enemy.transform.position);
                    if (distance < closestDistance )
                    {
                        closestDistance = distance;
                        closestEnemy = enemy;
                    }
                }
            }

            if (closestEnemy != null)
            {
                enemiesToBeHit.Add(closestEnemy);
                currentEnemy = closestEnemy;
            }
            else
            {
                break; // No more enemies to chain to
            }

        }

        ChainProjectile lightning = Instantiate(lightningPrefab, Enemy.transform.position, Quaternion.identity);

        lightning.TravelThroughEnemies(enemiesToBeHit, speed, projectile.Damage * damageMultiplier);

        Debug.Log("Lightning hit " + enemiesToBeHit.Count + " enemies.");
    }

    public override void LevelUp()
    {
        base.LevelUp();
        damageMultiplier += 0.25f; // Increase damage multiplier by 25% each level
        effectChance += 0.15f; // Increase chance by 15% each level
        numberOfEenemies *= 2;
    }

}
