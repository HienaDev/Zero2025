using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Projectile : MonoBehaviour
{

    private float speed = 0f;
    private List<float> speedModifiers = new List<float>();
    public float Speed => speed;
    public void AddSpeedModifier(float modifier) => speedModifiers.Add(modifier);


    private float damage = 20f;
    private List<float> damageModifiers = new List<float>();
    public float Damage => damage;
    public void AddDamageModifier(float modifier) => damageModifiers.Add(modifier);

    private int pierceCount = 0;
    private List<int> pierceModifiers = new List<int>();
    public void AddPierceModifier(int modifier) => pierceModifiers.Add(modifier);

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
        this.speed *= speedModifiers.Sum();
        this.damage = damage;
        this.damage *= damageModifiers.Sum();
        this.pierceCount += pierceModifiers.Sum();
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

            pierceCount--;

            if(pierceCount < 0)
                Destroy(gameObject);
        }
    }
}
