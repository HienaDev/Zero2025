using UnityEngine;
using UnityEngine.UI;

public class AddProjectileEffect : MonoBehaviour
{

    [SerializeField] private ProjectileEffect effectToAdd;
    public ProjectileEffect EffectToAdd => effectToAdd;

    private PlayerStats playerStats;

    [SerializeField] private Sprite icon;
    [SerializeField] private Image iconUI;
   

    private void Start()
    {
        playerStats = FindAnyObjectByType<PlayerStats>();

        if (iconUI != null && icon != null)
        {
            iconUI.sprite = icon;
        }
    }

    public void AddEffect()
    {
        if (playerStats != null && effectToAdd != null)
        {
            playerStats.AddProjectileEffect(effectToAdd);
        }
    }
}
