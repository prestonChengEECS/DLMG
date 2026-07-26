using UnityEngine;

public class ChargerJumper : MonoBehaviour
{
    public int charge = 0;
    public int currentCharge = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {
    }

    //updates the charge level after you interact with a lever
    public void updateCharge() {
        currentCharge = 0;
        foreach (Transform child in transform) {
            if (child != null) { 
                ChargerLever chargerLever = child.GetComponent<ChargerLever>();

                if (chargerLever != null) {
                    if (chargerLever.activated) {
                        currentCharge++;
                    }
                }
            }
        }

        if (charge != currentCharge ) { 
            charge = currentCharge;
        }
    }
}
