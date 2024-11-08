/*using UnityEngine;

public class RawItem : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInteraction playerInteraction = other.GetComponent<PlayerInteraction>();
            if (playerInteraction != null)
            {
                playerInteraction.PickUpItem(gameObject); // Pass the item itself to the interaction script
            }
        }
    }
}
*/