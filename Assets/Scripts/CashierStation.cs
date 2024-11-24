using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CashierStation : MonoBehaviour
{
    public DeliveryStation deliveryStation;
    public OrderSpawner orderSpawner;

    public void checkOrder()
    {
        List<string> deliveredIngredients = deliveryStation.ingredientsPlaced;
        List<string> orderIngredients = orderSpawner.order;
        /* CustomerMovement customerMovement = new CustomerMovement();
        GameObject leaveZoneObject = GameObject.FindGameObjectWithTag("LeaveZone");
        Transform leaveZone = leaveZoneObject != null ? leaveZoneObject.transform : null; */

        if (AreListsEqual(deliveredIngredients, orderIngredients))
        {
            Debug.Log("Order is correct!");
            // customerMovement.MoveTowardsLeaveZone(leaveZone.position);
            // Perform success actions here (e.g., give points, remove items)
        }
        else
        {
            Debug.Log("Order is incorrect!");
            // customerMovement.MoveTowardsLeaveZone(leaveZone.position);
            // Perform failure actions here (e.g., notify player, reset)
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
}
