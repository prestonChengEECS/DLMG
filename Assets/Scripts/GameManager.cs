using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; //necessary for IEnumerator
using TMPro;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Vector3 checkpointPosition;
    public string checkpointScene;
    public bool gameCompleted;

    private bool hasEndTriggerFaded = false;

    public float waitTime = 3f;

    public float justFadeInTime = 5.0f;
    public float dontFadeOutTime = 3.0f;

    public Sprite lighterSpadeOpen;
    public AudioSource lighterAudioSource;
    public AudioClip lighterAudio;
    public AudioClip flameAudio;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); //so that GameManager exists across all levels.
        }
        else
        {
            Destroy(gameObject); //so there is only ONE GameManager. Defensive programming
        }

        GameObject flame = GameObject.FindWithTag("Flame");
        if (flame != null) {
            Image flameImage = flame.GetComponent<Image>();

            if (flameImage != null)
            {
                flameImage.enabled = false;
            }
            else {
                Debug.Log("OH NAUR");
            }
        
        }
    }

    private void Start()
    {
        GameObject just = GameObject.FindWithTag("Just");

        if (just != null) {
            TextMeshProUGUI justText = just.GetComponent<TextMeshProUGUI>();

            if (justText != null)
            {
                justText.canvasRenderer.SetAlpha(0f);
            }
        }
    }

    public void SetCheckpoint(Vector3 position)
    {

        checkpointPosition = position;
        checkpointScene = SceneManager.GetActiveScene().name;
    }

    public void RespawnPlayer() {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null) {
            player.transform.position = checkpointPosition;
        }
    }



    private void Update()
    {
        if (gameCompleted && !hasEndTriggerFaded) {
            hasEndTriggerFaded = true;
            StartCoroutine(changeTitleScreen());
        }
    }

    //we want to change DONT in the title to JUST instead. this helps do that.
    //also changes the lighter from a closed position to an open position.
    private IEnumerator changeTitleScreen() {
        GameObject dont = GameObject.FindWithTag("Dont"); 
        GameObject just = GameObject.FindWithTag("Just");
        GameObject lighterButton = GameObject.FindWithTag("Lighter Button");
        Image lighterIcon = null;

        //finds the lighter image component
        foreach (Transform child in lighterButton.transform) {
            if (child.tag == "Lighter Icon") { 
                lighterIcon = child.GetComponent<Image>();
            }
        
        }

        yield return new WaitForSeconds(2.0f);

        if (dont != null) { 
            TextMeshProUGUI dontText = dont.GetComponent<TextMeshProUGUI>();

            if (dontText != null) {
                dontText.CrossFadeAlpha(0f, dontFadeOutTime, false);
            }
        }

        yield return new WaitForSeconds(4.0f);

        if (just != null) { 
            TextMeshProUGUI justText = just.GetComponent<TextMeshProUGUI>();

            if (justText != null) {
                justText.CrossFadeAlpha(1f, justFadeInTime, false);
            }
        }

        StartCoroutine(FlickerRoutine(lighterIcon));

        yield return new WaitForSeconds(6.0f);

        StartCoroutine(FlameRoutine());
    }


    //this is the method that triggers the lighter to switch states.
    //slerp (spherical linear interpolation) moves along the arc of a sphere, so use for rotations.
    //lerp (linear interpolation) moves in a straight line, so use for translations
    public IEnumerator FlickerRoutine(Image reference) {
        yield return new WaitForSeconds(2.0f);

        Vector3 originalScale = reference.transform.localScale; //the original size/scale of the lighter.
        Quaternion originalRotation =  reference.transform.localRotation; //the original rotation of the object

        //responsible for the tilting left and right effect
        Quaternion tiltRight = originalRotation * Quaternion.Euler(0f, 0f, -6f);
        Quaternion tiltLeft = originalRotation * Quaternion.Euler(0f, 0f, 8f);

        //responsible for resizing the lighter slightly to give that pop and flick effect.
        Vector3 squish = new Vector3(originalScale.x * 1.08f, originalScale.y * 0.88f, originalScale.z);
        Vector3 expand = new Vector3(originalScale.x * 0.92f, originalScale.y * 1.12f, originalScale.z);

        //squish it a little and rotate it right a little as well
        float elapsedTime = 0f;
        float duration1 = 0.12f;
        float t1 = 0f;

        while (elapsedTime < duration1) {
            elapsedTime += Time.deltaTime;
            t1 = elapsedTime / duration1;
            /* 
             * Lerp and Slerp take in two parameters. the first one is the start position. it denote where to start.
             * the second one is the end position, it is where we want the position to end.
             * Finally, the last parameter is a float that ranges from 0f to 1f. it basically tells you the percentage of rotation/translation it will stop at. 
             * For example, passing in Quaternion.Slerp(originalScale, tiltRight, 0.5), where tiltRight tilts it right by 6 degrees, it will stop at 3 degrees. 
             * However, since it is in a for loop and elapsedTime is always constantly updating and therefore t1 is always incresing until it reaches one, it will rotate gradually
             */
            reference.transform.localScale = Vector3.Lerp(originalScale, squish, t1);
            reference.transform.localRotation = Quaternion.Slerp(originalRotation,tiltRight, t1);

            yield return null; //necessary as it pauses the loop, draws the current frame to the screen, then comes back to resume the loop on the very next frame.
        }

        reference.sprite = lighterSpadeOpen; //flick open right at the peak

        elapsedTime = 0f;
        float duration2 = 0.08f;
        float t2 = 0f;


        lighterAudioSource.PlayOneShot(lighterAudio);
        while (elapsedTime < duration2) {

            elapsedTime += Time.deltaTime;
            t2 = elapsedTime / duration2;
            reference.transform.localScale = Vector3.Lerp(squish, expand ,t2);
            reference.transform.localRotation = Quaternion.Slerp(tiltRight, tiltLeft, t2);

            yield return null;
        }


        elapsedTime = 0f;
        float duration3 = 0.10f;
        float t3 = 0f;

        while (elapsedTime < duration3) {
            elapsedTime += Time.deltaTime;

            t3 = elapsedTime / duration3; //need to update t3 so it keeps rotating and scaling accordingly as time passes.

            reference.transform.localScale = Vector3.Lerp(expand, originalScale, t3);
            reference.transform.localRotation = Quaternion.Slerp(tiltLeft, originalRotation, t3);

            yield return null;
        }

        //just to ensure that it is set back to its original position
        reference.transform.localScale = originalScale;
        reference.transform.localRotation = originalRotation;
    }


    private IEnumerator FlameRoutine() {
        GameObject flame = GameObject.FindWithTag("Flame");
        if (flame == null) { //if there is no such object with a Flame tag. 
            yield break;
        }

        Vector3 originalScale = flame.transform.localScale; //the original size of the flame. 
        Image flameImage = flame.GetComponent<Image>();
        Vector3 originalPosition = flame.transform.localPosition;

        if (flameImage != null) {
            flameImage.enabled = true;
        }

        flame.transform.localScale = Vector3.zero;

        //-------------------------------------------------------------------------------------------------------------------------
        //PHASE 1: THE BURST STRETCH --> animate the flame exploding from nothing into an exaggerated and tall skinny.
        //-------------------------------------------------------------------------------------------------------------------------
        float elapsedTime = 0f;
        float duration = 0.1f;
        Vector3 stretch = new Vector3(originalScale.x * 0.6f, originalScale.y * 1.5f, originalScale.z);
        float t = 0f; //this will be updated constantly in the while loop as elapsedTime/duration.

        //-------------------------------------------------------------------------------------------------------------------------
        //PHASE 2: interlopate to the destination. then interlopate from its stretched position back down to its original scale.
        //-------------------------------------------------------------------------------------------------------------------------

        lighterAudioSource.PlayOneShot(flameAudio);

        yield return new WaitForSeconds(0.5f);

        while (elapsedTime < duration) {
            elapsedTime += Time.deltaTime;
            t = elapsedTime / duration; //keeps updating every single run of the while loop for a seamless "animation" the line below as a result keeps updating as well.
            flame.transform.localScale = Vector3.Lerp(Vector3.zero, stretch, t); //start from zero and then grow to its stretched length.

            yield return null; //updates the Lerp state every frame. 
        }

        elapsedTime = 0f;
        duration = 0.08f;
        t = 0f;

        while (elapsedTime < duration) {
            elapsedTime += Time.deltaTime;
            t = elapsedTime / duration;
            flame.transform.localScale = Vector3.Lerp(stretch, originalScale, t);

            yield return null;
        }

        //-------------------------------------------------------------------------------------------------------------------------
        //PHASE 3: have it vary in size for the rest of the time to simulate a flame instead of it just being static. 
        //-------------------------------------------------------------------------------------------------------------------------

        //Time.time * 30f speeds up the waveform clock in order to achieve a rapid fire jitter effect.
        //the higher the number you multiply Time.time by, the faster the fire jitters.
        //Multiplying the final wave result by 0.05f limits the wiggle to a tiny 5 percent scale offset.
        //the 0.05 is the amplitude. the higher the number you multiply by, the more noticable the jiggle is. 
        while (true) {
            float xOffset = Mathf.Cos(Time.time * 2f) * 0.02f; //just like math, Mathf.Cos is for the xOffset (cos refers to x)
            float yOffset = Mathf.Sin(Time.time * 2f) * 0.02f; //just like math. Mathf.Sin is for the yOffset (sin refers to y)

            flame.transform.localScale = new Vector3(originalScale.x + xOffset, originalScale.y + yOffset, originalScale.z);

            //stacking two waves together produces an organic wiggle kind of movement
            //this is possible all the while keeping the RectTransform position perfectly locked on the wick. 

            float baseSway = Mathf.Sin(Time.time * 3f) * 8f; //a broad but gentle body tilt
            float tipWiggle = Mathf.Sin(Time.time * 7f) * 2f; //a fast but sharp jitter

            //adding a faster sharper wave with a slower more broad wave creates a wiggle effect. 
            float finalZRotation = baseSway + tipWiggle;


            //apply the rotation effect
            flame.transform.localRotation = Quaternion.Euler(0f, 0f, finalZRotation);

            yield return null;

            //-------------------------------------------------------------------------------------------------------------------------
            //you use Mathf.sin when you want your movment to start perfectly at zero (the center) and smoothly build up. 
            //an example is a flame that starts pointing straight up, or a camera shake that smoothly starts from a standstill.

            //you use Math.cos when you want your movement to start at its absolute max value (the peak).
            //an example of this is a swining pendulum that you drop from its highest point, or an object you want to start fully scaled up. 
            //-------------------------------------------------------------------------------------------------------------------------
        }


    }
}
