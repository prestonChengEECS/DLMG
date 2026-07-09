using UnityEngine;

public class Spring : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public float frequency = 20f; //tracks how fast it bobs up and down
    public float damping = 4f; //basically how fast the coil stops oscillating
    public float intensity = 1.5f; //how high the initial jolt is

    private float elapsedTime = 0f; //custom stopwatch that we will use in the math
    public bool isBouncing = true; //determine the state whether it is bouncing or static
    private Vector3 originalPosition; //constantly updates as the spring component is oscillating

    //that way you can drag and drop each individual spring component inside of the array. Whenn TriggerSpringBounce is triggered, it is applied to all componets of the spring pad.
    public Spring[] connectedSpringComponents; 
    void Start()
    {
        originalPosition = transform.localPosition; //sets the initial position
    }

    // Update is called once per frame
    void Update()
    {
        if (isBouncing) {
            elapsedTime += Time.deltaTime; //continuously increments like a stop watch.

            //calculate the decaying amplitude (it gets smaller over time)
            //e^(elapsedTime) and since elapsed time gets larger continuously it just keeps getting smaller and smaller cause the overall formula is sin(intensity * e^(elapsedTime))
            float amplitude = Mathf.Sin(intensity * Mathf.Exp(-damping * elapsedTime)); //determines the scale/power of that movement

            //calculate the sine wave position aka the new y offset/difference we need to add to the current position.
            float yOffset = Mathf.Abs(Mathf.Sin(frequency * elapsedTime)) * amplitude;

            //add the yOffset to the current position.
            transform.localPosition = originalPosition + new Vector3(0, yOffset,0);

            //stop calculating and stop bouncing after it reaches a very small bounce amount.
            if (amplitude < 0.001f) {
                isBouncing = false;
                transform.localPosition = originalPosition; //set it back to the original position.
            }
        }
    }

    [ContextMenu("Trigger Bounce")]
    public void TriggerSpringBounce() {
        elapsedTime = 0; //reset the stopwatch
        isBouncing = true; //loop through the update loop again
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isBouncing) {
            TriggerSpringBounce();

            //goes through the connectedSpringComponents list which has all of the spring components.
            foreach (Spring component in connectedSpringComponents) {
                //as long as there is a Spring script on each of the components, TriggerSpringBounce is initiated across everything in the list.
                // && component != this makes sure that the component doesn't try to trigger itself and it just creates an infinite loop.
                /*
                 * Just a side note that this would work even without "component != this" so long as ONLY one of your scripts has connectedSpringComponents array filled in. 
                 * In this case I only have a filled in array for my base, that way when it runs through the array, all the other components with the Spring script are empty, and
                 * as a result it won't cause an infinite loop. 
                 * 
                 * An infinite loop would occur if all arrays were filled because each child spring would try to trigger everything in its own individual array, which would trigger the child again, 
                 * causing the scripts to ping pong back and forth forever. Keeping the child arrays empty safely breaks this chain.
                 */
                if (component != null && component != this) {
                    component.TriggerSpringBounce();

                }
            }
        }
    }
}
