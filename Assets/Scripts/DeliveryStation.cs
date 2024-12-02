using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryStation : MonoBehaviour
{
    public List<string> ingredientsPlaced = new List<string>();

    public void PlaceIngredient(GameObject item)
    {
        if (item != null && isPlaceable(item))
        {
            IngredientType ingredientType = item.GetComponent<IngredientType>();

            if (!ingredientsPlaced.Contains(ingredientType.ingredientType)) // Check if the ingredient already exists
            {
                ingredientsPlaced.Add(ingredientType.ingredientType);
                Debug.Log($"Added ingredient: {ingredientType.ingredientType}");
            }
            else
            {
                Debug.LogWarning($"Ingredient {ingredientType.ingredientType} already placed. Duplicate not allowed.");
            }
        }
    }

    private bool isPlaceable(GameObject item)
    {
        return item.CompareTag("Plate") || item.CompareTag("Cooked") || item.CompareTag("Sliced") || item.CompareTag("Condiments");
    }
}
