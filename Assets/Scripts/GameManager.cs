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
        }
    }

    //we want to change DONT in the title to JUST instead. this helps do that.
    //also changes the lighter from a closed position to an open position.
    private IEnumerator changeTitleScreen() {
        GameObject dont = GameObject.FindWithTag("Dont");
        yield return new WaitForSeconds(2.0f);
    }

}
