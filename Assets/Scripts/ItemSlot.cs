using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image icon;
    public bool isFull = false;

    public string itemName;
    public string itemDescription;

    public GameObject tooltip;
    public TextMeshProUGUI tooltipTitle;
    public TextMeshProUGUI tooltipDescription;

    public void SetItem(Sprite ItemIcon) {
        if (icon != null) {
            icon.sprite = ItemIcon;
            icon.enabled = true;
        }
        isFull = true; //marks the slot as taken.
    }

    public void ClearSlot() {

        if (icon != null) {
            icon.sprite = null; //the sprite 
            icon.enabled = false; //hides the icon
        }

        isFull = false; //marks the slot as free
    }

    //when you hover over a filled slot
    public void OnPointerEnter(PointerEventData eventData) {
        if (isFull) {
            //"text bubble" sets active and is now visible to the player
            tooltip.SetActive(true);
            //the name of the object is presented.
            tooltipTitle.text = itemName;
            //the description of the object is presented.
            tooltipDescription.text = itemDescription;
        }
    }

    //when you are no longer hovering over the slot
    public void OnPointerExit(PointerEventData eventData) {
        //gets hidden and can no longer be seen by the player. 
        tooltip.SetActive(false); 
    }
}
