using UnityEngine;
using DG.Tweening;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Camera))]
public class CameraTweenToPlayer : MonoBehaviour
{
    [Header("Tween Settings")]
    public float moveDuration = 3f;
    public Ease moveEase = Ease.InOutSine;

    [Header("Rotation")]
    public float rotationSpeedStart = 90f;
    public float rotationSpeedEnd = 360f;

    [Header("Zoom (Orthographic Size)")]
    public float targetOrthoSize = 3f;
    public float zoomDuration = 3f;

    [Header("Lens Distortion")]
    public float targetDistortion = 0.7f;
    public float distortionDuration = 3f;

    private Transform player;
    private Camera cam;
    private LensDistortion lensDistortion;

    private bool isTweening = false;
    private bool isReversing = false;
    private float rotationSpeed;
    private float tweenTimer = 0f;

    private Quaternion startRotation;
    private Vector3 startPosition;
    private float startOrthoSize;

    void Start()
    {
        cam = GetComponent<Camera>();
        startPosition = transform.position;
        startRotation = transform.rotation;
        startOrthoSize = cam.orthographicSize;

        var playerController = FindAnyObjectByType<PlayerController>();
        if (playerController != null)
            player = playerController.transform;
        else
            Debug.LogError("No PlayerController found in scene!");

        Volume volume = FindAnyObjectByType<Volume>();
        if (volume != null && volume.profile.TryGet(out lensDistortion))
            lensDistortion.intensity.value = 0f;
        else
            Debug.LogError("No Volume with LensDistortion found in scene!");

        TweenOut();
    }

    void Update()
    {
        if (!isTweening) return;

        tweenTimer += Time.deltaTime;
        float progress = Mathf.Clamp01(tweenTimer / moveDuration);
        float easedProgress = DOVirtual.EasedValue(0, 1, progress, moveEase);

        // Dynamic follow on XY
        Vector3 playerPos = new Vector3(player.position.x, player.position.y, transform.position.z);

        if (!isReversing)
        {
            // Forward: move camera toward player
            transform.position = Vector3.Lerp(startPosition, playerPos, easedProgress);
            rotationSpeed = Mathf.Lerp(rotationSpeedStart, rotationSpeedEnd, progress);
            transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime, Space.Self);
        }
        else
        {
            // Reverse: move camera away from player (opposite direction)
            transform.position = Vector3.Lerp(playerPos, startPosition, easedProgress);
            rotationSpeed = Mathf.Lerp(rotationSpeedStart, rotationSpeedEnd, progress);
            transform.Rotate(Vector3.forward, -rotationSpeed * Time.deltaTime, Space.Self);
        }

        // End behavior
        if (progress >= 1f)
        {
            isTweening = false;
            transform.DORotateQuaternion(startRotation, 0.5f).SetEase(Ease.InOutSine);
        }
    }

    // Called when the player dies / level resets
    public void TweenIn()
    {
        if (isTweening || player == null) return;

        isReversing = false;
        StartTweenCommon();

        // Distortion up
        DOTween.To(() => lensDistortion.intensity.value,
                   x => lensDistortion.intensity.value = x,
                   targetDistortion,
                   distortionDuration)
               .SetEase(Ease.InOutSine);

        // Zoom in
        DOTween.To(() => cam.orthographicSize,
                   x => cam.orthographicSize = x,
                   targetOrthoSize,
                   zoomDuration)
               .SetEase(Ease.InOutSine);
    }

    // Called at the start of the level (dreamy entry)
    public void TweenOut()
    {
        if (isTweening || player == null) return;

        // Start from "zoomed-in/distorted" state so we can reverse out of it
        cam.orthographicSize = targetOrthoSize;
        if (lensDistortion != null) lensDistortion.intensity.value = targetDistortion;

        isReversing = true;
        StartTweenCommon();

        // Distortion down
        DOTween.To(() => lensDistortion.intensity.value,
                   x => lensDistortion.intensity.value = x,
                   0f,
                   distortionDuration)
               .SetEase(Ease.InOutSine);

        // Zoom out to normal
        DOTween.To(() => cam.orthographicSize,
                   x => cam.orthographicSize = x,
                   startOrthoSize,
                   zoomDuration)
               .SetEase(Ease.InOutSine);
    }

    private void StartTweenCommon()
    {
        isTweening = true;
        tweenTimer = 0f;
        rotationSpeed = rotationSpeedStart;
    }
}
