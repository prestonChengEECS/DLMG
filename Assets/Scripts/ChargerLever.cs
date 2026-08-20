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
    public AudioClip generatorEnd;

    public SpriteRenderer[] jumperIndicators;
    int currentCharge;
    int changedCharge;

    [Header("Volume Balancers")]
    [Range(0f, 1f)] public float startVolume = 1f;   // Slider for the startup clip
    [Range(0f, 1f)] public float runningVolume = 0.5f; // Slider for the loop (lower this if it's too loud!)
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        chargerJumperScript = transform.parent.GetComponent<ChargerJumper>();
    }

    private void Update()
    {
        if (playerAccessible && Input.GetKeyDown(KeyCode.F) && !isTurning)
        {
            currentCharge = chargerJumperScript.getCharge();
            isTurning = true; //prevents you from just spamming F;
            activated = !activated;
            StartCoroutine(pullLever());
            ActivationFade();
            chargerJumperScript.initializeCharge();
            changedCharge = chargerJumperScript.getCharge();
            updateJumperCharge();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerAccessible = true;
        }
    }

    //when the player is not in range of the lever don't they no longer have the option to pull it
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerAccessible = false;
        }
    }

    //this is solely for the visual transitional state
    private IEnumerator pullLever()
    {
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

        if (activated)
        {
            elapsedTime = 0;
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                t = elapsedTime / duration;
                lever.transform.rotation = Quaternion.Slerp(offState, onState, t);
                yield return null;
            }
        }
        else
        {
            elapsedTime = 0;
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                t = elapsedTime / duration;
                lever.transform.rotation = Quaternion.Slerp(onState, offState, t);
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

    private IEnumerator StartUp()
    {
        float customTime = 0f; //the clock driving the sine wave. It advances based on currentSpeed
        float currentSpeed = 2f; //determines how fast it blinks indirectly. Used to calculate customTime.
        float minAlpha = 0f; //we will keep on increasing minAlpha so that it will keep growing brighter and brighter.

        Transform onIndicator = transform.Find("indicatoron");

        if (onIndicator == null)
        {
            yield break;
        }

        SpriteRenderer onIndicatorRenderer = onIndicator.GetComponent<SpriteRenderer>();

        Color color = onIndicatorRenderer.color;


        audioSource.PlayOneShot(generatorStart);

        while (activated)
        {
            //update currentSpeed so that it accelerates your speed value over time up to a cap. Mathf.Min handles that cap so it will not go past 15f.
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

    private IEnumerator WindDown()
    {
        StartCoroutine(WindDownAudio());
        //we need customTime so that we can create our own blinking frequency.
        float customTime = 0f;
        //we also need currentSpeed and currentSpeed will continue to DECREASE because we powering down.
        //since we are powering down, we will need to start with rapid blinking and then whittle down to no blinking. 
        float currentSpeed = 15f;

        audioSource.PlayOneShot(generatorEnd);

        float currentMaxAlpha = 1f; //this decreases until it hits zero. 
        float currentMinAlpha = 0.8f; //this decreases until it hits zero.
        //now we need to reference the spriteRenderer in order to change the alpha value. 
        Transform onIndicator = transform.Find("indicatoron");
        
        if (onIndicator == null)
        {
            yield break;
        }

        SpriteRenderer onIndicatiorRenderer = onIndicator.GetComponent<SpriteRenderer>();

        while (!activated) {
            //we have to update currentSpeed.
            //we want currentSpeed to continually decrease because then that means we can decrease the rate of blinking
            currentSpeed = Mathf.Max(currentSpeed - (2f * Time.deltaTime), 0f);

            //when you plug in customTime into sine, the rate of change of customTime controls the pulsating speed. 
            //thus since currentSpeed continually decreases, the rate of change of customTime decreases as well, decreasing the overall pulsating speed. 
            customTime += Time.deltaTime * currentSpeed;

            float wavePattern = (Mathf.Sin(customTime) + 1f) / 2f;

            //now we lerp it using wavePattern, which oscillates between values 0 and 1!
            float alpha = Mathf.Lerp(currentMaxAlpha, currentMinAlpha, wavePattern);

            //finally we assign it to the onIndicator.
            Color newColor = onIndicatiorRenderer.color;
            newColor.a = alpha;
            onIndicatiorRenderer.color = newColor;

            yield return null;
        }
    }

    private IEnumerator WindDownAudio() {
        // 1. Store the running sound temporarily, then start fading it out
        // (Assuming your running loop is currently playing on the audioSource)
        float fadeDuration = 3f; // A super quick crossfade window
        float startVolume = audioSource.volume;

        // Quick volume fade out on the current running sound
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, timer / fadeDuration);
            yield return null;
        }
        audioSource.volume = 0f;
        audioSource.Stop();

    }

    private void updateJumperCharge() {
        if (currentCharge < changedCharge)
        {
            StartCoroutine(chargeUp());
        }
        else {
            StartCoroutine(chargeDown());
        }
    }

    private IEnumerator chargeUp() {

        int charge = chargerJumperScript.getCharge();

        //goes through the whole entire 
        for (int i = 0; i < charge; i++) {
            if (jumperIndicators[i].color.a < 1f) {
                float duration = 6f;
                float timeElapsed = 0f;
                float t = 0f;

                Color newColor = jumperIndicators[i].color;

                while (timeElapsed < duration) {
                    timeElapsed += Time.deltaTime;
                    t = timeElapsed / duration;
                    newColor.a = Mathf.Lerp(0f, 1f, t);

                    jumperIndicators[i].color = newColor;

                    yield return null;
                }
            }
        }

        //we need a lerp function so we can fade in the alpha. 
    }

    private IEnumerator chargeDown() {
        //gets the initial charge
        int charge = chargerJumperScript.getCharge();

        for (int i = jumperIndicators.Length - 1; i >= charge; i--) {
            if (jumperIndicators[i].color.a > 0f) {
                float timeElapsed = 0f;
                float duration = 6f;
                float t = 0f;

                Color newColor = jumperIndicators[i].color;
                newColor.a = jumperIndicators[i].color.a;

                while (timeElapsed < duration) {
                    timeElapsed += Time.deltaTime;
                    t = timeElapsed / duration;
                    float alpha = Mathf.Lerp(1f, 0f, t);

                    newColor.a = alpha;

                    jumperIndicators[i].color = newColor;
                    yield return null;
                }
            }
        }

    
    }
}