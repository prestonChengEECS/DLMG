using UnityEngine;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour 
{
    //an instance of the Player's Inventory. 
    public static PlayerInventory instance;
    //we need a list to store all of the items. we choose to store the class PickupItem in order to be able to store anythingwithin that class, such as ItemType, ItemName, or icon.
    public List<PickupItem> items = new List<PickupItem>();
    //the amount of items max that we can store. every time we pick up we decrement.
    public int maxSlots = 12;

    private void Awake()
    {
        if (instance == null) {
            instance = this;
        } else { 
            Destroy(gameObject); //if there is already an instance of a playerInventory, destroy the one that is trying to be created. 
        }
    }

    public bool AddItem(PickupItem item) {
        if (items.Count >= maxSlots)
        {
            return false;
        }
        items.Add(item); //add the item to the list
        InventoryUI.instance.AddItemToSlot(item.icon);
        return true;
    }
}
