using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrillStation : MonoBehaviour
{
    public enum ItemState { Raw, Cooked, Burnt } // Enumeration for item states

    [System.Serializable]
    public class GrillItem
    {
        public GameObject item;
        public ItemState state; // Current state of the item
        public float cookTimer; // Timer for the cooking process

        public GrillItem(GameObject item)
        {
            this.item = item;
            this.state = ItemState.Raw; // Default state is raw
            this.cookTimer = 0f; // Start with zero cook time
        }
    }

    public int maxItems = 1; // Number of items the grill can hold
    private GrillItem[] itemsOnGrill;

    private bool isGrillOccupied = false; // Flag to check if the grill is occupied

    public Sprite cookedHotdogSprite;
    public Sprite cookedBunSprite;
    public Sprite burntHotdogSprite; // New Sprite for burnt hotdogs
    public Sprite burntBunSprite; // New Sprite for burnt buns

    public float overcookTimeHotdog = 10f; // Time limit for overcooking hotdogs
    public float overcookTimeBun = 6f; // Time limit for overcooking buns

    public int nonInteractableLayer;
    public int interactableLayer;

    void Start()
    {
        itemsOnGrill = new GrillItem[maxItems]; // Array to hold items on the grill
    }

    void Update()
    {
        if (isGrillOccupied)
        {
            for (int i = 0; i < maxItems; i++)
            {
                if (itemsOnGrill[i] != null && itemsOnGrill[i].item != null)
                {
                    itemsOnGrill[i].cookTimer += Time.deltaTime;

                    // Check for hotdog
                    if (itemsOnGrill[i].item.CompareTag("Hotdog"))
                    {
                        HandleCooking(i, overcookTimeHotdog, cookedHotdogSprite, burntHotdogSprite, 5f, ItemState.Cooked);
                    }
                    // Check for bun
                    else if (itemsOnGrill[i].item.CompareTag("Bun"))
                    {
                        HandleCooking(i, overcookTimeBun, cookedBunSprite, burntBunSprite, 3f, ItemState.Cooked);
                    }
                }
            }
        }
    }

    // Method to handle cooking logic for each item
    void HandleCooking(int index, float overcookTime, Sprite cookedSprite, Sprite burntSprite, float cookedTime, ItemState cookedState)
    {
        // Check for cooked item
        if (itemsOnGrill[index].cookTimer >= cookedTime && itemsOnGrill[index].state == ItemState.Raw)
        {
            CookItem(index, cookedSprite, cookedState);
        }
        // Check for burnt item
        else if (itemsOnGrill[index].cookTimer >= overcookTime && itemsOnGrill[index].state != ItemState.Burnt)
        {
            BurnItem(index, burntSprite);
        }
    }

    public bool IsGrillOccupied()
    {
        return isGrillOccupied;
    }

    // Method to place an item on the grill
    public void PlaceItem(GameObject item)
    {
        for (int i = 0; i < maxItems; i++)
        {
            if (itemsOnGrill[i] == null) // Find an empty slot
            {
                itemsOnGrill[i] = new GrillItem(item); // Place the item in the slot
                item.transform.SetParent(transform); // Attach to grill station
                item.transform.localPosition = Vector3.zero; // Reset position
                item.layer = nonInteractableLayer; // Make item non-interactable while cooking

                isGrillOccupied = true; // Set the grill as occupied
                Debug.Log(item.name + " is placed on the grill.");
                return;
            }
        }
        Debug.Log("Cannot place item on grill, it's already occupied!");
    }

    // Method to get a cooked item from the grill
    public GameObject GetCookedItem()
    {
        for (int i = 0; i < maxItems; i++)
        {
            if (itemsOnGrill[i] != null &&
                (itemsOnGrill[i].state == ItemState.Cooked))
            {
                return itemsOnGrill[i].item; // Return the cooked item
            }
        }
        return null;
    }

    // Method to release a slot and reset the grill status if necessary
    public void ReleaseSlot(GameObject item)
    {
        for (int i = 0; i < maxItems; i++)
        {
            if (itemsOnGrill[i] != null && itemsOnGrill[i].item == item) // Find the item in the grill slots
            {
                itemsOnGrill[i] = null; // Clear the item from the grill
                Debug.Log(item.name + " has been removed from the grill, checking if slots are empty.");
                break;
            }
        }

        // Check if all slots are empty
        bool allSlotsEmpty = true;
        for (int i = 0; i < maxItems; i++)
        {
            if (itemsOnGrill[i] != null)
            {
                allSlotsEmpty = false; // If any slot is occupied, break the loop
                break;
            }
        }

        if (allSlotsEmpty)
        {
            isGrillOccupied = false; // Reset the flag only if all slots are empty
            Debug.Log("All slots are empty, grill is now free.");
        }
    }

    // Method to cook an item on the grill
    void CookItem(int index, Sprite cookedSprite, ItemState newState)
    {
        if (itemsOnGrill[index] != null && itemsOnGrill[index].item.GetComponent<SpriteRenderer>() != null)
        {
            itemsOnGrill[index].item.GetComponent<SpriteRenderer>().sprite = cookedSprite; // Change the sprite to the cooked version
            itemsOnGrill[index].state = newState; // Update the item's state to cooked
            itemsOnGrill[index].item.layer = interactableLayer; // Make the item interactable again
            Debug.Log(itemsOnGrill[index].item.name + " is cooked!"); // Debug log for cooked item
        }
    }

    // Method to burn an item on the grill
    void BurnItem(int index, Sprite burntSprite)
    {
        if (itemsOnGrill[index] != null && itemsOnGrill[index].item.GetComponent<SpriteRenderer>() != null)
        {
            itemsOnGrill[index].item.GetComponent<SpriteRenderer>().sprite = burntSprite; // Change the sprite to the burnt version
            itemsOnGrill[index].state = ItemState.Burnt; // Update the item's state to burnt
            itemsOnGrill[index].item.layer = interactableLayer; // Make the burnt item interactable again
            Debug.Log(itemsOnGrill[index].item.name + " is burnt!"); // Debug log for burnt item
        }
    }
}
