using UnityEngine;

public class StaminaBarFollow : MonoBehaviour
{
    public Transform player; // Reference to the player's transform
    public Vector3 offset; // Offset from the player's position

    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        // Update the position of the stamina bar to follow the player
        if (player != null)
        {
            Vector3 worldPosition = player.position + offset; // Calculate the new position
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition); // Convert to screen position
            rectTransform.position = screenPosition; // Update the stamina bar's position
        }
    }
}
