using UnityEngine;

public class ProjectileEffect : MonoBehaviour
{
    protected virtual float effectChance => 1.0f; // Default to 100% chance
    protected Projectile projectile;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected void Awake()
    {
        projectile = GetComponent<Projectile>();
    }


    public virtual void CallEffect(Enemy Enemy)
    {
        // Placeholder for playing projectile effect (e.g., particle effect, sound)
        Debug.Log("Playing projectile effect");
    }
}
