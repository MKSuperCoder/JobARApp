using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMovement : MonoBehaviour
{
    public Vector3 startPoint; // Starting point for the object's movement
    public float speed = 1f; // Speed of the object's movement
    public GameObject[] platePrefabs; // Array of different plate prefabs
    public float delayBetweenPlates = 1f; // Delay between spawning plates
    public GameObject yellowSpotPrefab; // Prefab for the yellow spot
    private Vector3 offset = new Vector3(0.1f, 0f, 0.1f); // Offset to move the yellow spot slightly away from the object

    private Vector3 targetPosition; // Target position for the object
    private GameObject currentPlate; // Reference to the currently spawned plate

    void Start()
    {
        // Initialize if needed
    }

    // Method to spawn a plate at the detected object's position
    public void SpawnPlateAtObjectPosition(Vector3 objectPosition, Vector3 initialTargetPosition, GameObject platePrefab)
    {
        startPoint = objectPosition; // Set the start position
        targetPosition = initialTargetPosition; // Set the initial target position
        Vector3 endPoint = CalculateEndPoint(targetPosition); // Calculate the end point based on the target position
        currentPlate = Instantiate(platePrefab, startPoint, Quaternion.Euler(-89.98f, 0, 0)); // Instantiate the plate
        currentPlate.tag = "TrackedPlate"; // Tag the plate for identification
        StartCoroutine(MoveCoroutine(currentPlate, startPoint, endPoint)); // Start the movement coroutine
    }

    // Method to update the target position for the plate
    public void UpdateTargetPosition(Vector3 newTargetPosition)
    {
        targetPosition = newTargetPosition; // Update the target position
        if (currentPlate != null)
        {
            StopAllCoroutines(); // Stop any ongoing coroutines
            StartCoroutine(MoveCoroutine(currentPlate, currentPlate.transform.position, CalculateEndPoint(newTargetPosition))); // Start a new movement coroutine
        }
    }

    // Coroutine to move the plate from start to end position smoothly
    IEnumerator MoveCoroutine(GameObject plate, Vector3 start, Vector3 end)
    {
        float distance = Vector3.Distance(start, end); // Calculate the distance between start and end points
        float startTime = Time.time; // Record the start time
        float duration = distance / speed; // Calculate the duration based on distance and speed

        Debug.Log("MoveCoroutine started. Distance: " + distance + ", Duration: " + duration);

        while (Time.time < startTime + duration)
        {
            float elapsed = Time.time - startTime; // Calculate elapsed time
            float t = elapsed / duration; // Calculate interpolation factor
            plate.transform.position = Vector3.Lerp(start, end, t); // Move the plate smoothly

            Debug.Log("Moving plate. Elapsed: " + elapsed + ", t: " + t + ", Position: " + plate.transform.position);

            yield return null; // Wait for the next frame
        }

        plate.transform.position = end; // Ensure the plate reaches the final position
        Debug.Log("MoveCoroutine completed. Final Position: " + plate.transform.position);

        Destroy(plate); // Destroy the plate after reaching the target
        yield return new WaitForSeconds(delayBetweenPlates); // Wait before spawning the next plate
        SpawnPlateAtObjectPosition(start, targetPosition, plate); // Spawn a new plate at the start position
    }

    // Method to calculate the end point with the offset applied
    Vector3 CalculateEndPoint(Vector3 start)
    {
        return start + offset; // Add the offset to the start position to get the end point
    }
}
