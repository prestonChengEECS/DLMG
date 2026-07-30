using UnityEngine;

public class CrawlerController : MonoBehaviour
{
    public Rigidbody2D rb;
    public float pullForce = 0.2f;

    public void StopAndReach() {
        rb.linearVelocity = new Vector2(0,rb.linearVelocityY);
    }

    public void PullForward() {
        rb.linearVelocity = new Vector2(-pullForce, rb.linearVelocityY);
    }

}