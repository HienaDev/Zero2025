using System.Drawing;
using UnityEngine;

public class LaserDamage : MonoBehaviour
{
    private float damage = 20f;


    public void Initialize(float damage)
    {
        this.damage = damage;

        ApplyDamage();  
    }

    public void ApplyDamage()
    {
        Vector2 size = new Vector2(transform.localScale.x, transform.localScale.y);
        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, size, 0);

        foreach (var hit in hits)
        {
            if (hit != null)
            {
                Enemy enemy = hit.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                }
            }
        }

    }
}
