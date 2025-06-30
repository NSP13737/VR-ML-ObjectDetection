using UnityEngine;

public class TextureAlphaControll : MonoBehaviour
{
    // Use [SerializeField] to expose this private variable in the Inspector.
    // The default value is 1.0f (fully opaque).
    [SerializeField]
    private float alphaValue = 1.0f;

    private Material objectMaterial; // Reference to the material we'll be modifying

    void Awake()
    {
        // Get the material from the Renderer component attached to this GameObject.
        // .material creates a unique instance, so you don't modify the original asset.
        Renderer objectRenderer = GetComponent<Renderer>();
        if (objectRenderer != null)
        {
            objectMaterial = objectRenderer.material;
        }
        else
        {
            Debug.LogError("TransparencyController: No Renderer found on this GameObject.", this);
            enabled = false; // Disable the script if no Renderer is found
        }
    }

    void Update()
    {
        // Only update if the material exists
        if (objectMaterial != null)
        {
            // Clamp the alphaValue between 0 and 1 to ensure valid transparency.
            float clampedAlpha = Mathf.Clamp01(alphaValue);

            // Get the current color of the material
            Color currentColor = objectMaterial.color;

            // Set the alpha component of the color
            currentColor.a = clampedAlpha;

            // Apply the new color back to the material
            objectMaterial.color = currentColor;
        }
    }
}