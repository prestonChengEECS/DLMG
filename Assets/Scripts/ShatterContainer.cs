using UnityEngine;
using System.Collections;

public class ShatterContainer : MonoBehaviour
{
    private BoxCollider2D cd;

    private void Start()
    {
        cd = GetComponent<BoxCollider2D>();
    }
    public void TriggerCrumble() {

        //since this is attached to the ShatterContainer, it goes through every single child/shard of the ShatterContainer.
        //this for loop is responsible for going through each individual shard, assigning a random downward force and random X sway, and then 
        foreach (Transform shardTransform in transform) {
            //this reaches in shardTransform current child object, finds the script component of it, and then hands you a reference to it. 
            Shard shard = shardTransform.GetComponent<Shard>(); 

            //so it drifts slightly to the left and right and is not just rigid and falls straight down.
            float randomX = Random.Range(-1f, 1f);

            //gives it a downforce. randomized so some shards fall faster than others.
            float heavyDownForce = Random.Range(-3f,-6f);

            //each shard gets their own unique vector!
            Vector2 crumbleForce = new Vector2(randomX, heavyDownForce);

            //also assigns a random spin float so each invidual shard spins at different rates.
            float randomSpin = Random.Range(-50f, 50f);

            //finally initiates the crumble sequence to that one individual shard.
            if (shard != null) {
                shard.crumble(crumbleForce, randomSpin);
            }
        }
        cd.enabled = false;
    }

    //this is the master switch which actually calls the function and makes the function run.
    private IEnumerator OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) {
            yield return new WaitForSeconds(2.0f);
            TriggerCrumble();
        }
    }
}
