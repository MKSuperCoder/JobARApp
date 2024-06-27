using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;

public class SinkDetection : MonoBehaviour
{
    public ARTrackedObjectManager objectManager;
    public TextMeshProUGUI infoText;
    public TextMeshProUGUI instructionText;

    public GameObject yellowPrefab; // Prefab for the yellow object
    public GameObject trashCanPrefab; // Prefab for the trash can

    private List<GameObject> detectedTissues = new List<GameObject>();
    private List<GameObject> spawnedYellowMarkers = new List<GameObject>();

    private GameObject trashCanInstance; // Instance of the spawned trash can

    void Start()
    {
        instructionText.text = "";
    }

    void OnEnable()
    {
        objectManager.trackedObjectsChanged += OnTrackedObjectsChanged;
    }

    void OnDisable()
    {
        objectManager.trackedObjectsChanged -= OnTrackedObjectsChanged;
    }

    void OnTrackedObjectsChanged(ARTrackedObjectsChangedEventArgs eventArgs)
    {
        foreach (var trackedObject in eventArgs.added)
        {
            infoText.text = "Object detected: " + trackedObject.referenceObject.name;
            if (IsTissue(trackedObject.referenceObject.name))
            {
                instructionText.text = "Throw the tissues into the trash bin";

                Vector3 tissuePosition = trackedObject.transform.position;
                detectedTissues.Add(trackedObject.gameObject);
                SpawnYellowObject(tissuePosition);

                // Spawn the trash can if it's not already spawned
                if (trashCanInstance == null)
                {
                    SpawnTrashCan(tissuePosition);
                }
            }
        }

        foreach (var trackedObject in eventArgs.removed)
        {
            if (IsTissue(trackedObject.referenceObject.name))
            {
                instructionText.text = "Well done! You've removed the tissue!";
                RemoveDetectedTissue(trackedObject.gameObject);
            }
        }

        UpdateInstructionText();
    }

    bool IsTissue(string objectName)
    {
        return objectName.StartsWith("Tissue");
    }

    void SpawnYellowObject(Vector3 position)
    {
        GameObject yellowMarker = Instantiate(yellowPrefab, position, Quaternion.identity);
        spawnedYellowMarkers.Add(yellowMarker);
    }

    void RemoveDetectedTissue(GameObject tissue)
    {
        for (int i = detectedTissues.Count - 1; i >= 0; i--)
        {
            if (detectedTissues[i] == tissue)
            {
                Destroy(detectedTissues[i]);
                detectedTissues.RemoveAt(i);

                Destroy(spawnedYellowMarkers[i]);
                spawnedYellowMarkers.RemoveAt(i);
            }
        }
    }

    void UpdateInstructionText()
    {
        if (detectedTissues.Count > 0)
        {
            instructionText.text = "Throw the tissues into the trash bin";
        }
        else
        {
            instructionText.text = "Well done! You've removed all the tissues!";
        }
    }

    void SpawnTrashCan(Vector3 position)
    {
        // Adjust the position to place the trash can at a suitable location
        Vector3 trashCanPosition = position + new Vector3(0.5f, 0, 0); // Example offset, adjust as needed
        trashCanInstance = Instantiate(trashCanPrefab, trashCanPosition, Quaternion.identity);
    }

    void Update()
    {
    }
}
