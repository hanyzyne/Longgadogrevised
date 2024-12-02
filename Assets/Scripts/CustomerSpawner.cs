using System.Collections;
using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    public GameObject customerPrefab;  // Reference to the customer prefab
    public Transform spawnPoint;       // The point where the customers will spawn
    private WaitingLine waitingLine;   // Reference to the WaitingLine script
    public Transform[] lineSpots;      // Array to hold the line spots

    private bool isSpawningAllowed = true;  // Flag to control spawning status

    private void Start()
    {
        // Find the WaitingLine script in the scene
        waitingLine = FindObjectOfType<WaitingLine>();

        if (waitingLine == null)
        {
            Debug.LogError("WaitingLine script not found in the scene!");
        }

        // Start spawning customers after 3 seconds and repeat every 8-12 seconds
        StartCoroutine(SpawnCustomers());
    }

    private void Update()
    {

    }

    // Coroutine to handle customer spawning
    private IEnumerator SpawnCustomers()
    {
        // Wait for 3 seconds before the first spawn
        yield return new WaitForSeconds(3f);

        // Spawn the first customer
        SpawnCustomer();

        // Loop to spawn customers at random intervals (between 8-12 seconds)
        while (isSpawningAllowed)
        {
            // Check the number of customers in the queue before spawning
            if (waitingLine.GetNumberOfCustomers() < 6 && isSpawningAllowed)
            {
                // Wait for a random time between 8 to 12 seconds
                float waitTime = Random.Range(10f, 30f);
                yield return new WaitForSeconds(waitTime);

                // Spawn another customer if the queue is not full
                SpawnCustomer();
            }
            else
            {
                // If the queue is full, we wait a bit and then recheck
                yield return new WaitForSeconds(1f);  // Check every 1 second
            }
        }
    }

    // Method to spawn a customer at the spawn point and add to the queue
    // Add this to ensure spots are available before spawning
    private void SpawnCustomer()
    {
        if (customerPrefab != null && spawnPoint != null && waitingLine != null)
        {
            // Check if there is space for more customers
            if (waitingLine.GetNumberOfCustomers() >= 6)
            {
                Debug.LogWarning("No available spots to spawn a new customer!");
                return;  // Do not spawn if the queue is full
            }

            // Instantiate the customer at the spawn point
            GameObject newCustomer = Instantiate(customerPrefab, spawnPoint.position, Quaternion.identity);

            // Get the CustomerMovement script from the instantiated customer
            CustomerMovement customerMovement = newCustomer.GetComponent<CustomerMovement>();

            if (customerMovement != null)
            {
                // Assign the customer to a spot sequentially
                waitingLine.AssignSpotToCustomer(customerMovement.gameObject);

                // Update the queue status in the waiting line
                waitingLine.AddCustomerToQueue(newCustomer);

                // If a customer successfully joins the queue, the spawning flag should be reset.
                if (waitingLine.GetNumberOfCustomers() >= 6)
                {
                    isSpawningAllowed = false; // Stop spawning when the queue is full
                }
            }
            else
            {
                Debug.LogError("CustomerMovement script not found on the customer prefab!");
            }
        }
        else
        {
            Debug.LogError("Customer Prefab, Spawn Point, or Waiting Line is not assigned!");
        }
    }

    public void EnableSpawning()
    {
        isSpawningAllowed = true;
        StartCoroutine(SpawnCustomers());
        Debug.Log("Spawning is allowed again.");
    }



}