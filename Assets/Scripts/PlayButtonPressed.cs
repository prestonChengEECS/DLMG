using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using System.Collections;

public class PlayButtonPressed : MonoBehaviour
{
    public Button button;

    public Animator quoteAnimator;

    private void Start(){
        button.onClick.AddListener(startGame);
    }

    private void startGame()
    {
        Debug.Log("The game has started");
        StartCoroutine(PlayAfterDelay());
    }

    private IEnumerator PlayAfterDelay()
    {
        yield return new WaitForSeconds(5f);
        quoteAnimator.SetTrigger("PlayQuote");
    }
}
