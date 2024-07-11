using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;
using UnityEngine.UI;
using GoodEnough.TextToSpeech;

public class ObjectDetection : MonoBehaviour
{
    public ARTrackedObjectManager objectManager; // Manages tracked AR objects
    public ARRaycastManager raycastManager; // Performs raycasting for AR

    public GameObject platePrefab; // Prefab of the 3D plate object
    public GameObject yellowSpotPrefab; // Prefab for the destination marker

    private GameObject spawnedPlate; // Reference to the spawned plate
    private GameObject yellowSpotInstance; // Reference to the yellow spot instance
    private List<GameObject> userSpawnedObjects = new List<GameObject>(); // List to store user spawned objects
    private List<ARRaycastHit> hits = new List<ARRaycastHit>(); // List to store raycast hits

    public TextMeshProUGUI infoText; // UI text for displaying information
    public ObjectMovement objectMovement; // Reference to the ObjectMovement script
    public TextMeshProUGUI objectText; // UI text for displaying object instructions

    void Start()
    {
        // Initialize the ObjectMovement reference
        objectMovement = GameObject.Find("ObjectMovement").GetComponent<ObjectMovement>();
        // Test text-to-speech functionality
        TTS.Speak("Text To Speech is working", "en-US");
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

    // Event handler for tracked objects changes
    void OnTrackedObjectsChanged(ARTrackedObjectsChangedEventArgs eventArgs)
    {
        // Handle added tracked objects
        foreach (var ARTrackedObject in eventArgs.added)
        {
            infoText.text = "This is a " + ARTrackedObject.referenceObject.name;
            infoText.gameObject.SetActive(true);
            TTS.Speak(infoText.text, "en-US");
            Debug.Log("Object Detected: " + ARTrackedObject.referenceObject.name);

            // Check if the detected object is a paper plate
            if (ARTrackedObject.referenceObject.name == "PaperPlate")
            {
                Vector3 objectPosition = ARTrackedObject.transform.position;
                Quaternion objectRotation = ARTrackedObject.transform.rotation;

                objectText.gameObject.SetActive(true);
                objectText.text = "Move the " + ARTrackedObject.referenceObject.name + " to the yellow spot";
                StartCoroutine(SpawnPlateAndYellowSpotAtObjectPosition(objectPosition));
                TTS.Speak(objectText.text, "en-US");
                // Assign the tag to the detected AR object
                ARTrackedObject.gameObject.tag = "TrackedObject";
            }
        }

        // Handle updated tracked objects
        foreach (var ARTrackedObject in eventArgs.updated)
        {
            Debug.Log("Tracked object updated " + ARTrackedObject.referenceObject.name);
            if (ARTrackedObject.referenceObject.name == "PaperPlate")
            {
                Vector3 objectPosition = ARTrackedObject.transform.position;
                if (spawnedPlate != null)
                {
                    spawnedPlate.transform.position = objectPosition;
                }
            }
        }

        // Handle removed tracked objects
        foreach (var ARTrackedObject in eventArgs.removed)
        {
            Debug.Log("Tracked object deleted: " + ARTrackedObject.referenceObject.name);
        }
    }

    void Update()
    {
        // Handle touch input for moving the yellow spot
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                if (raycastManager.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon))
                {
                    Pose hitPose = hits[0].pose;

                    if (yellowSpotInstance == null)
                    {
                        // Place the yellow spot if it doesn't exist
                        yellowSpotInstance = Instantiate(yellowSpotPrefab, hitPose.position, hitPose.rotation);
                    }
                    else
                    {
                        // Move the existing yellow spot
                        yellowSpotInstance.transform.position = hitPose.position;
                        yellowSpotInstance.transform.rotation = hitPose.rotation;

                        // Update the plate's target position
                        if (spawnedPlate != null)
                        {
                            objectMovement.UpdateTargetPosition(yellowSpotInstance.transform.position);
                        }
                    }
                }
            }
        }
    }

    // Coroutine to spawn the plate and yellow spot at the object's position
    IEnumerator SpawnPlateAndYellowSpotAtObjectPosition(Vector3 objectPosition)
    {
        // Wait until a plane is detected at the center of the screen
        while (!raycastManager.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), hits, TrackableType.PlaneWithinPolygon))
        {
            yield return null;
        }

        if (hits.Count > 0)
        {
            Vector3 tablePosition = hits[0].pose.position;

            if (yellowSpotInstance == null)
            {
                yellowSpotInstance = Instantiate(yellowSpotPrefab, tablePosition, Quaternion.identity);
            }

            objectMovement.SpawnPlateAtObjectPosition(objectPosition, yellowSpotInstance.transform.position, platePrefab);
        }
    }

    // Method to handle when the yellow spot is reached
    public void OnYellowSpotReached()
    {
        if (yellowSpotInstance != null)
        {
            Destroy(yellowSpotInstance);
        }
    }
}
