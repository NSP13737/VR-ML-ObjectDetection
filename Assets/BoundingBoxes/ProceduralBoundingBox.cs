using Unity.Android.Gradle.Manifest;
using UnityEngine;
using TMPro;
using static UnityEditor.PlayerSettings;

[RequireComponent(typeof(LineRenderer))]
public class ProceduralBoundingBox : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private TextMeshPro textMeshPro;


    [SerializeField] private float lineWidth = 0.05f;

    [SerializeField] private Vector2 centerOffset = Vector2.zero;

    [SerializeField] private Material lineMaterial; // Assign a material in the Inspector

    [SerializeField] private string parentObjectName;

    private GameObject parentObject;

    [SerializeField] private float localZOffset = 0f;
    [SerializeField] private float scaleOffset = 1f;

    [Header("Text Settings")]
    [SerializeField] private GameObject textPrefab; // Assign your TextMeshPro prefab here
    [SerializeField] private Vector2 textOffsetFromBox = Vector2.zero; // Offset the text from the top-left corner
    [SerializeField] private float textSize = 1f;

    private Vector3 topLeftCorner; //used for text placement



    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        parentObject = GameObject.Find(parentObjectName);

        GameObject textInstance = Instantiate(textPrefab, transform);
        textMeshPro = textInstance.GetComponent<TextMeshPro>();
    }

    // This method is called to initialize the bounding box.
    public void Initialize(string className, float confidence, float x_min, float x_max, float y_min, float y_max)
    {

        // Set up the LineRenderer properties
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.useWorldSpace = false; // <--- KEY CHANGE: Set to local space
        lineRenderer.loop = true; // <--- KEY CHANGE: Let the LineRenderer close the loop

        // Assign the material
        if (lineMaterial != null)
        {
            lineRenderer.material = lineMaterial;
        }
        else
        {
            Debug.LogWarning("Line material is not assigned. Using default.");
        }

        UpdateLineRenderer(x_min, x_max, y_min, y_max);
        UpdateText(className, confidence, x_min, y_max);

        transform.position = parentObject.GetComponent<Transform>().position;
        transform.rotation = parentObject.GetComponent<Transform>().rotation;
        transform.Rotate(-90f, 0f, 0f);

    }


    /// <summary>
    /// Updates the LineRenderer to draw a rectangular outline using 4 points.
    /// The rectangle's corners are defined by the provided min/max x and y coordinates.
    /// The depth (Z-coordinate) is based on the parent object's position.
    /// </summary>
    public void UpdateLineRenderer(float x_min, float x_max, float y_min, float y_max)
    {
        // Check if the parent object is assigned
        if (parentObject == null)
        {
            Debug.LogWarning("parentObject is not assigned.");
        }

        // 1. Define the 4 corner points in the parent's LOCAL coordinate space.
        // We use localZOffset for the Z-coordinate.
        Vector3 corner1_local = new Vector3((x_min - centerOffset.x) * scaleOffset, (y_min - centerOffset.y) * scaleOffset, localZOffset); // Bottom-left
        Vector3 corner2_local = new Vector3((x_max - centerOffset.x) * scaleOffset, (y_min - centerOffset.y) * scaleOffset, localZOffset); // Bottom-right
        Vector3 corner3_local = new Vector3((x_max - centerOffset.x) * scaleOffset, (y_max - centerOffset.y) * scaleOffset, localZOffset); // Top-right
        Vector3 corner4_local = new Vector3((x_min - centerOffset.x) * scaleOffset, (y_max - centerOffset.y) * scaleOffset, localZOffset); // Top-left

        topLeftCorner = corner4_local;

        // 3. Set the 4 points to draw the rectangle.
        // The LineRenderer.loop = true property will close the loop for us.
        Vector3[] points = new Vector3[4]; // <--- KEY CHANGE: Only 4 points needed
        points[0] = corner1_local;
        points[1] = corner2_local;
        points[2] = corner3_local;
        points[3] = corner4_local;

        // 4. Set the positions for the LineRenderer.
        lineRenderer.positionCount = points.Length;
        lineRenderer.SetPositions(points);
    }


    /// <summary>
    /// Updates the position and content of the text display.
    /// </summary>
    public void UpdateText(string className, float confidence, float x_min, float y_max)
    {
        if (textMeshPro == null)
        {
            return; // Exit if the text component is not assigned or found
        }

        // Set the text content, formatting the confidence to a percentage
        textMeshPro.text = $"{className}\nConfidence: {confidence:P1}"; // P1 formats to percentage with one decimal place
        textMeshPro.fontSize = textSize;

        // Calculate the local position for the text
        // We'll place it slightly above the top-left corner of the bounding box.
        Vector3 textLocalPosition = topLeftCorner;
        textLocalPosition.x += textOffsetFromBox.x;
        textLocalPosition.y += textOffsetFromBox.y;


        // Position the text
        textMeshPro.transform.localPosition = textLocalPosition;

        // Optional: Make the text face the camera
        //if (Camera.main != null)
        //{
        //    textMeshPro.transform.rotation = Quaternion.LookRotation(textMeshPro.transform.position - Camera.main.transform.position);
        //}
    }
}