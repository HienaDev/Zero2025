using TMPro;
using UnityEngine;

public class LevelUpRelic : MonoBehaviour
{

    [SerializeField] private ProjectileEffect effectToLvlUp;
    public ProjectileEffect EffectToLvlUp => effectToLvlUp;
    public void SetEffectToLvlUp(ProjectileEffect effect)
    {
        effectToLvlUp = effect;

        GetComponentInChildren<TextMeshProUGUI>().text = "Level Up " + effectToLvlUp.name;
    }

    private PlayerStats playerStats;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerStats = FindAnyObjectByType<PlayerStats>();
    }

    public void LevelUp()
    {
        effectToLvlUp.LevelUp();
    }
}
