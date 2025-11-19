using UnityEngine;
using System.Collections;
using DG.Tweening;
using System.Collections.Generic;

public class BossAttacks : MonoBehaviour
{

    private bool readyToAttack = false;

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
    [SerializeField] private Transform leftSpikeAttackPoint;

    [SerializeField] private Transform rightSpikePivot;
    [SerializeField] private Transform rightSpikeElbow;
    [SerializeField] private GameObject rightSpike;
    [SerializeField] private Transform rightSpikeAttackPoint;

    [SerializeField] private float spikeAttackInterval = 0.75f;
    private bool leftAttacked = false;
    private bool leftArmMoving = true;
    private bool rightArmMoving = true;
    [SerializeField] private EnemyProjectile spikeShotPrefab;
    [SerializeField] private float spikeShotSpeed = 20f;
    private float justShotSpike;

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

    private Collider2D[] cols;

    public IEnumerator BossReviveSequence(Transform bossRoot)
    {
        // --- Collect all SpriteRenderers recursively ---
        List<SpriteRenderer> renderers = new List<SpriteRenderer>(bossRoot.GetComponentsInChildren<SpriteRenderer>());

        // --- Cache original colors ---
        Dictionary<SpriteRenderer, Color> originalColors = new Dictionary<SpriteRenderer, Color>();
        foreach (var sr in renderers)
            originalColors[sr] = sr.color;

        // --- Scale setup ---
        Vector3 originalScale = bossRoot.localScale;
        Vector3 smallScale = originalScale * 0.1f;   // smaller start
        Vector3 scaledUp = originalScale * 0.85f;     // overshoot before jump

        bossRoot.localScale = smallScale;

        // --- Step 1: Start dark (keep alpha intact) ---
        foreach (var sr in renderers)
        {
            Color c = originalColors[sr];
            c.r *= 0.2f;
            c.g *= 0.2f;
            c.b *= 0.2f;
            sr.color = c;
        }

        // --- Step 2: Appear slightly (brighten to ~30%) and scale up to normal ---
        Sequence appearSeq = DOTween.Sequence();
        appearSeq.Join(bossRoot.DOScale(originalScale, 1.2f).SetEase(Ease.OutQuad));

        foreach (var sr in renderers)
        {
            Color target = originalColors[sr];
            target.r *= 0.3f;
            target.g *= 0.3f;
            target.b *= 0.3f;
            appearSeq.Join(sr.DOColor(target, 1.2f));
        }

        yield return appearSeq.WaitForCompletion();

        // --- Step 3: Darken again before jumping (charging up) ---
        Sequence darkenSeq = DOTween.Sequence();
        foreach (var sr in renderers)
        {
            Color darker = originalColors[sr];
            darker.r *= 0.45f;
            darker.g *= 0.45f;
            darker.b *= 0.45f;
            darkenSeq.Join(sr.DOColor(darker, 0.4f));
        }
        yield return darkenSeq.WaitForCompletion();

        // --- Step 4: Squash slightly before jump ---
        yield return bossRoot.DOScale(scaledUp * 0.9f, 0.15f)
            .SetEase(Ease.InQuad)
            .WaitForCompletion();

        // --- Step 5: Jump way higher and faster ---
        float jumpHeight = 6.5f;
        float jumpDuration = 0.18f;
        Vector3 jumpTarget = bossRoot.position + Vector3.up * jumpHeight;

        yield return bossRoot.DOMove(jumpTarget, jumpDuration)
            .SetEase(Ease.OutCirc)
            .WaitForCompletion();

        // --- Step 6: Hover briefly ---
        yield return new WaitForSeconds(0.15f);

        // --- Step 7: Land FAST and hard ---
        Vector3 landTarget = bossRoot.position - Vector3.up * jumpHeight;

        yield return bossRoot.DOMoveY(landTarget.y, 0.4f)
            .SetEase(Ease.InCubic)
            .WaitForCompletion();

        // --- Step 8: Impact squash + 80% brightness ---
        Sequence impactSeq = DOTween.Sequence();

        // small squash before recovery
        impactSeq.Append(bossRoot.DOScale(originalScale * 0.9f, 0.1f).SetEase(Ease.InQuad));

        // brightness to 80% while returning to normal scale
        impactSeq.Append(bossRoot.DOScale(originalScale, 0.2f).SetEase(Ease.OutBack));

        foreach (var sr in renderers)
        {
            Color target = originalColors[sr];
            target.r *= 0.8f;
            target.g *= 0.8f;
            target.b *= 0.8f;
            impactSeq.Join(sr.DOColor(target, 0.3f));
        }

        yield return impactSeq.WaitForCompletion();

        // --- Step 9: Final full brightness ---
        yield return new WaitForSeconds(0.15f);
        Sequence finalSeq = DOTween.Sequence();
        foreach (var sr in renderers)
            finalSeq.Join(sr.DOColor(originalColors[sr], 0.3f).SetEase(Ease.OutSine));
        yield return finalSeq.WaitForCompletion();


        foreach (Collider2D col in cols)
            col.enabled = true;

        justShotSpike = Time.time;

        readyToAttack = true;
    }

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


