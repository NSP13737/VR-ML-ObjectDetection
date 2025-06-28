using System.Collections;
using System.IO;
using UnityEngine;

public class ScreenshotManager : MonoBehaviour
{
    private WebCamTexture webCamTexture;
    public Renderer targetRenderer; // Drag and drop the Quad or Plane here in the Inspector

    byte[] imageBytes = null;


  void Start()
    {
        webCamTexture = new WebCamTexture();
        webCamTexture.Play();
    }

    void Update()
    {
        
        Texture2D capturedImage = CaptureScreenshot();
        if (capturedImage != null)
        {
            DisplayOnRenderTexture(capturedImage);
        }
        
    }

    /// <summary>
    /// Captures the current frame from the webcam and returns it as a Texture2D.
    /// </summary>
    /// <returns>The captured Texture2D image, or null if the webcam is not playing.</returns>
    Texture2D CaptureScreenshot()
    {
        //Debug.Log("Webcam is playing " + webCamTexture.isPlaying);
        if (webCamTexture != null && webCamTexture.isPlaying)
        {
            Texture2D screenshot = new Texture2D(webCamTexture.width, webCamTexture.height);
            screenshot.SetPixels(webCamTexture.GetPixels());
            screenshot.Apply();

            imageBytes = screenshot.EncodeToPNG();

            // File.WriteAllBytes(Application.dataPath + "/../SavedScreen.png", imageBytes);

            //Debug.Log("Screenshot captured and returned.");
            return screenshot;
        }

        Debug.LogWarning("WebCamTexture is not playing or is null.");
        return null;
    }

    public byte[] GetScreenshotBytes()
    {

        return imageBytes; 
    
    }

    /// <summary>
        /// Displays the given Texture2D on a target Renderer's material.
        /// </summary>
        /// <param name="image">The Texture2D to display.</param>
    void DisplayOnRenderTexture(Texture2D image)
    {
        if (targetRenderer != null && image != null)
        {
            // Check if the material has a main texture property
            if (targetRenderer.material.HasProperty("_MainTex"))
            {
                targetRenderer.material.mainTexture = image;
                //Debug.Log("Image displayed on render texture.");
            }
            else
            {
                Debug.LogWarning("Target renderer's material does not have a '_MainTex' property.");
            }
        }
        else
        {
            Debug.LogWarning("Target renderer or image is null. Cannot display texture.");
        }
    }
}