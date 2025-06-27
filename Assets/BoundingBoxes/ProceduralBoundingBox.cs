using Unity.Android.Gradle.Manifest;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ProceduralBoundingBox: MonoBehaviour
{
    private LineRenderer lineRenderer;


    [SerializeField] private float lineWidth = 0.05f;

    [SerializeField] private Vector3 centerOffset = Vector3.zero;

    [SerializeField] private Material lineMaterial; // Assign a material in the Inspector

    [SerializeField] private string parentObjectName;

    private GameObject parentObject;

    [SerializeField] private float localZOffset = 0f;

    //private bool isInitialized = false;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        parentObject = GameObject.Find(parentObjectName);
    }

    public void Initialize(float x_min, float x_max, float y_min, float y_max, string category, float certainty)
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
        Vector3 corner1_local = new Vector3(x_min, localZOffset, y_min); // Bottom-left
        Vector3 corner2_local = new Vector3(x_max, localZOffset, y_min); // Bottom-right
        Vector3 corner3_local = new Vector3(x_max, localZOffset, y_max); // Top-right
        Vector3 corner4_local = new Vector3(x_min, localZOffset, y_max); // Top-left

        // 2. Transform the local points into WORLD coordinates using the parent's transform.
        // This is the key step that aligns the rectangle with the parent's rotation.
        Vector3 corner1_world = parentObject.transform.TransformPoint(corner1_local);
        Vector3 corner2_world = parentObject.transform.TransformPoint(corner2_local);
        Vector3 corner3_world = parentObject.transform.TransformPoint(corner3_local);
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
}