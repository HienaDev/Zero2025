using UnityEngine;
using System.Collections;

public class BossAttacks : MonoBehaviour
{
    private BossHealth bossHealth; // has method AreAllArmsDestroyed()

    [Header("Left Arm")]
    [SerializeField] private Transform leftClawPivot;
    [SerializeField] private Transform leftClawElbow;
    [SerializeField] private GameObject leftClaw;

    [Header("Right Arm")]
    [SerializeField] private Transform rightClawPivot;
    [SerializeField] private Transform rightClawElbow;
    [SerializeField] private GameObject rightClaw;

    [Header("Pivot Movement Settings")]
    [SerializeField] private float pivotRotationSpeed = 30f;
    [SerializeField] private Vector2 pivotRotationRange = new Vector2(15f, 40f);
    [SerializeField] private float pivotChangeInterval = 2f;

    [Header("Claw Attack Settings")]
    [SerializeField] private Vector2 attackIntervalRange = new Vector2(4f, 8f);
    [SerializeField] private float aimDuration = 1f;
    [SerializeField] private float clawSpeed = 20f;

    private Transform player;
    private float nextAttackTime;

    private float leftTargetZ;
    private float rightTargetZ;
    private float leftTimer;
    private float rightTimer;
    private bool isLeftAttacking;
    private bool isRightAttacking;

    [Header("Spike Arms")]
    [SerializeField] private Transform leftSpikePivot;
    [SerializeField] private Transform leftSpikeElbow;
    [SerializeField] private GameObject leftSpike;

    [SerializeField] private Transform rightSpikePivot;
    [SerializeField] private Transform rightSpikeElbow;
    [SerializeField] private GameObject rightSpike;

    private float leftSpikeTargetZ;
    private float rightSpikeTargetZ;
    private float leftSpikeTimer;
    private float rightSpikeTimer;

    [Header("Mouth Projectile Attack")]
    [SerializeField] private Transform mouthPoint;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 12f;
    [SerializeField] private float fireInterval = 2f;

    [Tooltip("Base number of projectiles per wave (e.g. 12)")]
    [SerializeField] private int projectileCountPerWave = 8;

    [Tooltip("Cone angle in degrees for the wave")]
    [SerializeField] private float spreadAngle = 60f;

    [Header("Projectile Count Oscillation")]
    [Tooltip("How much to subtract from 'projectileCountPerWave' on alternating waves. If 1, it will fire 12, 11, 12, 11, ...")]
    [SerializeField] private int projectileCountVariation = 1;

    private bool _alternateCountToggle = false; // toggles count each wave
    private float nextMouthFireTime;

    [Header("Face Sweep Attack")]
    [Tooltip("Root transform of the face (do NOT assign the mouthPoint here). This is what moves left/right.")]
    [SerializeField] private Transform faceRoot;

    [SerializeField] private bool enableSweepAttack = false;
    [Tooltip("How far the face moves to each side in local X.")]
    [SerializeField] private float sweepDistance = 3f;
    [Tooltip("How fast the face moves side to side.")]
    [SerializeField] private float sweepSpeed = 2f;

    private float sweepStartX;
    private bool sweepingRight = true;

    private void Start()
    {
        player = FindAnyObjectByType<PlayerController>()?.transform;
        bossHealth = GetComponent<BossHealth>();

        leftTargetZ = Random.Range(-pivotRotationRange.x, pivotRotationRange.y);
        rightTargetZ = Random.Range(-pivotRotationRange.y, pivotRotationRange.x);
        leftSpikeTargetZ = Random.Range(-pivotRotationRange.x, pivotRotationRange.y);
        rightSpikeTargetZ = Random.Range(-pivotRotationRange.y, pivotRotationRange.x);

        leftTimer = rightTimer = 0f;
        leftSpikeTimer = rightSpikeTimer = 0f;
        nextAttackTime = Time.time + Random.Range(attackIntervalRange.x, attackIntervalRange.y);

        if (faceRoot != null)
            sweepStartX = faceRoot.localPosition.x;
    }

