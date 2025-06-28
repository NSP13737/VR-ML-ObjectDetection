using UnityEngine;
using TMPro; // Make sure to add this using statement
using System.Collections.Generic;

public class BoundingBoxManager : MonoBehaviour
{
    [SerializeField] private GameObject boundingBoxPrefab;
    [SerializeField] private UDPServer udpServer;

    private GameObject currentBoxInstance;

    private List<GameObject> activeBoxes = new List<GameObject>();

    private List<DetectionResult> detections = new List<DetectionResult>();

    private void Start()
    {
       
    }

    private void Update()
    {
        //UNCOMMENT FOR NETWORKING
        //detections = udpServer.GetDetectionResults(); // Ask the UDP Server script if it has a new set of bounding boxes for us to draw
        if (Input.GetKeyDown("space"))
        {
            //manuallySpawnBox(0, 5, 0, 6);
            foreach (DetectionResult detection in detections)
            {
                manuallySpawnBox(detection.ClassName, detection.Confidence, detection.XMin, detection.XMax, detection.YMin, detection.YMax);
                Debug.Log("Created box");
            }
            Debug.Log(activeBoxes.Count);
        }
        if (Input.GetKeyDown("d"))
        {
            ClearAllBoxes();
            Debug.Log(activeBoxes.Count);
        }
    }

    

    public void manuallySpawnBox(string className, float confidence, float x_min, float x_max, float y_min, float y_max)
    {
        currentBoxInstance = Instantiate(boundingBoxPrefab, transform.position, Quaternion.identity);
        ProceduralBoundingBox boxScript = currentBoxInstance.GetComponent<ProceduralBoundingBox>();
        boxScript.Initialize(className, confidence, x_min, x_max, y_min, y_max);

        activeBoxes.Add(currentBoxInstance);
    }
    public void ClearAllBoxes()
    {
        foreach (GameObject box in activeBoxes)
        {
            Destroy(box);
        }
        activeBoxes.Clear();
    }
}