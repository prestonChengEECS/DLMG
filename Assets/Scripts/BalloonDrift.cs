using UnityEngine;

public class BalloonDrift : MonoBehaviour
{
    public float driftSpeed = 0.5f;
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, driftSpeed);
    }
}
