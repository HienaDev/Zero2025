using UnityEngine;

public class MoveUpForever : MonoBehaviour
{
    public float speed = 5f; // Movement speed in units per second

    void Update()
    {
        // Move the object upward (along the Y-axis) every frame
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }
}
