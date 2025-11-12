using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public class Projectile : MonoBehaviour
{

    private float speed = 0f;
    private List<float> speedModifiers = new List<float>();
    public float Speed => speed;


    private float damage = 20f;
    private List<float> damageModifiers = new List<float>();
    public float Damage => damage;

    private int pierceCount = 0;
    private List<int> pierceModifiers = new List<int>();

    [SerializeField] private List<ProjectileEffect> effects = new List<ProjectileEffect>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    IEnumerator Start()
    {
        yield return new WaitForSeconds(10f);

        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Initialize(float speed, float damage, int pierceCount)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = transform.right * speed;

        this.speed = speed;

        this.damage = damage;

        this.pierceCount = pierceCount;
    }

    public void AddEffect(ProjectileEffect effect) => effects.Add(effect);

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemy = collision.GetComponentInParent<Enemy>();
        if (enemy != null)
        {
            

            foreach (var effect in effects)
            {
                effect.CallEffect(enemy);
            }

            enemy.TakeDamage(damage);

            pierceCount--;

            if(pierceCount < 0)
                Destroy(gameObject);
        }
    }
}
