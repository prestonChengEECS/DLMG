using UnityEngine;
using System.Collections;

public class ChargerLever : MonoBehaviour
{
    public bool activated = true;
    private bool playerAccessible = false;
    ChargerJumper chargerJumperScript;
    private bool isTurning = false;

    public AudioSource audioSource;
    public AudioClip generatorStart;
    public AudioClip generatorRunning;
    public AudioClip generatorEnd;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        chargerJumperScript = transform.parent.GetComponent<ChargerJumper>();
    }

    private void Update()
    {
        if (playerAccessible && Input.GetKeyDown(KeyCode.F) && !isTurning) {
            isTurning = true; //prevents you from just spamming F;
            activated = !activated;
            StartCoroutine(pullLever());
            ActivationFade();
            chargerJumperScript.initializeCharge();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) {
            playerAccessible = true;
        }
    }

    //when the player is not in range of the lever don't they no longer have the option to pull it
    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            playerAccessible = false;
        }
    }

    //this is solely for the visual transitional state
    private IEnumerator pullLever() {
        //----------------------------------------------------------------------------------
        //initialize variables. there are two states, an off state and an on state
        //----------------------------------------------------------------------------------
        Quaternion offState = Quaternion.Euler(0f, 0f, 0f);
        Quaternion onState = Quaternion.Euler(0f, 0f, 45f);

        Transform lever = transform.GetChild(0); //gets the actual lever shaft of the lever.
        Quaternion currentState = lever.transform.rotation; //gets the current state of the lever. checks to see if it is on the onState or the offState


        float elapsedTime = 0f;
        float duration = 4f;
        float t = elapsedTime / duration;

        if (activated) {
            elapsedTime = 0;
            while (elapsedTime < duration) {
                elapsedTime += Time.deltaTime;
                t = elapsedTime / duration;
                lever.transform.rotation = Quaternion.Slerp(offState, onState, t);
                yield return null;
            }
        } else {
            elapsedTime = 0;
            while (elapsedTime < duration) {
                elapsedTime += Time.deltaTime;
                t = elapsedTime / duration;
                lever.transform.rotation = Quaternion.Slerp(onState, offState,t);
                yield return null;
            }
        }
        isTurning = false;
    }

    private void ActivationFade()
    {
        if (activated)
        {
            StartCoroutine(StartUp());
        }
        else
        {
            StartCoroutine(WindDown());
        }
    }

    private IEnumerator StartUp() {
        float customTime = 0f; //the clock driving the sine wave. It advances based on currentSpeed
        float currentSpeed = 2f; //determines how fast it blinks indirectly. Used to calculate customTime.
        float minAlpha = 0f; //we will keep on increasing minAlpha so that it will keep growing brighter and brighter.

        Transform onIndicator = transform.Find("indicatoron");

        if (onIndicator == null) {
            yield break;
        }

        SpriteRenderer onIndicatorRenderer = onIndicator.GetComponent<SpriteRenderer>();

        Color color = onIndicatorRenderer.color;


        audioSource.clip = generatorStart;
        audioSource.loop = false;
        audioSource.Play();

        while (activated) {
            //update currentSpeed so that it accelerates your speed value over time up to a cap. Mathf.Min handles that cap so it will not go past 25f.
            currentSpeed = Mathf.Min(currentSpeed + (2f * Time.deltaTime), 15f);

            //now we have to update customTime so that it can blink faster and faster each time.
            //quadratic growth because currentspeed keeps getting larger as time passes so you are adding bigger and bigger numbers to customTime each time. 
            customTime += Time.deltaTime * currentSpeed;

            //now we actually have to calculate the wavePattern or speed so we can pass it into Lerp
            //as customTime grows, the Mathf.Sin value changes more quickly, resulting in faster blinking patterns.
            //we update minAlpha so that the range in which the transparency can oscillate gets smaller and smaller.
            minAlpha = Mathf.Min(minAlpha + (0.15f * Time.deltaTime), 0.5f);
            float wavePattern = (Mathf.Sin(customTime) + 1f) / 2f;

            //now we fit it into Lerp!
            float currentAlpha = Mathf.Lerp(minAlpha, 1f, wavePattern);

            //now that we have the blinking sequence implemented, we can now assign it to the onIndicator!
            //first we create a new color (a result color) that we can map back to the onIndicatorRenderer.
            Color newColor = color;
            newColor.a = currentAlpha;

            onIndicatorRenderer.color = newColor;


            yield return null;
        }
    }

    private IEnumerator WindDown() {
        //we need customTime so that we can create our own blinking frequency.
        float customTime = 0f;
        //we also need currentSpeed and currentSpeed will continue to increase.
        float currentSpeed = 2f;

        //now we need to reference the spriteRenderer in order to change the alpha value. 
        Transform onIndicator = transform.Find("indicatoron");

        if (onIndicator == null) {
            yield break;
        }

        SpriteRenderer onIndicatiorRenderer = onIndicator.GetComponent<SpriteRenderer>();
    }
}
