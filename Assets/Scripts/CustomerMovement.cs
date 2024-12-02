using UnityEngine;
using System.Collections;

public class CustomerMovement : MonoBehaviour
{
    public Transform[] lineSpots;
    public int currentSpotIndex = -1;
    private bool isMoving = false;
    public float timer = 35f;
    private bool isTimerActive = false;
    private int spotIndex = -1;

    public void SetAssignedSpotIndex(int index)
    {
        spotIndex = index;
    }

    public int GetAssignedSpotIndex()
    {
        return spotIndex;
    }

    private void Start()
    {
        WaitingLine waitingLine = FindObjectOfType<WaitingLine>();

        if (waitingLine != null)
        {
            lineSpots = waitingLine.GetLineSpots(); // Initialize line spots
            Debug.Log("Line spots initialized: " + lineSpots.Length);
        }
        else
        {
            Debug.LogError("WaitingLine script not found in the scene.");
        }

        StartTimer(); // Start the timer when the customer joins the line
    }

    public void MoveToSpot(Vector3 targetPosition, int index)
    {
        WaitingLine waitingLine = FindObjectOfType<WaitingLine>();
        waitingLine.spotOccupied[index] = false;
        currentSpotIndex = index;
        spotIndex = index;
        StartCoroutine(MoveToPosition(targetPosition));
        waitingLine.spotOccupied[index] = true;
    }

    private IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        float elapsedTime = 0f;
        float duration = 2f;
        Vector3 startingPosition = transform.position;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startingPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
    }

    private void StartTimer()
    {
        if (!isTimerActive)
        {
            isTimerActive = true;
            StartCoroutine(TimerCountdown());
        }
    }

    private IEnumerator TimerCountdown()
    {
        while (timer > 0f && isTimerActive)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        if (timer <= 0f)
        {
            MoveToLeaveZone(false);
        }
    }

    public void MoveToLeaveZone(bool isHappy)
    {
        GameObject leaveZone = GameObject.FindGameObjectWithTag("LeaveZone");
        if (leaveZone != null)
        {
            Vector3 targetPosition = leaveZone.transform.position;
            StartCoroutine(MoveAndFreeSpot(targetPosition, isHappy));
        }
        else
        {
            Debug.LogError("LeaveZone not found in the scene!");
        }
    }

    private IEnumerator MoveAndFreeSpot(Vector3 targetPosition, bool isHappy)
    {
        WaitingLine waitingLine = FindObjectOfType<WaitingLine>();
        if (waitingLine != null)
        {
            waitingLine.FreeSpot(currentSpotIndex); // Free the spot when the customer leaves
        }

        if (isHappy)
        {
            Debug.Log("Customer leaves happily!");
        }
        else
        {
            Debug.Log("Customer leaves angrily!");
        }

        yield return StartCoroutine(MoveTowardsLeaveZone(targetPosition));
    }

    public IEnumerator MoveTowardsLeaveZone(Vector3 targetPosition)
    {
        // Step 1: Move towards target Y coordinate first
        while (Mathf.Abs(transform.position.y - targetPosition.y) > 0.1f)  // Move towards target Y first
        {
            float newY = Mathf.MoveTowards(transform.position.y, targetPosition.y, 2f * Time.deltaTime);
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
            yield return null;
        }

        // Step 2: Move towards target X coordinate after Y is aligned
        while (Mathf.Abs(transform.position.x - targetPosition.x) > 0.1f)  // Then move towards target X
        {
            float newX = Mathf.MoveTowards(transform.position.x, targetPosition.x, 2f * Time.deltaTime);
            transform.position = new Vector3(newX, targetPosition.y, transform.position.z);  // Keep Y fixed
            yield return null;
        }

        // Final position reached, destroy the customer
        Destroy(gameObject);  // Destroy the customer prefab
    }


    public int GetCurrentSpotIndex()
    {
        return currentSpotIndex; // Replace with the actual field or property that stores the spot index.
    }



}
