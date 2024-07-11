using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;
using GoodEnough.TextToSpeech;

public class SinkDetection : MonoBehaviour
{
    public ARTrackedObjectManager objectManager; // Manages tracked AR objects
    public TextMeshProUGUI infoText; // UI text for displaying information about detected objects
    public TextMeshProUGUI instructionText; // UI text for displaying instructions to the user

    public GameObject yellowPrefab; // Prefab for the yellow marker object
    public GameObject cubePrefab; // Prefab for the cube
    public Transform trashCan; // Reference to the trash can's position

    private List<GameObject> detectedTissues = new List<GameObject>(); // List to store detected tissue objects
    private List<GameObject> spawnedYellowMarkers = new List<GameObject>(); // List to store spawned yellow markers

    private CubeAnimator cubeAnimator; // Reference to the CubeAnimator script

    void Start()
    {
        instructionText.text = ""; // Clear any existing instructions
        cubeAnimator = new CubeAnimator(); // Initialize the CubeAnimator
    }

    void OnEnable()
    {
        // Subscribe to the trackedObjectsChanged event
        objectManager.trackedObjectsChanged += OnTrackedObjectsChanged;
    }

    void OnDisable()
    {
        // Unsubscribe from the trackedObjectsChanged event
        objectManager.trackedObjectsChanged -= OnTrackedObjectsChanged;
    }

    // Event handler for when tracked objects change
    void OnTrackedObjectsChanged(ARTrackedObjectsChangedEventArgs eventArgs)
    {
        // Handle added tracked objects
        foreach (var trackedObject in eventArgs.added)
        {
            infoText.text = "Object detected: " + trackedObject.referenceObject.name;
            TTS.Speak(infoText.text, "en-US");

            // Check if the detected object is a tissue
            if (IsTissue(trackedObject.referenceObject.name))
            {
                instructionText.text = "Throw the tissues into the trash bin";
                TTS.Speak(instructionText.text, "en-US");
                Vector3 tissuePosition = trackedObject.transform.position;
                detectedTissues.Add(trackedObject.gameObject); // Add the tissue to the list
                SpawnYellowObject(tissuePosition); // Spawn a yellow marker at the tissue's position
                SpawnAndMoveCube(tissuePosition); // Spawn and move the cube towards the tissue
            }
        }

        // Handle removed tracked objects (currently commented out)
        /*
        foreach (var trackedObject in eventArgs.removed)
        {
            if (IsTissue(trackedObject.referenceObject.name))
            {
                instructionText.text = "Well done! You've removed the tissue!";
                TTS.Speak(instructionText.text);
                RemoveDetectedTissue(trackedObject.gameObject); // Remove the detected tissue
            }
        }
        */

        UpdateInstructionText(); // Update the instruction text based on the current state
    }

    // Helper method to determine if an object is a tissue
    bool IsTissue(string objectName)
    {
        return objectName.StartsWith("Tissue");
    }

    // Method to spawn a yellow marker at a given position
    void SpawnYellowObject(Vector3 position)
    {
        GameObject yellowMarker = Instantiate(yellowPrefab, position, Quaternion.identity);
        spawnedYellowMarkers.Add(yellowMarker); // Add the yellow marker to the list
    }

    // Method to spawn and move a cube to the trash can
    void SpawnAndMoveCube(Vector3 position)
    {
        GameObject cube = Instantiate(cubePrefab, position, Quaternion.identity);
        Vector3 liftPosition = position + Vector3.up * 0.5f; // Lift the cube slightly upwards
        Vector3 trashPosition = trashCan.position; // Get the position of the trash can
        StartCoroutine(cubeAnimator.MoveCubeToTrashCan(cube, liftPosition, trashPosition, 1f)); // Start the movement coroutine
    }

    // Method to remove a detected tissue and its corresponding yellow marker
    void RemoveDetectedTissue(GameObject tissue)
    {
        for (int i = detectedTissues.Count - 1; i >= 0; i--)
        {
            if (detectedTissues[i] == tissue)
            {
                Destroy(detectedTissues[i]); // Destroy the tissue object
                detectedTissues.RemoveAt(i); // Remove the tissue from the list

                Destroy(spawnedYellowMarkers[i]); // Destroy the corresponding yellow marker
                spawnedYellowMarkers.RemoveAt(i); // Remove the yellow marker from the list
            }
        }
    }

    // Method to update the instruction text based on the current state
    void UpdateInstructionText()
    {
        if (detectedTissues.Count > 0)
        {
            instructionText.text = "Throw the tissues into the trash bin"; // Instruction if there are tissues detected
        }
        /*
        else
        {
            instructionText.text = "Well done! You've removed all the tissues!"; // Instruction if all tissues are removed
        }
        */
    }

    void Update()
    {
        // Update method can be used for additional updates if needed
    }
}
