using UnityEngine;

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
            pullLever();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) {
            playerAccessible = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            playerAccessible = false;
        }
    }

    public void pullLever() { 
        
    }
}
