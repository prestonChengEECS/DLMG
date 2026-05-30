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
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject); //if there is already an instance of a playerInventory, destroy the one that is trying to be created. 
        }

        for (int i = 0; i < maxSlots; i++) {
            items.Add(null);
        }

    }

    public bool AddItem(PickupItem item)
    {
        for (int i = 0; i < items.Count; i++) {
            if (items[i] == null) { 
                items[i] = item;
                InventoryUI.instance.AddItemToSlot(item.icon, item.itemName, item.itemDescription);
                return true;
            }
        }

        return false; //all slots full, no nulls left, returns false and doesn't add.
    }
}