    private void Update()
    {
        // switch to mouth phase once all arms are destroyed
        if (bossHealth != null && bossHealth.AreAllArmsDestroyed())
        {
            if (enableSweepAttack) HandleSweepAttack(); // moves the FACE, not the firepoint
            HandleMouthProjectiles();
            return; // skip arm logic
        }

        // --- Pivot idle rotation (Claws) ---
        if (!isLeftAttacking && leftClawPivot != null)
            HandlePivotMovement(leftClawPivot, ref leftTimer, ref leftTargetZ, true);

        if (!isRightAttacking && rightClawPivot != null)
            HandlePivotMovement(rightClawPivot, ref rightTimer, ref rightTargetZ, false);

        // --- Pivot idle rotation (Spikes) ---
        if (leftSpikePivot != null)
            HandlePivotMovement(leftSpikePivot, ref leftSpikeTimer, ref leftSpikeTargetZ, true);
        if (rightSpikePivot != null)
            HandlePivotMovement(rightSpikePivot, ref rightSpikeTimer, ref rightSpikeTargetZ, false);

        // --- Periodic arm attack ---
        if (Time.time >= nextAttackTime)
        {
            if (Random.value < 0.5f && !isLeftAttacking && leftClaw != null)
                StartCoroutine(ClawAttack(leftClawPivot, leftClawElbow, leftClaw, true));
            else if (!isRightAttacking && rightClaw != null)
                StartCoroutine(ClawAttack(rightClawPivot, rightClawElbow, rightClaw, false));

            nextAttackTime = Time.time + Random.Range(attackIntervalRange.x, attackIntervalRange.y);
        }
    }

    // ---------------------------
    // Mouth projectile waves
    // ---------------------------
    private void HandleMouthProjectiles()
    {
        if (Time.time < nextMouthFireTime || mouthPoint == null || projectilePrefab == null)
            return;

        // 1) compute alternating projectile count: base N vs (N - variation)
        int effectiveCount = projectileCountPerWave - (_alternateCountToggle ? projectileCountVariation : 0);
        _alternateCountToggle = !_alternateCountToggle;

        // clamp to at least 1 so we never divide by zero or spawn none
        effectiveCount = Mathf.Max(1, effectiveCount);

        // 2) compute angles for a constant cone spread
        float currentSpread = spreadAngle;
        float halfSpread = currentSpread * 0.5f;

        if (effectiveCount == 1)
        {
            // single shot straight forward
            Quaternion rot = mouthPoint.rotation;
            GameObject proj = Instantiate(projectilePrefab, mouthPoint.position, rot);
            Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
            if (rb) rb.linearVelocity = rot * Vector2.down * projectileSpeed;
        }
        else
        {
            float angleStep = currentSpread / (effectiveCount - 1);
            for (int i = 0; i < effectiveCount; i++)
            {
                float angle = -halfSpread + angleStep * i;
                Quaternion rotation = mouthPoint.rotation * Quaternion.Euler(0, 0, angle);
                GameObject proj = Instantiate(projectilePrefab, mouthPoint.position, rotation);
                Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
                if (rb) rb.linearVelocity = rotation * Vector2.down * projectileSpeed;
            }
        }

        nextMouthFireTime = Time.time + fireInterval;
    }

    // ---------------------------
    // Face sweep (left-right) — moves the FACE, not the firepoint
    // ---------------------------
    private void HandleSweepAttack()
    {
        if (faceRoot == null) return;

        Vector3 pos = faceRoot.localPosition;
        float targetX = sweepingRight ? (sweepStartX + sweepDistance) : (sweepStartX - sweepDistance);

        pos.x = Mathf.MoveTowards(pos.x, targetX, sweepSpeed * Time.deltaTime);
        faceRoot.localPosition = pos;

        if (Mathf.Abs(pos.x - targetX) < 0.01f)
            sweepingRight = !sweepingRight;
    }

    private void HandlePivotMovement(Transform pivot, ref float timer, ref float targetZ, bool isLeft)
    {
        if (pivot == null) return;

        timer += Time.deltaTime;

        float currentZ = pivot.localEulerAngles.z;
        if (currentZ > 180) currentZ -= 360;

        if (Mathf.Abs(currentZ - targetZ) < 1f)
        {
            timer = 0f;
            targetZ = isLeft
                ? Random.Range(-pivotRotationRange.x, pivotRotationRange.y)
                : Random.Range(-pivotRotationRange.y, pivotRotationRange.x);
        }

        float newZ = Mathf.MoveTowards(currentZ, targetZ, pivotRotationSpeed * Time.deltaTime);
        pivot.localEulerAngles = new Vector3(0, 0, newZ);
    }

    private IEnumerator ClawAttack(Transform pivot, Transform elbow, GameObject claw, bool left)
    {
        if (player == null || elbow == null || claw == null)
            yield break;

        if (left) isLeftAttacking = true;
        else isRightAttacking = true;

        float elapsed = 0f;

        while (elapsed < aimDuration)
        {
            elapsed += Time.deltaTime;
            if (player == null) break;

            Vector3 dir = (player.position - elbow.position).normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 90f;
            elbow.rotation = Quaternion.Lerp(
                elbow.rotation,
                Quaternion.Euler(0, 0, angle),
                Time.deltaTime * 5f
            );

            yield return null;
        }

        var clawScript = claw.GetComponent<ClawProjectile>();
        if (clawScript)
        {
            bool done = false;
            clawScript.BeginSequence(elbow, clawSpeed, () => done = true);
            yield return new WaitUntil(() => done);
        }

        if (left) isLeftAttacking = false;
        else isRightAttacking = false;
    }
}
