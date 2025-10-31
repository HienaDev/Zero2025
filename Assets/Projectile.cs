using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class Projectile : MonoBehaviour
{

    private float speed = 0f;
    private float damage = 20f;

    private List<ProjectileEffect> effects = new List<ProjectileEffect>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Initialize(float speed, float damage)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = transform.right * speed;

        this.speed = speed;
        this.damage = damage;
    }

    public void AddEffect(ProjectileEffect effect) => effects.Add(effect);

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);

            foreach (var effect in effects)
            {
                effect.CallEffect(enemy);
            }

            Destroy(gameObject);
        }
    }
}
