using UnityEngine;
using TMPro; // Make sure to add this using statement
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class BoundingBoxManager : MonoBehaviour
{
    [SerializeField] private GameObject boundingBoxPrefab;
    [SerializeField] private TCPServer tcpServer;

    private GameObject currentBoxInstance;

    private List<GameObject> activeBoxes = new List<GameObject>();

    private List<DetectionResult> previousDetections = new List<DetectionResult>();
    private List<DetectionResult> detections = new List<DetectionResult>();

    private void Start()
    {
        //Debug.Log(detections);
    }

    private void Update()
    {
        
        detections = tcpServer.GetDetectionResults(); // Ask the TCP Server script if it has a new set of bounding boxes for us to draw


        //only draw new detection boxes on a frame when we get new results (don't want to keep drawing the same result multiple times)
        if ((isNewDetection(detections, previousDetections)))
        {
            ClearAllBoxes();
            foreach (DetectionResult detection in detections)
            {
                manuallySpawnBox(detection.ClassName, detection.Confidence, detection.XMin, detection.XMax, detection.YMin, detection.YMax);
            }
            previousDetections = detections;
        }
        else
        {
            //Debug.Log("Got same detection list OR detections is null");
        }


        //CODE FOR MANUALLY TESTING
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

    

    private void manuallySpawnBox(string className, float confidence, float x_min, float x_max, float y_min, float y_max)
    {
        currentBoxInstance = Instantiate(boundingBoxPrefab, transform.position, Quaternion.identity);
        ProceduralBoundingBox boxScript = currentBoxInstance.GetComponent<ProceduralBoundingBox>();
        boxScript.Initialize(className, confidence, x_min, x_max, y_min, y_max);

        activeBoxes.Add(currentBoxInstance);
    }
    private void ClearAllBoxes()
    {
        foreach (GameObject box in activeBoxes)
        {
            Destroy(box);
        }
        activeBoxes.Clear();
    }


    /// <summary>
    /// Checks if both lists are longer than 0, then checks the confidence of the first detected object. Since this is a float, the chances of this being the same between two frames are miniscule
    /// </summary>
    private bool isNewDetection(List<DetectionResult> currentResult, List<DetectionResult> previousResult)
    {

        if ((currentResult.Count != 0) && (previousResult.Count == 0)) //Special case: this should only be called on the first frame
        {
            return true;
        }
        if ((currentResult.Count == 0) && (previousResult.Count == 0))
        {
            return false;
        }
        else if (currentResult[0].Confidence == previousResult[0].Confidence) 
        {
            return false;
        }
        else
        {
            return true;
        }
    }

}

