using UnityEngine;

public class PickupItem : MonoBehaviour 
{
    public string itemName;
    public Sprite icon;
    public ItemType itemType;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if there is a collision between the player and the 
        if (collision.CompareTag("Player")) {
            bool pickedUp = PlayerInventory.instance.AddItem(this);

            if (pickedUp) {
                Destroy(gameObject);            
            }
        }
    }
}
