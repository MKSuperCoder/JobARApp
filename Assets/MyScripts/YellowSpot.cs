using UnityEngine;

public class YellowSpot : MonoBehaviour
{
    public ObjectDetection objectDetection;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("TrackedObject"))
        {
            objectDetection.OnYellowSpotReached();
        }
    }
}
