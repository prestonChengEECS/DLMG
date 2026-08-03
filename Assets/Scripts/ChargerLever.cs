using UnityEngine;
using System.Collections;

public class ChargerLever : MonoBehaviour
{
    public bool activated = true;
    private bool playerAccessible = false;
    ChargerJumper chargerJumperScript;
    private bool isTurning = false;
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
        float duration = 2f;
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
}
