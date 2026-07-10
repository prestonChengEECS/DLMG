using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; //necessary for IEnumerator
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Vector3 checkpointPosition;
    public string checkpointScene;
    public bool gameCompleted;
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
    public void changeTitleScreen() {
        GameObject dont = GameObject.FindWithTag("Dont");


        if (dont != null) {
            TextMeshProUGUI dontText = dont.GetComponent<TextMeshProUGUI>();

            if (dontText != null) {
                dontText.CrossFadeAlpha(0f, 2f, false);
            }
        }

        
    }

    private void Update()
    {
        if (gameCompleted) {
            changeTitleScreen();
        }
    }
}
