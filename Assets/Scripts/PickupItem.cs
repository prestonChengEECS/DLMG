using UnityEngine;
using TMPro;
public class PickupItem : MonoBehaviour
{
    public enum ItemClass
    {
        Inventory,
        Collectible
    }
    [SerializeField]
    public string itemName;
    public string itemDescription;
    public Sprite icon;
    public bool interactable;
    public ItemClass itemClass;
    private bool playerIsNearby = false;
    private GameObject interact;
    public TMP_FontAsset myCustomFont;
    public GameObject collectibleExpanded;
    private void Update()
    {
        if (playerIsNearby && Input.GetKeyDown(KeyCode.F))
        {
            if (itemClass == ItemClass.Inventory)
            {
                bool pickedUp = PlayerInventory.instance.AddItem(this);
                if (pickedUp)
                {
                    Destroy(gameObject);
                }
            }
            else if (itemClass == ItemClass.Collectible)
            {
                bool pickedUp = PlayerInventory.instance.AddCollection(this);
                if (pickedUp)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerIsNearby = true;
            interact = new GameObject("InteractKey");
            interact.transform.SetParent(transform);
            TextMeshPro interactF = interact.AddComponent<TextMeshPro>();
            interactF.text = "F";
            interactF.fontSize = 8;
            interactF.font = myCustomFont;
            interactF.alignment = TextAlignmentOptions.Center;
            interact.transform.localPosition = new Vector3(0, 10f, 0);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerIsNearby = false;
        }
        Destroy(interact);
    }
}