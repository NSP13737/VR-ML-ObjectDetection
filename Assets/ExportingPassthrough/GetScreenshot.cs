using System.Collections;
using System.IO;
using UnityEngine;

public class GetScreenshot : MonoBehaviour
{
    private WebCamTexture webCamTexture;

    void Start()
    {
        webCamTexture = new WebCamTexture();
        webCamTexture.Play();
    }

    void Update()
    {
        // Check for spacebar press
        if (Input.GetKeyDown(KeyCode.Space))
        {
            foreach (WebCamDevice device in WebCamTexture.devices)
            {
                Debug.Log(device.name);
            }
            CaptureScreenshot();
        }
    }

    void CaptureScreenshot()
    {
        if (webCamTexture != null && webCamTexture.isPlaying)
        {
            Texture2D screenshot = new Texture2D(webCamTexture.width, webCamTexture.height);
            screenshot.SetPixels(webCamTexture.GetPixels());
            screenshot.Apply();

            byte[] imageBytes = screenshot.EncodeToPNG();
            File.WriteAllBytes(Application.dataPath + "/../SavedScreen.png", imageBytes);
            // Example placeholder — send imageBytes via UDP here
            Debug.Log("Screenshot captured and ready for UDP transmission.");
        }
    }
}