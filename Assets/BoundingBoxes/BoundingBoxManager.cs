using UnityEngine;
using TMPro; // Make sure to add this using statement
using System.Collections.Generic;

public class BoundingBoxManager : MonoBehaviour
{
    [SerializeField] private GameObject boundingBoxPrefab;

    private GameObject currentBoxInstance;

    private List<GameObject> activeBoxes = new List<GameObject>();

    private void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            manuallySpawnBox(0, 5, 0, 6);
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

    public void manuallySpawnBox(float x_min, float x_max, float y_min, float y_max)
    {
        string detectedCategory = "test";
        float detectionCertainty = 0f;
        currentBoxInstance = Instantiate(boundingBoxPrefab, transform.position, Quaternion.identity);
        ProceduralBoundingBox boxScript = currentBoxInstance.GetComponent<ProceduralBoundingBox>();
        boxScript.Initialize(x_min, x_max, y_min, y_max, detectedCategory, detectionCertainty);

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