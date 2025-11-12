using TMPro;
using UnityEngine;
using NaughtyAttributes;
using System;
using UnityEngine.UI;   

public class AddBaseStatModifier : MonoBehaviour
{
    [SerializeField] private Sprite icon;
    [SerializeField] private Image iconUI;

    [SerializeField] private AudioClip[] sounds;
    public enum StatType
    {
        MaxSpeed,
        JumpForce,
        Damage,
        ProjectileSpeed,
        ShootRate,
        ShootRateBaseMultiply,
        ProjectilePierce,
        ProjectileCount,
        SelfKnockbackForce,
        Heal,
        BaseDamage,
        DecreaseBaseShootRate,
    }

    [Serializable]
    public struct StatModifier
    {
        public StatType statType;
        public float modifierValue;
        public StatModifier(StatType statType, float modifierValue)
        {
            this.statType = statType;
            this.modifierValue = modifierValue;
        }
    }

    [SerializeField] private bool singleStatIncrease = true;

    private PlayerStats playerStats;

    [ShowIf("singleStatIncrease")]
    [SerializeField] private StatType statToModify;

    [ShowIf("singleStatIncrease")]
    [SerializeField] private Vector2 modifierRange = new Vector2(0.05f, 0.25f);
    float modifierValue = 0f;

    private TextMeshProUGUI statText;

    [HideIf("singleStatIncrease")]
    [SerializeField] private StatModifier[] statModifiers;

    private void Start()
    {
        playerStats = FindAnyObjectByType<PlayerStats>();
        statText = GetComponentInChildren<TextMeshProUGUI>();

        iconUI.sprite = icon;

        modifierValue = UnityEngine.Random.Range(modifierRange.x, modifierRange.y);

        if (singleStatIncrease)
            switch (statToModify)
            {
                case StatType.MaxSpeed:
                    statText.text = "Speed increase: " + (modifierValue * 100).ToString("F0") + "%";
                    break;
                case StatType.JumpForce:
                    statText.text = "Jump increase: " + (modifierValue * 100).ToString("F0") + "%";
                    break;
                case StatType.Damage:
                    statText.text = "Damage increase: " + (modifierValue * 100).ToString("F0") + "%";
                    break;
                case StatType.ProjectileSpeed:
                    statText.text = "Projectile Speed increase: " + (modifierValue * 100).ToString("F0") + "%";
                    break;
                case StatType.ShootRate:
                    statText.text = "Shooting Rate increase: " + (modifierValue * 100).ToString("F0") + "%";
                    break;
                case StatType.ProjectilePierce:
                    statText.text = "Projectile Pierce increase: " + (modifierValue * 100).ToString("F0") + "%";
                    break;
                case StatType.ProjectileCount:
                    statText.text = "Projectile Count increase: " + (modifierValue * 100).ToString("F0") + "%";
                    break;
                case StatType.SelfKnockbackForce:
                    statText.text = "Self Knockback increase: " + (modifierValue * 100).ToString("F0") + "%";
                    break;
                case StatType.Heal:
                    statText.text = "Heal 1 Heart";
                    break;
            }

    }

    public void AddStat()
    {

        if (sounds.Length > 0)
            AudioManager.Instance.Play(sounds[UnityEngine.Random.Range(0, sounds.Length)], loop: false, volume: 1f, pitch: UnityEngine.Random.Range(0.9f, 1.1f));


        if (singleStatIncrease)
            switch (statToModify)
            {
                case StatType.MaxSpeed:
                    playerStats.AddSpeedModifiers(modifierValue);
                    break;
                case StatType.JumpForce:
                    playerStats.AddJumpModifier(modifierValue);
                    break;
                case StatType.Damage:
                    playerStats.AddDamageModifier(modifierValue);
                    break;
                case StatType.ProjectileSpeed:
                    playerStats.AddProjectileSpeedModifier(modifierValue);
                    break;
                case StatType.ShootRate:
                    playerStats.AddShootRateModifier(modifierValue);
                    break;
                case StatType.ShootRateBaseMultiply:
                    playerStats.MultiplyBaseShootRate(modifierValue);
                    break;
                case StatType.ProjectilePierce:
                    playerStats.AddPierceModifier((int)modifierValue);
                    break;
                case StatType.ProjectileCount:
                    playerStats.AddNumberOfProjectilesModifier((int)modifierValue);
                    break;
                case StatType.SelfKnockbackForce:
                    playerStats.IncreaseBaseSelfKnockbackForce(modifierValue);
                    break;
                case StatType.Heal:
                    playerStats.Heal(1);
                    break;
            }
        else
        {
            foreach (StatModifier statModifier in statModifiers)
            {
                switch (statModifier.statType)
                {
                    case StatType.MaxSpeed:
                        playerStats.AddSpeedModifiers(statModifier.modifierValue);
                        break;
                    case StatType.JumpForce:
                        playerStats.AddJumpModifier(statModifier.modifierValue);
                        break;
                    case StatType.Damage:
                        playerStats.AddDamageModifier(statModifier.modifierValue);
                        break;
                    case StatType.BaseDamage:
                        playerStats.IncreaseBaseDamage(statModifier.modifierValue);
                        break;
                    case StatType.ProjectileSpeed:
                        playerStats.AddProjectileSpeedModifier(statModifier.modifierValue);
                        break;
                    case StatType.ShootRate:
                        playerStats.AddShootRateModifier(statModifier.modifierValue);
                        break;
                    case StatType.DecreaseBaseShootRate:
                        playerStats.DecreaseBaseShootRate(statModifier.modifierValue);
                        break;
                    case StatType.ShootRateBaseMultiply:
                        playerStats.MultiplyBaseShootRate(statModifier.modifierValue);
                        break;
                    case StatType.ProjectilePierce:
                        playerStats.AddPierceModifier((int)statModifier.modifierValue);
                        break;
                    case StatType.ProjectileCount:
                        playerStats.AddNumberOfProjectilesModifier((int)statModifier.modifierValue);
                        break;
                    case StatType.SelfKnockbackForce:
                        playerStats.IncreaseBaseSelfKnockbackForce(statModifier.modifierValue);
                        break;
                }
            }
        }
    }
}
