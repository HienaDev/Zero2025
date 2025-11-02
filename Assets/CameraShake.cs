using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private Vector3 originalPos;
    private float shakeDuration;
    private float shakeMagnitude;
    private float totalDuration;
    private float dampingSpeed = 1f;
    private float seed;

    void OnEnable()
    {
        originalPos = transform.localPosition;
        seed = Random.value * 100f;
    }

    void Update()
    {
        if (shakeDuration > 0)
        {
            // Calculate normalized time (0 → 1)
            float normalizedTime = 1f - (shakeDuration / totalDuration);

            // Apply ease-in-out curve
            float ease = EaseInOut(normalizedTime);

            // Sample Perlin noise for smooth motion
            float x = (Mathf.PerlinNoise(Time.time * 10f + seed, 0f) - 0.5f) * 2f;
            float y = (Mathf.PerlinNoise(0f, Time.time * 10f + seed) - 0.5f) * 2f;

            // Apply magnitude scaled by easing factor
            transform.localPosition = originalPos + new Vector3(x, y, 0f) * shakeMagnitude * ease;

            // Reduce shake duration
            shakeDuration -= Time.deltaTime * dampingSpeed;
        }
        else
        {
            shakeDuration = 0f;
            transform.localPosition = originalPos;
        }
    }

    /// <summary>
    /// Triggers a smooth, eased screen shake.
    /// </summary>
    public void Shake(float duration, float magnitude)
    {
        totalDuration = duration;
        shakeDuration = duration;
        shakeMagnitude = magnitude;
    }

    /// <summary>
    /// Ease In-Out curve (S-curve)
    /// </summary>
    private float EaseInOut(float t)
    {
        // Smoothstep-like curve: accelerate then decelerate
        return 1f - Mathf.Cos(t * Mathf.PI);
    }
}
