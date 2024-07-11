using UnityEngine;

public class YellowSpot : MonoBehaviour
{
    public ObjectDetection objectDetection; // Reference to the ObjectDetection script

    private void Start()
    {
        // Find the ObjectDetection component in the scene and assign it
        objectDetection = GameObject.Find("ObjectDetection").GetComponent<ObjectDetection>();
    }

    // This method is called when another collider enters the trigger collider attached to this object
    void OnTriggerEnter(Collider other)
    {
        // Check if the collider belongs to an object with the "TrackedObject" tag
        if (other.gameObject.CompareTag("TrackedObject"))
        {
            // Call the OnYellowSpotReached method in the ObjectDetection script
            objectDetection.OnYellowSpotReached();
        }
    }
}
