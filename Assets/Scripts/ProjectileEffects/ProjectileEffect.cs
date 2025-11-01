using UnityEngine;

public class ProjectileEffect : MonoBehaviour
{
    protected float effectChance = 1.0f; // Default to 100% chance
    protected Projectile projectile;

    private int maxLevel = 3;
    private int currentLevel = 1;

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

    public virtual void LevelUp()
    {
        if (currentLevel < maxLevel)
        {
            currentLevel++;
            // Implement level-up logic specific to the effect
            Debug.Log($"Projectile effect leveled up to level {currentLevel}");
        }
    }
}
