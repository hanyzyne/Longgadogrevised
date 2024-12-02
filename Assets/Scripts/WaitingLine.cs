using System.Collections.Generic;
using UnityEngine;

public class WaitingLine : MonoBehaviour
{
    public Transform[] lineSpots; // Assign this in the Inspector with the spots in the scene
    public bool[] spotOccupied; // Track whether a spot is occupied by any customer
    public List<GameObject> customerQueue = new List<GameObject>(); // Queue of customers waiting for a spot
    public int nextAvailableSpot = 0;  // Keeps track of the next available spot index

    private void Start()
    {
        if (lineSpots == null || lineSpots.Length == 0)
        {
            Debug.LogError("LineSpots array is not assigned!");
        }
        spotOccupied = new bool[lineSpots.Length]; // Initialize as empty (no spots occupied)
    }

    public void AssignSpotToCustomer(GameObject customer)
    {
        if (nextAvailableSpot < lineSpots.Length)
        {
            Transform targetSpot = lineSpots[nextAvailableSpot];
            CustomerMovement customerMovement = customer.GetComponent<CustomerMovement>();
            if (customerMovement != null)
            {
                customerMovement.MoveToSpot(targetSpot.position, nextAvailableSpot);
                customerMovement.SetAssignedSpotIndex(nextAvailableSpot); // Track the assigned spot
                OccupySpot(nextAvailableSpot);  // Mark spot as occupied
                Debug.Log($"Customer assigned to spot {nextAvailableSpot}");

                // After assigning the customer, update the next available spot
                UpdateNextAvailableSpot();  // Ensure the next available spot is updated after assignment
            }
        }
        else
        {
            Debug.LogWarning("No available spots left!");
        }
    }

    public bool IsSpotOccupied(int spotIndex)
    {
        return spotOccupied[spotIndex];
    }

    public void OccupySpot(int index)
    {
        if (index >= 0 && index < lineSpots.Length)
        {
            spotOccupied[index] = true;  // Mark the spot as occupied
        }
    }

    public void FreeSpot(int freedSpotIndex)
    {
        if (freedSpotIndex >= 0 && freedSpotIndex < spotOccupied.Length && spotOccupied[freedSpotIndex])
        {
            spotOccupied[freedSpotIndex] = false; // Free the spot
            Debug.Log($"Spot {freedSpotIndex} is now freed.");

            // Remove the customer from the queue
            GameObject customerToRemove = FindCustomerAtSpot(freedSpotIndex);
            if (customerToRemove != null)
            {
                RemoveCustomerFromQueue(customerToRemove);
            }

            FindObjectOfType<CustomerSpawner>().EnableSpawning();

            // Shift remaining customers in the queue
            ShiftCustomersToRight(freedSpotIndex);
            UpdateNextAvailableSpot(); // Ensure the next available spot is updated
        }
        else
        {
            Debug.LogWarning($"Attempted to free invalid or already free spot: {freedSpotIndex}");
        }
    }



    public void ShiftCustomersToRight(int freedSpotIndex)
    {
        Debug.Log($"Shifting customers starting from freed spot {freedSpotIndex}.");

        for (int i = freedSpotIndex; i < lineSpots.Length - 1; i++) // Start from freed spot and shift customers
        {
            GameObject nextCustomer = FindCustomerAtSpot(i + 1); // Find the customer at the next spot

            if (nextCustomer != null)
            {
                CustomerMovement customerMovement = nextCustomer.GetComponent<CustomerMovement>();
                if (customerMovement != null && customerMovement.GetAssignedSpotIndex() == i + 1)  // Check if customer is in the next spot
                {
                    MoveCustomerToSpot(i + 1, i); // Move the customer to the freed spot
                    spotOccupied[i] = true;
                    spotOccupied[i + 1] = false;
                    UpdateCustomerQueue(i + 1, i); // Update the customer queue to reflect the change
                }
            }
        }
    }


    private void UpdateCustomerQueue(int fromSpotIndex, int toSpotIndex)
    {
        GameObject customerToMove = FindCustomerAtSpot(fromSpotIndex);

        if (customerToMove != null)
        {
            customerQueue.Remove(customerToMove);
            customerQueue.Insert(toSpotIndex, customerToMove); // Insert customer at the new spot index
            Debug.Log($"Updated customer queue after moving customer from spot {fromSpotIndex} to {toSpotIndex}");
        }
    }

    private void MoveCustomerToSpot(int fromSpotIndex, int toSpotIndex)
    {
        GameObject customerToMove = FindCustomerAtSpot(fromSpotIndex);

        if (customerToMove != null)
        {
            CustomerMovement customerMovement = customerToMove.GetComponent<CustomerMovement>();
            if (customerMovement != null)
            {
                customerMovement.MoveToSpot(lineSpots[toSpotIndex].position, toSpotIndex);
                customerMovement.SetAssignedSpotIndex(toSpotIndex); // Update the customer's assigned spot index
            }
        }
    }

    private GameObject FindCustomerAtSpot(int spotIndex)
    {
        foreach (GameObject customer in GameObject.FindGameObjectsWithTag("Customer"))
        {
            CustomerMovement customerMovement = customer.GetComponent<CustomerMovement>();
            if (customerMovement != null && customerMovement.GetCurrentSpotIndex() == spotIndex)
            {
                return customer;
            }
        }
        return null;
    }

    public Transform[] GetLineSpots()
    {
        return lineSpots;
    }

    public void AddCustomerToQueue(GameObject customer)
    {
        customerQueue.Add(customer);
    }

    public int GetNumberOfCustomers()
    {
        return customerQueue.Count;
    }

    public void RemoveCustomerFromQueue(GameObject customer)
    {
        if (customerQueue.Contains(customer))
        {
            int indexToRemove = customerQueue.IndexOf(customer);
            customerQueue.RemoveAt(indexToRemove); // Remove customer from queue
            spotOccupied[indexToRemove] = false;  // Free the spot
            Debug.Log($"Customer removed from queue and spot {indexToRemove} freed.");
        }
        else
        {
            Debug.LogWarning("Customer not found in the queue.");
        }
    }


    public bool AreThereAvailableSpots()
    {
        foreach (bool isOccupied in spotOccupied)
        {
            if (!isOccupied)
            {
                return true;
            }
        }
        return false;
    }

    public void UpdateNextAvailableSpot()
    {
        for (int i = 0; i < spotOccupied.Length; i++)
        {
            if (!spotOccupied[i])
            {
                nextAvailableSpot = i;
                Debug.Log($"Next available spot updated to: {nextAvailableSpot}");
                return; // Exit once an available spot is found
            }
        }
        nextAvailableSpot = spotOccupied.Length; // All spots are occupied
        Debug.LogWarning("No available spots left!");
    }

}
