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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        chargerJumperScript = transform.parent.GetComponent<ChargerJumper>();
    }

    private void Update()
    {
        if (playerAccessible && Input.GetKeyDown(KeyCode.F) && !isTurning) {
            isTurning = true; //prevents you from just spamming F
            StartCoroutine(pullLever());
            activated = !activated;
            StartCoroutine(ActivationFade());
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
        float duration = 6f;
        float t = elapsedTime / duration;

        if (!activated) {
            elapsedTime = 0;
            while (elapsedTime < duration) {
                elapsedTime += Time.deltaTime;
                t = elapsedTime / duration;
                lever.transform.rotation = Quaternion.Slerp(offState, onState, t);
                yield return null;
            }
            activated = true;
        } else if (activated) {
            elapsedTime = 0;
            while (elapsedTime < duration) {
                elapsedTime += Time.deltaTime;
                t = elapsedTime / duration;
                lever.transform.rotation = Quaternion.Slerp(onState, offState,t);
                yield return null;
            }
            activated = false;
        }

        isTurning = false;
    }

    private IEnumerator ActivationFade() { 
        SpriteRenderer onStateIndication = transform.GetChild(3).GetComponent<SpriteRenderer>();
        float fadeDuration = 6f;
        float timeElapsed = 0f;
        float t = 0f;

        //cache the original color and its starting alpha
        Color startColor = onStateIndication.color;
        float startAlpha = onStateIndication.color.a;

        //this is the ending state that we are trying to reach.
        float targetAlpha = 1f;
        

        if (activated) {
            //switch from fully transparent to fully visible
            while (timeElapsed < fadeDuration) {
                timeElapsed += Time.deltaTime;
                t = timeElapsed / fadeDuration;

                //Lerp the alpha value.
                float currentAlpha = Mathf.Lerp(startAlpha, targetAlpha, t);

                //apply it to the asset
                Color newColor = startColor; //make a copy of the color.
                newColor.a = currentAlpha;
                onStateIndication.color = newColor;
                yield return null;
                
            }

            while (activated) {
                //the "heartbeat" of the pulsing effect. sin makes it harmonic so it fades in and fades out and so on
                //mathf.sin itself returns a value between -1 and 1. a phase shift of 1 makes it so that it outputs values between 0 and 2. finally, dividing that by 2 gets you an output between 0 and 1. 
                //we need values between 0 and 1 in order to use Lerp correctly.
                float pulse = (Mathf.Sin(Time.time * 4f) + 1f) / 2f;
                float currentAlpha = Mathf.Lerp(0.3f, 1.0f, pulse);

                Color newColor = startColor;
                newColor.a = currentAlpha;
                onStateIndication.color = newColor;
                yield return null;
            }
        }

        if (!activated) {
            startColor = onStateIndication.color;
            startAlpha = onStateIndication.color.a;

            timeElapsed = 0f;
            t = 0f;

            while (timeElapsed < fadeDuration) {
                timeElapsed += Time.deltaTime;
                t = timeElapsed / fadeDuration;

                float currentAlpha = Mathf.Lerp(startAlpha, 0f, t);

                Color newColor = startColor;
                newColor.a = currentAlpha;
                onStateIndication.color = newColor;
                yield return null;
            }
        }
    }
}
