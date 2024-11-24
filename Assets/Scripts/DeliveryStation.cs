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
            ingredientsPlaced.Add(ingredientType.ingredientType);
            Debug.Log($"Added ingredient: {ingredientType.ingredientType}");
        }
    }

    private bool isPlaceable(GameObject item)
    {
        return item.CompareTag("Plate") || item.CompareTag("Cooked") || item.CompareTag("Sliced");
    }
}
