using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class AddProjectileEffect : MonoBehaviour
{

    [SerializeField] private ProjectileEffect effectToAdd;

    public ProjectileEffect[] dependsOnThisEffect;

    public ProjectileEffect EffectToAdd => effectToAdd;

    [SerializeField] private AudioClip[] sounds;


    private PlayerStats playerStats;

    [SerializeField] public Sprite icon;
    [SerializeField] private Image iconUI;

    [SerializeField] private UnityEvent onEffectAdded;


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
        AudioManager.Instance.Play(sounds[UnityEngine.Random.Range(0, sounds.Length)], loop: false, volume: 1f, pitch: UnityEngine.Random.Range(0.9f, 1.1f));

        onEffectAdded.Invoke();

        if (playerStats != null && effectToAdd != null)
        {
            FindAnyObjectByType<WaveManager>().AddRelicDisplay(icon);
            playerStats.AddProjectileEffect(effectToAdd);
        }
    }
}
