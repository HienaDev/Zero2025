using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private float damage = 10f;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    //private void OnTriggerEnter2D(Collider2D other)
    //{
    //    if (other.GetComponent<PlayerController>())
    //    {
    //        // You can add player health interaction here
    //        // other.GetComponent<PlayerHealth>()?.TakeDamage(damage);
    //        Destroy(gameObject);
    //    }
    //}

    public void Launch(Vector2 direction, float speed)
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = direction * speed;
    }
}
