using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteFlasher : MonoBehaviour
{
    [Header("Flash Settings")]
    [SerializeField] private Color flashColor = Color.white;
    [SerializeField] private float flashDuration = 0.2f;
    [SerializeField] private Ease flashEase = Ease.InOutSine;

    private SpriteRenderer sr;
    private Color originalColor;
    private Tween flashTween;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
    }

    private void OnEnable()
    {
        // Reset color in case it was left mid-flash
        sr.color = originalColor;

        // Kill any previous tween
        flashTween?.Kill();

        // Start continuous flash loop
        flashTween = sr
            .DOColor(flashColor, flashDuration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(flashEase)
            .SetLink(gameObject); // ensures tween dies if object is destroyed
    }

    private void OnDisable()
    {
        // Stop the tween and reset color
        flashTween?.Kill();
        sr.color = originalColor;
    }
}
