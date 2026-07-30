using UnityEngine;

public class ChargerLever : MonoBehaviour
{
    public bool activated = true;
    ChargerJumper chargerJumperScript; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        chargerJumperScript = transform.parent.GetComponent<ChargerJumper>();
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) {
            
        }
    }
}
