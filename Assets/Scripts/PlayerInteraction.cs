using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public GameObject heldItem;
    public Transform holdPosition;
    private CookingStation nearbyCookingStation;
    private SlicingStation nearbySlicingStation; // Reference to the nearby slicing station
    private DeliveryStation nearbyDeliveryStation;
    private CashierStation nearbyCashierStation;
    private GameObject nearbyItem;
    

    public Transform dropPosition; // Position where the item will be dropped

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            DropItem();
        }

        if (Input.GetKeyDown(KeyCode.F) )
        {
            SliceItem();
        }
    }

    private void Interact()
    {
        if (heldItem != null && nearbyCookingStation != null && heldItem.CompareTag("Raw"))
        {
            nearbyCookingStation.PlaceItem(heldItem);
            heldItem.transform.SetParent(null);
            heldItem.SetActive(false);
            heldItem = null;
        }
        else if (heldItem == null && nearbyItem != null)
        {
            PickUpItem(nearbyItem);
            Debug.Log($"{heldItem.name} picked up.");
        }
        else if (heldItem == null && nearbyCookingStation != null && nearbyCookingStation.IsRemovable())
        {
            heldItem = nearbyCookingStation.RemoveItem();
            if (heldItem != null)
            {
                heldItem.transform.SetParent(holdPosition);
                heldItem.transform.localPosition = Vector3.zero;
                Debug.Log($"{heldItem.name} picked up from the cooking station.");
            }
        }
        else if (heldItem != null && nearbySlicingStation != null && nearbySlicingStation.currentItem == null)
        {
            // Place item on slicing station if the player is holding one
            nearbySlicingStation.PlaceItem(heldItem);
            heldItem = null;
        }
        else if (heldItem == null && nearbySlicingStation != null && nearbySlicingStation.currentItem != null)
        {
            // Pick up item from slicing station if the player is not holding one
            heldItem = nearbySlicingStation.RemoveItem();
            if (heldItem != null)
            {
                heldItem.transform.SetParent(holdPosition);
                heldItem.transform.localPosition = Vector3.zero;
                Debug.Log($"{heldItem.name} picked up from the slicing station.");
            }
        }
        else if (heldItem != null && nearbyDeliveryStation != null)
        {
            nearbyDeliveryStation.PlaceIngredient(heldItem);
            if (heldItem.CompareTag("Plate") || heldItem.CompareTag("Cooked") || heldItem.CompareTag("Sliced"))
            {
                heldItem.SetActive(false);
                heldItem = null;
            }

        }
        else if (heldItem == null && nearbyCashierStation != null)
        {
            nearbyCashierStation.checkOrder();
        }
        else
        {
            Debug.Log("No valid interaction available.");
        }
    }

    private void PickUpItem(GameObject item)
    {
        heldItem = item;
        heldItem.transform.SetParent(holdPosition);
        heldItem.transform.localPosition = Vector3.zero;
        nearbyItem = null; // Clear reference to picked-up item
    }

    public void DropItem()
    {
        if (heldItem != null)
        {
            heldItem.transform.SetParent(null);
            heldItem.transform.position = dropPosition.position;
            heldItem = null;
            Debug.Log("Item dropped.");
        }
        else
        {
            Debug.Log("No item to drop.");
        }
    }

    private void SliceItem()
    {
        // Slice the item currently on the slicing station
        if (nearbySlicingStation != null && heldItem == null && nearbySlicingStation.currentItem != null)
        {
            nearbySlicingStation.SliceItem();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("CookingStation"))
        {
            nearbyCookingStation = collision.collider.GetComponent<CookingStation>();
        }
        else if ((collision.collider.CompareTag("Bun") || collision.collider.CompareTag("Lettuce") || collision.collider.CompareTag("Raw") || collision.collider.CompareTag("Plate")) && heldItem == null || collision.collider.CompareTag("Condiments") && heldItem == null)
        {
            nearbyItem = collision.gameObject;
        }
        else if (collision.collider.CompareTag("SlicingStation"))
        {
            nearbySlicingStation = collision.collider.GetComponent<SlicingStation>();
        }
        else if (collision.collider.CompareTag("DeliveryStation"))
        {
            nearbyDeliveryStation = collision.collider.GetComponent<DeliveryStation>(); 
        }
        else if (collision.collider.CompareTag("CashierStation"))
        {
            nearbyCashierStation = collision.collider.GetComponent<CashierStation>();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("CookingStation"))
        {
            nearbyCookingStation = null;
        }
        else if (collision.gameObject == nearbyItem)
        {
            nearbyItem = null;
        }
        else if (collision.collider.CompareTag("SlicingStation"))
        {
            nearbySlicingStation = null;
        }
        else if (collision.collider.CompareTag("DeliveryStation"))
        {
            nearbyDeliveryStation = null;
        }
    }
}
