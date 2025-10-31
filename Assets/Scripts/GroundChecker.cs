using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    private PlayerController playerController;

    private void Start()
    {
        playerController = GetComponentInParent<PlayerController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<TAG_Ground>() != null)
            playerController.SetGrounded(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<TAG_Ground>() != null)
            playerController.SetGrounded(false);
    }
}
