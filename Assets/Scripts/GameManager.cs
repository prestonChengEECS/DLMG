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
    }

    private void Start()
    {
        GameObject just = GameObject.FindWithTag("Just");
        TextMeshProUGUI justText = just.GetComponent<TextMeshProUGUI>();

        if (justText != null) {
            justText.canvasRenderer.SetAlpha(0f);
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

}
