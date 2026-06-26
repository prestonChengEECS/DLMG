using UnityEngine;

public class Shard : MonoBehaviour
{
    private Rigidbody2D rb;
    private Collider2D cd;
    public float timeTillDestroy = 2.5f;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.simulated = false; //when set to false, physics isn't applied to the current rigidbody. 
    }

    public void crumble(Vector2 force, float randomRotationSpeed) {

        rb.simulated = true; //when this method is called, then physics is applicable. 

        //the first parameter force is Vector2 and specifies the direction in which the object moves.
        rb.AddForce(force, ForceMode2D.Impulse); //ForceMode2D.Impulse provides an instant one-time force.

        //applies a random rotation to each shard
        rb.angularVelocity = randomRotationSpeed;


        //Destroy the shard after a set amount of time to save performance. 
        Destroy(gameObject, timeTillDestroy);
    }
}
