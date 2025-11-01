using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class PlayerShoot : MonoBehaviour
{

    private PlayerStats playerStats;
    private PlayerController playerController;

    [SerializeField] private Projectile projectilePrefab;

    [SerializeField] private GameObject cannonSprite;
    [SerializeField] private GameObject firePoint;

    private float justShot;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerStats = GetComponent<PlayerStats>();
        playerController = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {

        Aim();

        if (Input.GetButton("Fire1") && justShot + playerStats.ShootRate < Time.time)
        {
            Shoot();
            justShot = Time.time;
        }
    }

    private void Aim()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        cannonSprite.transform.eulerAngles = new Vector3(0, 0,
            Mathf.Atan2(mousePosition.y - transform.position.y, mousePosition.x - transform.position.x) * Mathf.Rad2Deg);
    }

    public void Shoot()
    {

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        playerController.ApplyExternalForce(-cannonSprite.transform.right * playerStats.SelfKnockbackForce);

        for (int i = 0; i < playerStats.NumberOfProjectiles; i++)
        {

            // Instantiate the projectile at the player's position
            Projectile projectile = Instantiate(projectilePrefab, firePoint.transform.position, Quaternion.identity);

            projectile.gameObject.transform.eulerAngles = new Vector3(0, 0,
                Mathf.Atan2(mousePosition.y - transform.position.y, mousePosition.x - transform.position.x) * Mathf.Rad2Deg + (i - (playerStats.NumberOfProjectiles - 1) / 2) * playerStats.SpreadAngle);

            projectile.Initialize(playerStats.ProjectileSpeed, playerStats.Damage, playerStats.PierceCount);

            // Apply effects to the projectile
            foreach (ProjectileEffect effect in playerStats.ProjectileEffects)
            {
                var tempEffect = projectile.gameObject.AddComponent(effect.GetType());
                projectile.AddEffect(tempEffect as ProjectileEffect);

                JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(effect), tempEffect);
            }
        }
    }
}
