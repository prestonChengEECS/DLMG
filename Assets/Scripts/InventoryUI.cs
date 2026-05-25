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
        inventoryPanel.SetActive(false); //initially starts off my hiding the inventory panel
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) {
            inventoryPanel.SetActive(!inventoryPanel.activeSelf);
        }
    }

    public void AddItemToSlot(Sprite icon)
    {
        foreach (ItemSlot slot in slots) {
            if (!slot.isFull) {
                slot.SetItem(icon);
                break;
            }
        }
    }



}
