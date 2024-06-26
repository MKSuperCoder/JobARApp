using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;
using UnityEngine.UI;

public class ObjectDetection : MonoBehaviour
{
    public ARTrackedObjectManager objectManager;
    public ARRaycastManager raycastManager;

    public GameObject platePrefab; // Prefab of the 3D plate object
    public GameObject yellowSpotPrefab; // Prefab for the destination marker

    private GameObject spawnedPlate;
    private GameObject yellowSpotInstance; // Reference to the yellow spot instance
    private List<GameObject> userSpawnedObjects = new List<GameObject>();
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    public TextMeshProUGUI infoText;
    public ObjectMovement objectMovement;
    public TextMeshProUGUI objectText;



    void Start()
    {
        objectMovement = GameObject.Find("ObjectMovement").GetComponent<ObjectMovement>();

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
        foreach (var ARTrackedObject in eventArgs.added)
        {
            infoText.text = "This is a " + ARTrackedObject.referenceObject.name;
            infoText.gameObject.SetActive(true);
            Debug.Log("Object Detected: " + ARTrackedObject.referenceObject.name);

            if (ARTrackedObject.referenceObject.name == "PaperPlate")
            {
                Vector3 objectPosition = ARTrackedObject.transform.position;
                Quaternion objectRotation = ARTrackedObject.transform.rotation;

                objectText.gameObject.SetActive(true);
                objectText.text = "Move the " + ARTrackedObject.referenceObject.name + " to the yellow spot";
                StartCoroutine(SpawnPlateAndYellowSpotAtObjectPosition(objectPosition));

                // Assign the tag to the detected AR object
                ARTrackedObject.gameObject.tag = "TrackedObject";
            }
        }

        foreach (var ARTrackedObject in eventArgs.updated)
        {
            Debug.Log("Tracked object updated " + ARTrackedObject.referenceObject.name);
            if (ARTrackedObject.referenceObject.name == "PaperPlate")
            {
                Vector3 objectPosition = ARTrackedObject.transform.position;
                spawnedPlate.transform.position = objectPosition;
            }
        }

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

    IEnumerator SpawnPlateAndYellowSpotAtObjectPosition(Vector3 objectPosition)
    {
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

    public void OnYellowSpotReached()
    {
        if (yellowSpotInstance != null)
        {
            Destroy(yellowSpotInstance);
        }
    }
}