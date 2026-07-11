using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; //necessary for IEnumerator
using TMPro;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Vector3 checkpointPosition;
    public string checkpointScene;
    public bool gameCompleted;

    private bool hasEndTriggerFaded = false;

    public float waitTime = 3f;

    public float justFadeInTime = 3.0f;
    public float dontFadeOutTime = 3.0f;
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


    //we want to change DONT in the title to please
    public async void changeTitleScreen() {

        GameObject dont = GameObject.FindWithTag("Dont");
        GameObject just = GameObject.FindWithTag("Just");

        if (dont != null) {
            TextMeshProUGUI dontText = dont.GetComponent<TextMeshProUGUI>(); //gets the text component of the "Dont" gameObject

            if (dontText != null) {
                dontText.CrossFadeAlpha(0f, dontFadeOutTime, false);
            }
        }

        await Task.Delay((int)(waitTime * 1000));

        if (just != null) {
            TextMeshProUGUI justText = just.GetComponent<TextMeshProUGUI>();

            if (justText != null) { 
                justText.CrossFadeAlpha(1f, justFadeInTime, false);
            }

        }
    }

    private void Update()
    {
        if (gameCompleted && !hasEndTriggerFaded) {
            hasEndTriggerFaded = true;
            changeTitleScreen();
        }
    }

}
