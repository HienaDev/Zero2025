using UnityEngine;

public class LaserHorizontal : ProjectileEffect
{

    [SerializeField] private LaserDamage laserHorizontalPrefab;

    public override void CallEffect(Enemy Enemy)
    {
        LaserDamage laser = Instantiate(laserHorizontalPrefab, Enemy.transform.position, Quaternion.identity);

        laser.Initialize(100f);
    }

}
