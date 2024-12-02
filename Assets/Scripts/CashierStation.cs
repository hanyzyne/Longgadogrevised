using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CashierStation : MonoBehaviour
{
    public DeliveryStation deliveryStation;
    public OrderSpawner orderSpawner;
    public WaitingLine waitingLine;

    public void checkOrder()
    {
        List<string> deliveredIngredients = deliveryStation.ingredientsPlaced;
        List<string> orderIngredients = orderSpawner.order;

        if (AreListsEqual(deliveredIngredients, orderIngredients))
        {
            Debug.Log("Order is correct!");
            MoveFirstCustomerToLeaveZone(true);
        }
        else
        {
            Debug.Log("Order is incorrect!");
            MoveFirstCustomerToLeaveZone(false);
        }
    }

    private bool AreListsEqual(List<string> list1, List<string> list2)
    {
        if (list1.Count != list2.Count)
        {
            return false;
        }

        // Create temporary copies of the lists
        List<string> temp1 = new List<string>(list1);
        List<string> temp2 = new List<string>(list2);

        // Sort both lists to compare them regardless of order
        temp1.Sort();
        temp2.Sort();

        for (int i = 0; i < temp1.Count; i++)
        {
            if (temp1[i] != temp2[i])
            {
                return false;
            }
        }

        return true;
    }

    private void MoveFirstCustomerToLeaveZone(bool isHappy)
    {
        if (waitingLine == null)
        {
            Debug.LogError("WaitingLine reference is missing in CashierStation!");
            return;
        }

        if (waitingLine.customerQueue.Count > 0)
        {
            GameObject firstCustomer = waitingLine.customerQueue[0]; // Get the first customer in the queue
            CustomerMovement customerMovement = firstCustomer.GetComponent<CustomerMovement>();

            if (customerMovement != null)
            {
                customerMovement.MoveToLeaveZone(isHappy);
                waitingLine.RemoveCustomerFromQueue(firstCustomer); // Remove customer from the queue
            }
            else
            {
                Debug.LogError("CustomerMovement script is missing on the first customer!");
            }
        }
        else
        {
            Debug.Log("No customers in the waiting line.");
        }
    }
}
