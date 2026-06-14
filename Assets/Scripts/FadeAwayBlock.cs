using UnityEngine;
using System.Collections;

public class FadeAwayBlock : MonoBehaviour
{
    public float collapsingTime = 1.5f;
    private SpriteRenderer spriteRenderer; //controls whether you can see the sprite or not.
    private BoxCollider2D boxCollider; //so you can toggle the box collider on and off.
    private bool isCollapsing;


    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    //we use oncollisionenter2d in order to get the solid physics landing data.
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) {
            //gets the first first point where the player hit the block. .normal then gets the normal vector.
            
            /*
             * it is put in vector form. x and y coordinates in the form (x,y). A hitPoint Vector2 of (0,-1)
             * means you land directly on top of the object that this script is attached to. 
             */
            Vector2 hitPoint = collision.GetContact(0).normal;

            //we use -0.7 for some leniency. in case if the player doesn't land directly on top.
            //we need to check isCollapsing because if the player jumps again we check the normal vector again and it can restart the sequence. 
            if (hitPoint.y < -0.7 && !isCollapsing) {
                StartCoroutine(CollapseSequence());
            }
        }
    }

    IEnumerator CollapseSequence()
    {
        isCollapsing = true; //start collapsing

        yield return new WaitForSeconds(collapsingTime); //wait for a few seconds
        spriteRenderer.enabled = false;
        boxCollider.enabled = false;
    }
}
