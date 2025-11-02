using UnityEngine;

public class FollowCursor : MonoBehaviour
{
    [Header("Follow Settings")]
    public float followSpeed = 15f;   // How quickly the object follows the cursor
    public bool smoothFollow = true;  // Toggle smooth movement
    public bool is2D = true;          // True = 2D mode, False = 3D mode

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;

        // Hide the system cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined; // Keep cursor within game window
    }

    void Update()
    {
        FollowMouse();
    }

    void FollowMouse()
    {
        // Convert mouse position to world space
        Vector3 mousePos = Input.mousePosition;
        Vector3 targetPos = mainCamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, Mathf.Abs(mainCamera.transform.position.z)));

        if (is2D)
        {
            // For 2D, keep Z fixed to object’s current Z
            targetPos.z = transform.position.z;
        }

        // Move toward the cursor
        if (smoothFollow)
        {
            transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);
        }
        else
        {
            transform.position = targetPos;
        }
    }

    private void OnDisable()
    {
        // Re-enable system cursor if this script is disabled
        Cursor.visible = true;
    }
}
