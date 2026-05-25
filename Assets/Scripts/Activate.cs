using UnityEngine;

public class Activate : MonoBehaviour
{
    private Animator animator;
    private bool isActivated;


    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isActivated)
        {
            isActivated = true;
            animator.Play("Activate");
        }
    }
}
