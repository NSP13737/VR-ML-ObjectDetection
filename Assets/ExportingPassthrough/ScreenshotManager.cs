using Mono.Cecil.Cil;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ScreenshotManager : MonoBehaviour
{
    private WebCamTexture webCamTexture;
    private Texture2D displayedImageTexture;
    public Renderer targetRenderer; // Drag and drop the Quad or Plane here in the Inspector
    [SerializeField] private Vector2Int desiredImgRes; //Image resolution to convert to (native is 640 by 480)



    void Start()
    {
        webCamTexture = new WebCamTexture();
        webCamTexture.Play();

        
        //Debug.Log("Default webcam resolution | Width: " + webCamTexture.width + " Height: " + webCamTexture.height);
    }

    void Update()
    {
        
        /*Texture2D capturedImage = DebugCaptureScreenshot();
        if (capturedImage != null)
        {
            DisplayOnRenderTexture(capturedImage);
            Destroy(capturedImage);
            //Debug.Log("Tried to display");
        }
        else
        {
            //Debug.Log("cap img is null");
        }*/
        
    }

    /// <summary>
    /// Captures the current frame from the webcam and returns it as a Texture2D.
    /// </summary>
    /// <returns>The captured Texture2D image, or null if the webcam is not playing.</returns>
    private Texture2D DebugCaptureScreenshot()
    {
        //Debug.Log("Webcam is playing " + webCamTexture.isPlaying);
        if (webCamTexture != null && webCamTexture.isPlaying)
        {
            Texture2D screenshot = new Texture2D(webCamTexture.width, webCamTexture.height);
            screenshot.SetPixels(webCamTexture.GetPixels());
            screenshot.Apply();
            

            // File.WriteAllBytes(Application.dataPath + "/../SavedScreen.png", imageBytes);

            //Debug.Log("Screenshot captured and returned.");
            return screenshot;
        }

        Debug.LogWarning("WebCamTexture is not playing or is null.");
        return null;
    }

    public byte[] CaptureScreenshot()
    {
        
        if (webCamTexture != null && webCamTexture.isPlaying)
        {
            /* // Code for resizing img from gemini
            // --- 2. Create a temporary RenderTexture for resizing using Graphics.Blit ---
            
            RenderTexture currentRT = RenderTexture.active; // Save the current active RenderTexture to restore it later            
            RenderTexture resizedRT = RenderTexture.GetTemporary(desiredImgRes.x, desiredImgRes.y, 0, RenderTextureFormat.ARGB32); // Create a new, temporary RenderTexture with the target dimensions
            Graphics.Blit(webCamTexture, resizedRT); // Use Graphics.Blit (uses GPU) to scale the webcam texture onto the new RenderTexture.

            // --- 3. Read the scaled pixels from the RenderTexture into a new Texture2D ---
            
            RenderTexture.active = resizedRT; // Set the resizedRT as the active render target
            Texture2D resizedTexture = new Texture2D(desiredImgRes.x, desiredImgRes.y, TextureFormat.RGB24, false); // Create a new Texture2D to hold the scaled pixels
            resizedTexture.ReadPixels(new Rect(0, 0, desiredImgRes.x, desiredImgRes.y), 0, 0); // Read the pixels from the active RenderTexture (resizedRT) into the Texture2D
            resizedTexture.Apply();

            // --- 4. Clean up the temporary RenderTexture and restore the active one ---
            RenderTexture.active = currentRT;
            RenderTexture.ReleaseTemporary(resizedRT);

            // --- 5. Encode the resized Texture2D to JPG ---
            // You can also adjust the quality (0-100) to reduce size further.
            // E.g., EncodeToJPG(75) for 75% quality.
            byte[] imageBytes = resizedTexture.EncodeToJPG();

            // --- 6. Clean up the resized Texture2D ---
            // It's crucial to destroy textures created at runtime to prevent memory leaks.
            Destroy(resizedTexture);

            // Optional: Log the new size for debugging
            Debug.Log($"Resized image to {desiredImgRes.x}x{desiredImgRes.y}. Total bytes: {imageBytes.Length}");

            return imageBytes;
        */




            //My original code
            Texture2D temporaryScreenshot = new Texture2D(webCamTexture.width, webCamTexture.height, TextureFormat.RGBA32, false);
            temporaryScreenshot.SetPixels(webCamTexture.GetPixels());
            temporaryScreenshot.Apply();

            //GEMINI
            // Ensure the persistent displayedImageTexture is correctly sized and assigned.
            // This check handles initial setup or if webcam resolution changes.
            if (displayedImageTexture == null || displayedImageTexture.width != temporaryScreenshot.width || displayedImageTexture.height != temporaryScreenshot.height)
            {
                if (displayedImageTexture != null)
                {
                    Destroy(displayedImageTexture); // Destroy the old persistent texture if resizing
                }
                displayedImageTexture = new Texture2D(temporaryScreenshot.width, temporaryScreenshot.height, TextureFormat.RGB24, false);
                targetRenderer.material.mainTexture = displayedImageTexture; // Re-assign if recreated
                Debug.Log("Persistent display texture re-initialized due to size mismatch or initial setup.");
            }

            // Copy pixels from the temporary screenshot to the persistent displayedImageTexture.
            // This ensures the renderer's texture remains valid even after temporaryScreenshot is destroyed.
            displayedImageTexture.SetPixels(temporaryScreenshot.GetPixels());
            displayedImageTexture.Apply(); // Apply pixel changes to the persistent display texture.

         
            byte[] imageBytes = temporaryScreenshot.EncodeToJPG();

            Destroy(temporaryScreenshot); // Important to prevent memory leak

            return imageBytes;
        }

        Debug.LogWarning("WebCamTexture is not playing or is null.");
        return null;
    }


    IEnumerator ExecuteAfterDelay(float delayInSeconds)
    {
        yield return new WaitForSeconds(delayInSeconds);
    }


}