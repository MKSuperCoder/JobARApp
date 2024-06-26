using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMovement : MonoBehaviour
{
    public Vector3 startPoint;
    public float speed = 1f;
    public GameObject[] platePrefabs; // Array of different prefabs
    public float delayBetweenPlates = 1f;
    public GameObject yellowSpotPrefab;
    private Vector3 offset = new Vector3(0.1f, 0f, 0.1f); // Small offset to move the yellow spot a bit away from the object

    private Vector3 targetPosition;
    private GameObject currentPlate;

    void Start()
    {
        // Ensure initialization if needed
    }

    public void SpawnPlateAtObjectPosition(Vector3 objectPosition, Vector3 initialTargetPosition, GameObject platePrefab)
    {
        startPoint = objectPosition;
        targetPosition = initialTargetPosition;
        Vector3 endPoint = CalculateEndPoint(targetPosition); // Calculate end point based on table position
        currentPlate = Instantiate(platePrefab, startPoint, Quaternion.Euler(-89.98f, 0, 0));
        currentPlate.tag = "TrackedPlate"; // Add tag to the plate
        StartCoroutine(MoveCoroutine(currentPlate, startPoint, endPoint));
    }

    public void UpdateTargetPosition(Vector3 newTargetPosition)
    {
        targetPosition = newTargetPosition;
        if (currentPlate != null)
        {
            StopAllCoroutines();
            StartCoroutine(MoveCoroutine(currentPlate, currentPlate.transform.position, CalculateEndPoint(newTargetPosition)));
        }
    }

    IEnumerator MoveCoroutine(GameObject plate, Vector3 start, Vector3 end)
    {
        float distance = Vector3.Distance(start, end);
        float startTime = Time.time;
        float duration = distance / speed;

        Debug.Log("MoveCoroutine started. Distance: " + distance + ", Duration: " + duration);

        while (Time.time < startTime + duration)
        {
            float elapsed = Time.time - startTime;
            float t = elapsed / duration;
            plate.transform.position = Vector3.Lerp(start, end, t);

            Debug.Log("Moving plate. Elapsed: " + elapsed + ", t: " + t + ", Position: " + plate.transform.position);

            yield return null;
        }

        plate.transform.position = end;
        Debug.Log("MoveCoroutine completed. Final Position: " + plate.transform.position);

        Destroy(plate);
        yield return new WaitForSeconds(delayBetweenPlates);
        SpawnPlateAtObjectPosition(start, targetPosition, plate);
    }

    Vector3 CalculateEndPoint(Vector3 start)
    {
        return start + offset;
    }
}

