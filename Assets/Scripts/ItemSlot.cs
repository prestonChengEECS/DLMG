using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public Image icon;
    public bool isFull = false;

    public string itemName;
    public string itemDescription;

    public GameObject tooltip;
    public TextMeshProUGUI tooltipTitle;
    public TextMeshProUGUI tooltipDescription;


    public int slotIndex; //physical slot GameObject needs to know its own position number

    private GameObject dragIcon; //this will be the floating icon copy of the item being dragged.

    public void SetItem(Sprite ItemIcon, string name, string description) {
        if (icon != null) {
            icon.sprite = ItemIcon;
            icon.enabled = true;
        }
        itemName = name;
        itemDescription = description;
        isFull = true; //marks the slot as taken.
    }

    public void ClearSlot() {

        if (icon != null) {
            icon.sprite = null; //the sprite 
            icon.enabled = false; //hides the icon
        }

        //clears out the description so when you hover over the now empty slot, it doesn't retain the description.
        itemDescription = "";
        itemName = "";
        isFull = false; //marks the slot as free
    }


    //when you hover over a filled slot, the corresponding description appears
    public void OnPointerEnter(PointerEventData eventData) {
        Debug.Log("Hovering over slot! isFull = " + isFull);
        if (isFull) {
            Debug.Log("Showing tooltip!");
            //"text bubble" sets active and is now visible to the player
            tooltip.SetActive(true);
            //the name of the object is presented.
            tooltipTitle.text = itemName; 
            //the description of the object is presented.
            tooltipDescription.text = itemDescription;
        }
    }

    //when you are no longer hovering over the slot, the description disappears
    public void OnPointerExit(PointerEventData eventData) {
        //gets hidden and can no longer be seen by the player. 
        tooltip.SetActive(false); 
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //if it is empty don't do anything at all
        if (!isFull) {
            return;
        }

        //if it is not empty create a new GameObject that will have an image component so it can copy the image.
        dragIcon = new GameObject("DragIcon");
        dragIcon.transform.SetParent(transform.root); //sets dragIcon GameObject to the child of the canvas.
        dragIcon.transform.SetAsLastSibling(); //is the very last child in the Canvas's children list so it renders on top of everything else.
        Image img = dragIcon.AddComponent<Image>(); //creates an image component so that there is an actual visual when dragging. 
        img.sprite = icon.sprite; //img sprite becomes a copy of the icon sprite. 
        img.raycastTarget = false; //setting it to false makes the dragIcon invisible to the raycaster, so mouse events won't block the dragging and dropping visual. 

        //helps keep the image sizes consistent when dragging
        RectTransform dragRect = dragIcon.GetComponent<RectTransform>(); ;
        RectTransform iconRect = icon.GetComponent<RectTransform>();
        dragRect.sizeDelta = iconRect.sizeDelta;
    }

    //while holding the mouse down, the icon follows with where the mouse is.
    public void OnDrag(PointerEventData eventData)
    {
        if (dragIcon != null) {
            dragIcon.transform.position = eventData.position; //eventData.position is the position of the mouse.
        }
    }


    //we want to dragonIcon to be deleted when the pointer is released
    public void OnEndDrag(PointerEventData eventData) {
        Destroy(dragIcon);
    }

    //when we drop it on top of another slot, we want it to swap places. this is what handles it
    public void OnDrop(PointerEventData eventData) { 
        //pointerDrag specifically gives you the GameObject that is currently being dragged. This is the slot you picked up and are dragging.
        //GetComponent gets the ItemSlot script attached to that dragged GameObject, so you can access its icon, itemName, itemDescription.
        ItemSlot draggedFrom = eventData.pointerDrag.GetComponent<ItemSlot>();

        /*
         * Stores the destination slot's data. 
         * Takes into account it's sprite, name, and description, as well as if it was filled in the first place.
         * isFull = true will be helpful to evaluate because we need to know if we have to swap.
         * isFull = false means we just have to move from one slot to another. We don't need to transfer data such as tempSprite, tempName, etc to the origin slot.
         */
        Sprite tempSprite = icon.sprite;
        string tempName = itemName;
        string tempDescription = itemDescription;
        bool tempFull = isFull;

        //put all of this data into a new destination slot
        SetItem(draggedFrom.icon.sprite, draggedFrom.itemName, draggedFrom.itemDescription);

        if (tempFull)
        {
            //if you are dragging into an already populated slot, swap them.
            draggedFrom.SetItem(tempSprite, tempName, tempDescription);
        }
        else { 
            //if you are dragging into an empty slot you just need to clear the slot.
            draggedFrom.ClearSlot();
        }

        //help reevaluate the PlayerInventory list to adjust to the visual UI change.

        //this is a temporary variable where it denotes the index of the list corresponding to the draggedFrom index.
        PickupItem temp = PlayerInventory.instance.items[draggedFrom.slotIndex];
        //the starting slot's information is now the ending slot's information.
        PlayerInventory.instance.items[draggedFrom.slotIndex] = PlayerInventory.instance.items[slotIndex];
        //the ending slot's information is now the starting slot's information.
        PlayerInventory.instance.items[slotIndex] = temp;

        
    }
}
