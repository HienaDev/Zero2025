using UnityEngine;

public class AddProjectileEffect : MonoBehaviour
{

    [SerializeField] private ProjectileEffect effectToAdd;
    public ProjectileEffect EffectToAdd => effectToAdd;

    private PlayerStats playerStats;

    private void Start()
    {
        playerStats = FindAnyObjectByType<PlayerStats>();
    }

    public void AddEffect()
    {
        if (playerStats != null && effectToAdd != null)
        {
            playerStats.AddProjectileEffect(effectToAdd);
        }
    }
}