        cols = GetComponentsInChildren<Collider2D>();

        foreach (Collider2D col in cols)
            col.enabled = false;

        StartCoroutine(BossReviveSequence(this.transform));
    }

    private void Update()
    {

        if (!readyToAttack)
            return;

        if (Time.time > justShotSpike + spikeAttackInterval)
        {
            // Left Spike Attack
            if (leftSpike != null && !leftAttacked)
            {
                leftArmMoving = false;
                DOVirtual.DelayedCall(0.15f, () =>
                {
                    leftAttacked = true;
                    Quaternion rot = leftSpikeAttackPoint.rotation;
                    EnemyProjectile spikeShotTemp = Instantiate(spikeShotPrefab, leftSpikeAttackPoint.position, rot);
                    Rigidbody2D rb = spikeShotTemp.GetComponent<Rigidbody2D>();
                    if (rb) rb.linearVelocity = rot * Vector2.down * spikeShotSpeed;



                    float recoilTargetY = leftSpikeElbow.localPosition.y - 0.5f;
                    leftSpikeElbow.DOLocalMoveY(recoilTargetY, 0.2f) // Shorter duration for the initial retract
                        .SetLoops(2, LoopType.Yoyo) // Retract (1) then return (2)
                        .SetEase(Ease.OutSine) // Makes the movement feel snappy
                        .OnComplete(() =>
                        {
                            DOVirtual.DelayedCall(0.15f, () =>
                            {
                                leftArmMoving = true;
                            });
                            
                        });
                });


            }
            // Right Spike Attack
            else if (rightSpike != null && leftAttacked)
            {
                rightArmMoving = false;

                DOVirtual.DelayedCall(0.15f, () =>
                {
                    leftAttacked = false;
                    Quaternion rot = rightSpikeAttackPoint.rotation;
                    EnemyProjectile spikeShotTemp = Instantiate(spikeShotPrefab, rightSpikeAttackPoint.position, rot);
                    Rigidbody2D rb = spikeShotTemp.GetComponent<Rigidbody2D>();
                    if (rb) rb.linearVelocity = rot * Vector2.down * spikeShotSpeed;



                    float recoilTargetY = rightSpikeElbow.localPosition.y - 0.5f;
                    rightSpikeElbow.DOLocalMoveY(recoilTargetY, 0.2f)
                        .SetLoops(2, LoopType.Yoyo)
                        .SetEase(Ease.OutSine)
                        .OnComplete(() =>
                        {
                            DOVirtual.DelayedCall(0.15f, () =>
                            {
                                rightArmMoving = true;
                            });
                            
                        });
                });

            }

            justShotSpike = Time.time;
        }

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
        if (leftSpikePivot != null && leftArmMoving)
            HandlePivotMovement(leftSpikePivot, ref leftSpikeTimer, ref leftSpikeTargetZ, true);
        if (rightSpikePivot != null && rightArmMoving)
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
