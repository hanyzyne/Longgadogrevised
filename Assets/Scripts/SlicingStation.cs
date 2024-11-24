using UnityEngine;

public class SlicingStation : MonoBehaviour
{
    public GameObject currentItem; // The item placed on the slicing station
    public Transform slicePosition; // Position where the item will be sliced
    private bool isItemReadyToSlice = false; // Flag to check if the item can be sliced
    private int sliceMax = 5;
    private int sliceMin = 0;


    public void PlaceItem(GameObject item)
    {
        if (currentItem == null && IsSliceable(item)) // Check if no item on station and item is sliceable
        {
            currentItem = item;
            currentItem.transform.SetParent(slicePosition);
            currentItem.transform.localPosition = Vector3.zero;
            isItemReadyToSlice = true;
            Debug.Log($"{currentItem.name} placed on slicing station.");
            Debug.Log($"Slicing is {isItemReadyToSlice}");
        }
    }

    public GameObject RemoveItem()
    {
        if (currentItem != null)
        {
            GameObject item = currentItem;
            currentItem = null;
            isItemReadyToSlice = false;
            item.transform.SetParent(null);
            Debug.Log($"{item.name} removed from slicing station.");
            return item;
        }
        return null;
    }

    public void SliceItem()
    {
        if (isItemReadyToSlice && currentItem != null)
        {
            sliceMin++;
            Debug.Log($"Sliced {sliceMin} times out of {sliceMax}");

            if (sliceMin >= sliceMax)
            {
                IngredientType ingredientType = currentItem.GetComponent<IngredientType>();
                if (ingredientType != null)
                {
                    ingredientType.ingredientType = "Sliced" + ingredientType.ingredientType;
                    Debug.Log($"Ingredient type updated to: {ingredientType.ingredientType}");
                }
                currentItem.name = "Sliced " + currentItem.name; // Update name for identification
                currentItem.tag = "Sliced"; // Optional: update the tag to identify it as sliced
                isItemReadyToSlice = false; // Reset flag since slicing is complete
                Debug.Log($"{currentItem.name} has been sliced!");
            }
        }
        else
        {
            Debug.Log("No item ready to slice.");
        }
    }

    private bool IsSliceable(GameObject item)
    {
        // Define criteria for which items can be sliced
        return item.CompareTag("Bun") || item.CompareTag("Lettuce"); // Add more tags if needed
    }
}
