using UnityEngine;
using System.Collections;

public class ClawProjectile : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer warningSprite; // optional
    [SerializeField] private LineRenderer lineRenderer;     // optional

    private float speed;
    private bool launched;
    private bool retracting;
    private Rigidbody2D rb;

    private Vector3 shotPosition;            // where it was shot from
    private System.Action onRetractComplete;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (warningSprite != null)
            warningSprite.enabled = false;

        if (lineRenderer != null)
            lineRenderer.positionCount = 0;  // hidden by default
    }

    public void BeginSequence(Transform shootFrom, float travelSpeed, System.Action onDone)
    {
        onRetractComplete = onDone;
        speed = travelSpeed;
        shotPosition = transform.position;   // exact world pos before launch
        StartCoroutine(LaunchSequence());
    }

    private IEnumerator LaunchSequence()
    {
        // 1️⃣ Warning sign
        if (warningSprite != null)
            warningSprite.enabled = true;

        yield return new WaitForSeconds(2f);

        if (warningSprite != null)
            warningSprite.enabled = false;

        // 2️⃣ Launch forward
        launched = true;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.linearVelocity = -transform.up * speed;

        // Start line updates
        if (lineRenderer != null)
            StartCoroutine(UpdateLineRenderer());

        // 3️⃣ Wait until stopped by collision
        yield return new WaitUntil(() => !launched);

        // 4️⃣ Stay for 3 seconds
        yield return new WaitForSeconds(3f);

        // 5️⃣ Retract
        retracting = true;
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;

        Vector3 startPos = transform.position;
        Vector3 targetPos = shotPosition;
        float t = 0f;
        float retractDuration = 1.5f;

        while (t < 1f)
        {
            t += Time.deltaTime / retractDuration;
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        // Reached pivot → line can disappear now
        if (lineRenderer != null)
            lineRenderer.positionCount = 0;

        retracting = false;
        onRetractComplete?.Invoke();
    }

    private IEnumerator UpdateLineRenderer()
    {
        if (lineRenderer == null)
            yield break;

        lineRenderer.positionCount = 2;

        // Keep updating until fully retracted (not just until retracting stops)
        while (launched || retracting || Vector3.Distance(transform.position, shotPosition) > 0.01f)
        {
            lineRenderer.SetPosition(0, shotPosition);         // origin
            lineRenderer.SetPosition(1, transform.position);   // claw tip
            yield return null;
        }

        // Hide line once truly back
        lineRenderer.positionCount = 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!launched || retracting)
            return;

        if (collision.GetComponent<PlayerController>() || collision.GetComponent<TAG_Ground>())
        {
            launched = false;
            rb.linearVelocity = Vector2.zero;
        }
    }
}
