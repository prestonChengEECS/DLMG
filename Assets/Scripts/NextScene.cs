using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


public class NextScene : MonoBehaviour 
{
    //determines the anount of delay before it boots up the next scene
    public float delay = 40f;

    public void LoadNextScene() {
        //calls method defined below. 
        StartCoroutine(LoadNextAfterDelay());
    }

    private IEnumerator LoadNextAfterDelay()
    {
        yield return new WaitForSeconds(delay);
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentIndex + 1);
    }

    private void Awake()
    {
        //we need it so that it does not destroy the gameManager and so that this can persist across all scenes
        DontDestroyOnLoad(gameObject);
    }
}
