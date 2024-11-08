using UnityEngine;
using System.Collections;

public class CookingStation : MonoBehaviour
{
    public float cookingTime = 5f; // Time to cook the item
    public float burnTime = 10f; // Time before the item burns
    public GameObject cookedItemPrefab; // Prefab for the cooked item
    public GameObject burntItemPrefab; // Prefab for the burnt item
    public GameObject rawItemPrefab; // Prefab for the raw item visual representation

    private GameObject currentItem; // The item currently being cooked
    private Coroutine cookingCoroutine; // Reference to the cooking coroutine
    private GameObject visualItem; // Visual representation of the item
    private bool isCooked = false; // Has the item cooked?
    private bool isBurnt = false; // Has the item burnt?

    void Update()
    {
        // No need to handle cooking or burning in Update, it's handled in coroutines
    }

    public void PlaceItem(GameObject item)
    {
        if (currentItem == null)
        {
            currentItem = item; // Set the current item to the item placed
            isBurnt = false; // Reset burn state
            isCooked = false; // Reset cooked state

            // Instantiate the visual representation of the raw item
            visualItem = Instantiate(rawItemPrefab, transform.position, Quaternion.identity);
            visualItem.transform.SetParent(transform); // Optional: Set parent to keep it organized

            Debug.Log($"{item.name} is now cooking.");
            cookingCoroutine = StartCoroutine(CookingProcess()); // Start cooking coroutine
        }
        else
        {
            Debug.Log("Cooking station is busy!");
        }
    }

    public GameObject RemoveItem()
    {
        if (currentItem != null && isCooked) // Only return item if it's cooked
        {
            Debug.Log("Cooked item has been removed.");
            GameObject instantiatedItem = Instantiate(cookedItemPrefab, transform.position, Quaternion.identity);
            Destroy(currentItem); // Destroy the current item after instantiation
            Destroy(visualItem); // Destroy visual representation of the item
            currentItem = null; // Reset current item
            isCooked = false; // Reset cooked state for next cooking item
            return instantiatedItem; // Return the instantiated item
        }
        else if (isBurnt)
        {
            Debug.Log("Burnt item has been removed.");
            GameObject instantiatedItem = Instantiate(burntItemPrefab, transform.position, Quaternion.identity);
            Destroy(currentItem);
            Destroy(visualItem); // Destroy visual representation of the item
            currentItem = null;
            isBurnt = false; // Reset burnt state for next cooking item
            return instantiatedItem; // Return the instantiated burnt item
        }

        Debug.Log("No item to remove!");
        return null;
    }

    private IEnumerator CookingProcess()
    {
        yield return new WaitForSeconds(cookingTime); // Wait for cooking time

        // If cooking is done, set item as cooked
        if (currentItem != null)
        {
            isCooked = true; // Set cooked state
            Debug.Log($"{currentItem.name} is cooked!");

            // Update visual representation to cooked item
            Destroy(visualItem); // Destroy raw item visual
            visualItem = Instantiate(cookedItemPrefab, transform.position, Quaternion.identity);
            visualItem.transform.SetParent(transform); // Optional: Set parent to keep it organized
        }

        // Wait for the burn time if the item is not removed
        yield return new WaitForSeconds(burnTime - cookingTime); // Wait for the remaining time until it burns

        // Check if the item has burnt
        if (currentItem != null)
        {
            isBurnt = true; // Set burnt state
            isCooked = false; // Ensure cooked state is false
            Debug.Log("The item has burnt!");

            // Update visual representation to burnt item
            Destroy(visualItem); // Destroy cooked item visual
            visualItem = Instantiate(burntItemPrefab, transform.position, Quaternion.identity);
            visualItem.transform.SetParent(transform); // Optional: Set parent to keep it organized
        }
    }

    // Check if the item is removable (only if cooked)
    public bool IsRemovable()
    {
        return (isCooked || isBurnt); // Allow removing if item is cooked or burnt
    }
}
