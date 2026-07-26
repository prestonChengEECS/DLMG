using UnityEngine;

public class ChargerLever : MonoBehaviour
{
    public bool activated = true;
    ChargerJumper chargerJumper;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        chargerJumper = transform.parent.GetComponent<ChargerJumper>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) {
            chargerJumper.updateCharge();
        }
    }
}
