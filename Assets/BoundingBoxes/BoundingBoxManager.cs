using UnityEngine;
using TMPro; // Make sure to add this using statement
using System.Collections.Generic;

public class BoundingBoxManager : MonoBehaviour
{
    [SerializeField] private GameObject boundingBoxPrefab;

    private GameObject currentBoxInstance;

    private List<GameObject> activeBoxes = new List<GameObject>();

    private List<DetectionResult> detections = new List<DetectionResult>();

    private void Start()
    {
        detections.Add(new DetectionResult
        {
            ClassName = "person",
            Confidence = 0.911696195602417f,
            XMin = 699.1796264648438f,
            YMin = 248.44227600097656f,
            XMax = 839.1951904296875f,
            YMax = 564.0f
        });

        // Second object: a person
        detections.Add(new DetectionResult
        {
            ClassName = "person",
            Confidence = 0.8872039914131165f,
            XMin = 549.8279418945312f,
            YMin = 332.7102966308594f,
            XMax = 625.3314819335938f,
            YMax = 544.033935546875f
        });

        // Third object: an elephant
        detections.Add(new DetectionResult
        {
            ClassName = "elephant",
            Confidence = 0.882445216178894f,
            XMin = 587.4811401367188f,
            YMin = 182.59332275390625f,
            XMax = 706.4411010742188f,
            YMax = 318.6602783203125f
        });

        // Fourth object: a person
        detections.Add(new DetectionResult
        {
            ClassName = "person",
            Confidence = 0.8703112602233887f,
            XMin = 173.59312438964844f,
            YMin = 227.5321044921875f,
            XMax = 256.4068298339844f,
            YMax = 526.1870727539062f
        });

        // Fifth object: a person
        detections.Add(new DetectionResult
        {
            ClassName = "person",
            Confidence = 0.8335622549057007f,
            XMin = 272.2511901855469f,
            YMin = 288.37445068359375f,
            XMax = 358.0374450683594f,
            YMax = 542.0066528320312f
        });

        // Sixth object: a person
        detections.Add(new DetectionResult
        {
            ClassName = "person",
            Confidence = 0.8164474964141846f,
            XMin = 245.52224731445312f,
            YMin = 301.875244140625f,
            XMax = 296.55987548828125f,
            YMax = 499.5787048339844f
        });

        // Seventh object: a person
        detections.Add(new DetectionResult
        {
            ClassName = "person",
            Confidence = 0.8156471848487854f,
            XMin = 116.97859191894531f,
            YMin = 250.91278076171875f,
            XMax = 191.30555725097656f,
            YMax = 516.7891845703125f
        });

        // Eighth object: an elephant
        detections.Add(new DetectionResult
        {
            ClassName = "elephant",
            Confidence = 0.742301881313324f,
            XMin = 243.16664123535156f,
            YMin = 151.23382568359375f,
            XMax = 367.0865783691406f,
            YMax = 241.177001953125f
        });

        // Ninth object: an elephant
        detections.Add(new DetectionResult
        {
            ClassName = "elephant",
            Confidence = 0.6554667353630066f,
            XMin = 220.70343017578125f,
            YMin = 170.63088989257812f,
            XMax = 293.80828857421875f,
            YMax = 240.57081604003906f
        });

        // Tenth object: a potted plant
        detections.Add(new DetectionResult
        {
            ClassName = "potted plant",
            Confidence = 0.6257315874099731f,
            XMin = 1.4457029104232788f,
            YMin = 363.3796691894531f,
            XMax = 109.8753433227539f,
            YMax = 510.4000244140625f
        });

        // Eleventh object: a handbag
        detections.Add(new DetectionResult
        {
            ClassName = "handbag",
            Confidence = 0.5272660851478577f,
            XMin = 115.80669403076172f,
            YMin = 296.2249450683594f,
            XMax = 172.86471557617188f,
            YMax = 391.0787353515625f
        });
    }

    private void Update()
    {
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

    // This method will be called to generate the boxes
    public void GenerateBoxes(int n)
    {
        ClearAllBoxes(); // First, clear any existing boxes

        for (int i = 0; i < n; i++)
        {
            // Calculate a random size and position for the new box
            Vector3 randomPosition = new Vector3(
                Random.Range(-5f, 5f),
                Random.Range(-5f, 5f),
                Random.Range(-5f, 5f));
            float randomSize = Random.Range(1f, 3f);

            // Instantiate the prefab
            GameObject newBox = Instantiate(boundingBoxPrefab, randomPosition, Quaternion.identity);

            // Set the scale of the box
            newBox.transform.localScale = Vector3.one * randomSize;

            // Get the TextMeshPro component from the instantiated object's children
            // You can also add a reference to it in your BoundingBox script for easier access
            TextMeshProUGUI textComponent = newBox.GetComponentInChildren<TextMeshProUGUI>();
            if (textComponent != null)
            {
                // Set the text to a descriptive string, like "Box #1"
                textComponent.text = "Box #" + (i + 1).ToString();
            }

            // Add the new box to your list
            activeBoxes.Add(newBox);
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