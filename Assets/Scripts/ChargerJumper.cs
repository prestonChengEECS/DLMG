using UnityEngine;

public class ChargerJumper : MonoBehaviour
{
    public int charge = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
    }
    // Update is called once per frame
    void Update()
    {
        foreach (Transform children in transform)
        {
            ChargerLever springComponent = children.GetComponent<ChargerLever>();

            if (springComponent != null && springComponent.activated)
            {
                charge++;
            }
        }
    }
}
