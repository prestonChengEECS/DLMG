using UnityEngine;
using System.Collections;

public class ChargerLever : MonoBehaviour
{
    public bool activated = true;
    private bool playerAccessible = false;
    ChargerJumper chargerJumperScript; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        chargerJumperScript = transform.parent.GetComponent<ChargerJumper>();
    }

    private void Update()
    {
        if (playerAccessible && Input.GetKeyDown(KeyCode.F)) {
            StartCoroutine(pullLever());
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

    IEnumerator pullLever() {
        Transform lever = transform.GetChild(0);
        Quaternion leverInitialRoation = lever.transform.rotation;

        Quaternion leverTargetRotation = Quaternion.Euler(0f, 0f, 45f);
        float duration = 2.0f;
        float elapsedTime = 0f;
        float t;
        while (elapsedTime < duration) {
            elapsedTime += Time.deltaTime;
            t = elapsedTime / duration;
            lever.transform.rotation = Quaternion.Slerp(leverInitialRoation, leverTargetRotation, t);
            yield return null;
        }
    }
}
