using Unity.Android.Gradle.Manifest;
using UnityEngine;
using TMPro;

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

    private void Update()
    {
        if (Camera.main != null)
        {
            // Make the text look at the camera.
            // This function automatically handles the up direction.
            textMeshPro.transform.LookAt(Camera.main.transform);

            // After looking at the camera, we need to flip it around so the text faces the camera.
            // The text's Z-axis is now pointing at the camera, so we rotate 180 degrees around the Y-axis.
            textMeshPro.transform.rotation *= Quaternion.Euler(0, 180, 0);
        }
    }

    public void Initialize(string className, float confidence, float x_min, float x_max, float y_min, float y_max)
    {
        // Set up the LineRenderer properties
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.useWorldSpace = true;
        lineRenderer.loop = false;

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

    }


    /// <summary>
    /// Updates the LineRenderer to draw a rectangular outline using 5 points.
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
        Vector3 corner1_local = new Vector3((x_min - centerOffset.x) * scaleOffset, localZOffset, (y_min - centerOffset.y) * scaleOffset); // Bottom-left
        Vector3 corner2_local = new Vector3((x_max - centerOffset.x) * scaleOffset, localZOffset, (y_min - centerOffset.y) * scaleOffset); // Bottom-right
        Vector3 corner3_local = new Vector3((x_max - centerOffset.x) * scaleOffset, localZOffset, (y_max - centerOffset.y) * scaleOffset); // Top-right
        Vector3 corner4_local = new Vector3((x_min - centerOffset.x) * scaleOffset, localZOffset, (y_max - centerOffset.y) * scaleOffset); // Top-left

        // 2. Transform the local points into WORLD coordinates using the parent's transform.
        // This is the key step that aligns the rectangle with the parent's rotation.
        Vector3 corner1_world = parentObject.transform.TransformPoint(corner1_local);
        Vector3 corner2_world = parentObject.transform.TransformPoint(corner2_local);
        Vector3 corner3_world = parentObject.transform.TransformPoint(corner3_local);
        topLeftCorner = corner1_world;
        Vector3 corner4_world = parentObject.transform.TransformPoint(corner4_local);

        // 3. Set the 5 points to draw a closed rectangle.
        // We manually close the loop by repeating the first point.
        Vector3[] points = new Vector3[5];
        points[0] = corner1_world;
        points[1] = corner2_world;
        points[2] = corner3_world;
        points[3] = corner4_world;
        points[4] = corner1_world; // Close the loop

        // 4. Set the positions for the LineRenderer.
        lineRenderer.positionCount = points.Length;
        lineRenderer.SetPositions(points);
    }


    /// <summary>
    /// Updates the position and content of the text display.
    /// </summary>
    ///

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
        // Note: The text is a child, so we use local coordinates.
        /*
        Vector3 textLocalPosition = new Vector3(
            (x_min - centerOffset.x) * scaleOffset,
            localZOffset + textOffsetFromBox, // Add a small vertical offset
            (y_max - centerOffset.y) * scaleOffset
        );
        */
        Vector3 textLocalPosition = topLeftCorner; // Getting the location that the bounding box verticie since the other implementation didn't work :(
        textLocalPosition.x += textOffsetFromBox.x;
        textLocalPosition.y += textOffsetFromBox.y;



        // Position the text
        textMeshPro.transform.localPosition = textLocalPosition;

        // Optional: Make the text face the camera
        //if (Camera.main != null)
        //{
           // textMeshPro.transform.rotation = Quaternion.LookRotation(textMeshPro.transform.position - Camera.main.transform.position);
        //}
    }
}