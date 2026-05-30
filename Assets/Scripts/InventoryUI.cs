using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    public GameObject inventoryPanel; //the panel itself basically the skeleton of the whole inventory system.
    public static InventoryUI instance; //a single instance of the inventory UI. there is one and only one.where 
    public ItemSlot[] slots;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        inventoryPanel.SetActive(false); //initially starts off by hiding the inventory panel

        //helps assign a specific number aka ID to each invidiual physical slot. important for dragging and dropping and swapping. 
        for (int i = 0; i < slots.Length; i++) {
            slots[i].slotIndex = i;
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) {
            inventoryPanel.SetActive(!inventoryPanel.activeSelf);
        }
    }

    public void AddItemToSlot(Sprite icon, string name, string description)
    {
        foreach (ItemSlot slot in slots) {
            if (!slot.isFull) {
                slot.SetItem(icon, name, description);
                break;
            }
        }
    }



}
