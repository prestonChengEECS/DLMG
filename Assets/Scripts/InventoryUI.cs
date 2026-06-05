using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{

    public static InventoryUI instance; //a single instance of the inventory UI. there is one and only one.

    public GameObject inventoryPanel; //the panel itself basically the skeleton of the whole inventory system.
    public GameObject inventoryBackground; //the background of the inventory system.
    public GameObject inventory; //the text "INVENTORY".
    public ItemSlot[] slots;

    public GameObject collectiblesPanel;
    public GameObject collectiblesBackground;
    public GameObject collectibles;
    public ItemSlot[] collectibleSlots;



    public AudioSource inventoryAudioSource;
    public AudioClip zipperOpen;
    public AudioClip zipperClose;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        inventoryPanel.SetActive(false); //initially starts off by hiding the inventory panel
        inventoryBackground.SetActive(false); //initially starts off by hiding the background.
        inventory.SetActive(false);

        collectiblesPanel.SetActive(false);
        collectiblesBackground.SetActive(false);
        collectibles.SetActive(false);

        //helps assign a specific number aka ID to each invidiual physical slot. important for dragging and dropping and swapping. 
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].slotIndex = i;
        }

        for (int i = 0; i < collectibleSlots.Length; i++)
        {
            collectibleSlots[i].slotIndex = i;
        }
    }
    private void Update()
    {
        //responsible for opening and closing the inventory
        if (Input.GetKeyDown(KeyCode.Q))
        {

            if (inventoryPanel.activeSelf) {
                inventoryAudioSource.PlayOneShot(zipperClose);
            } else if (!inventoryPanel.activeSelf) {
                inventoryAudioSource.PlayOneShot(zipperOpen);
            }

            inventoryPanel.SetActive(!inventoryPanel.activeSelf);
            inventoryBackground.SetActive(!inventoryBackground.activeSelf);
            inventory.SetActive(!inventory.activeSelf);

            collectiblesPanel.SetActive(false);
            collectiblesBackground.SetActive(false);
            collectibles.SetActive(false);
        }

        if (inventoryPanel.activeSelf && Input.GetKeyDown(KeyCode.E))
        {


            if (collectiblesPanel.activeSelf)
            {
                inventoryAudioSource.PlayOneShot(zipperClose);
            }
            else if (!collectiblesPanel.activeSelf)
            {
                inventoryAudioSource.PlayOneShot(zipperOpen);
            }

            collectiblesPanel.SetActive(!collectiblesPanel.activeSelf);
            collectiblesBackground.SetActive(!collectiblesBackground.activeSelf);
            collectibles.SetActive(!collectibles.activeSelf);

            inventoryPanel.SetActive(false);
            inventoryBackground.SetActive(false);
            inventory.SetActive(false);
        }
    }

    public void AddItemToSlot(Sprite icon, string name, string description)
    {
        foreach (ItemSlot slot in slots)
        {
            if (!slot.isFull)
            {
                slot.SetItem(icon, name, description);
                break; //once it is added to the slot break out of the for loop
            }
        }
    }

    public void AddItemToCollections(Sprite icon, string name, string description, GameObject collectibleExpandedUI)
    {
        foreach (ItemSlot slot in collectibleSlots)
        {
            if (!slot.isFull)
            {
                slot.SetCollections(icon, name, description, collectibleExpandedUI);
                break;
            }
        }

    }



}