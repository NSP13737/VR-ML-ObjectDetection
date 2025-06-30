using System; // Adam & Unity
using System.Collections; //Adam & Unity
using System.Collections.Generic; // Adam & Unity
using System.Net.Sockets; //Adam & Unity
using System.Net; // Adam & Unity
using System.Text; //Adam & Unity
using UnityEngine;
using UnityEngine.InputSystem.XR; // Unity
using Newtonsoft.Json;

public class TCPServer : MonoBehaviour
{
    // ML perception
    public float transmissionFrequency = 0.1f;
    List<DetectionResult> detectionResults;
    // Networking
    private string serverIP = "127.0.0.1";
    private int serverPort = 5005;
    private TcpClient tcpClient;
    private NetworkStream stream;
    private bool isConnected;
    [SerializeField] private ScreenshotManager screenshotManager;

    private List<DetectionResult> detectionsTest = new List<DetectionResult>();


    private void Start()
    {
        // Set up TCP client
        StartCoroutine(ConnectToServer());


        detectionsTest.Add(new DetectionResult
        {
            ClassName = "person",
            Confidence = 0.911696195602417f,
            XMin = 699.1796264648438f,
            YMin = 248.44227600097656f,
            XMax = 839.1951904296875f,
            YMax = 564.0f
        });

        // Second object: a person
        detectionsTest.Add(new DetectionResult
        {
            ClassName = "person",
            Confidence = 0.8872039914131165f,
            XMin = 549.8279418945312f,
            YMin = 332.7102966308594f,
            XMax = 625.3314819335938f,
            YMax = 544.033935546875f
        });

        // Third object: an elephant
        detectionsTest.Add(new DetectionResult
        {
            ClassName = "elephant",
            Confidence = 0.882445216178894f,
            XMin = 587.4811401367188f,
            YMin = 182.59332275390625f,
            XMax = 706.4411010742188f,
            YMax = 318.6602783203125f
        });

        // Fourth object: a person
        detectionsTest.Add(new DetectionResult
        {
            ClassName = "person",
            Confidence = 0.8703112602233887f,
            XMin = 173.59312438964844f,
            YMin = 227.5321044921875f,
            XMax = 256.4068298339844f,
            YMax = 526.1870727539062f
        });

        // Fifth object: a person
        detectionsTest.Add(new DetectionResult
        {
            ClassName = "person",
            Confidence = 0.8335622549057007f,
            XMin = 272.2511901855469f,
            YMin = 288.37445068359375f,
            XMax = 358.0374450683594f,
            YMax = 542.0066528320312f
        });

        // Sixth object: a person
        detectionsTest.Add(new DetectionResult
        {
            ClassName = "person",
            Confidence = 0.8164474964141846f,
            XMin = 245.52224731445312f,
            YMin = 301.875244140625f,
            XMax = 296.55987548828125f,
            YMax = 499.5787048339844f
        });

        // Seventh object: a person
        detectionsTest.Add(new DetectionResult
        {
            ClassName = "person",
            Confidence = 0.8156471848487854f,
            XMin = 116.97859191894531f,
            YMin = 250.91278076171875f,
            XMax = 191.30555725097656f,
            YMax = 516.7891845703125f
        });

        // Eighth object: an elephant
        detectionsTest.Add(new DetectionResult
        {
            ClassName = "elephant",
            Confidence = 0.742301881313324f,
            XMin = 243.16664123535156f,
            YMin = 151.23382568359375f,
            XMax = 367.0865783691406f,
            YMax = 241.177001953125f
        });

        // Ninth object: an elephant
        detectionsTest.Add(new DetectionResult
        {
            ClassName = "elephant",
            Confidence = 0.6554667353630066f,
            XMin = 220.70343017578125f,
            YMin = 170.63088989257812f,
            XMax = 293.80828857421875f,
            YMax = 240.57081604003906f
        });

        // Tenth object: a potted plant
        detectionsTest.Add(new DetectionResult
        {
            ClassName = "potted plant",
            Confidence = 0.6257315874099731f,
            XMin = 1.4457029104232788f,
            YMin = 363.3796691894531f,
            XMax = 109.8753433227539f,
            YMax = 510.4000244140625f
        });

        // Eleventh object: a handbag
        detectionsTest.Add(new DetectionResult
        {
            ClassName = "handbag",
            Confidence = 0.5272660851478577f,
            XMin = 115.80669403076172f,
            YMin = 296.2249450683594f,
            XMax = 172.86471557617188f,
            YMax = 391.0787353515625f
        });
    }

    private IEnumerator ConnectToServer()
    {
        while (true)
        {
            // Attempt connection
            bool connectionSuccessful = AttemptConnection();

            if (connectionSuccessful)
            {
                isConnected = true;
                Debug.Log($"Connected to Python server on port {serverPort}");
                StartCoroutine(TransmitPerception());
                
                break;
            }
            else
            {
                isConnected = false;
                Debug.LogWarning($"Failed to connect to server on port {serverPort}. Retrying in 2 seconds...");
                yield return new WaitForSeconds(2f);
            }
        }
    }

    private bool AttemptConnection()
    {
        try
        {
            tcpClient = new TcpClient();
            IAsyncResult connectAsync = tcpClient.BeginConnect(serverIP, serverPort, null, null);

            // Poll for connection completion (avoid yield in try/catch)
            float timeout = 5f; // Timeout in seconds
            float startTime = Time.realtimeSinceStartup;
            while (!connectAsync.IsCompleted && Time.realtimeSinceStartup - startTime < timeout)
            {
                System.Threading.Thread.Sleep(10); // Brief sleep to avoid blocking
            }

            if (connectAsync.IsCompleted)
            {
                tcpClient.EndConnect(connectAsync);
                stream = tcpClient.GetStream();
                return true;
            }
            else
            {
                throw new Exception("Connection timed out");
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Connection attempt failed on port {serverPort}: {e.Message}");
            if (tcpClient != null)
            {
                tcpClient.Close();
                tcpClient = null;
            }
            return false;
        }
    }

    private IEnumerator TransmitPerception()
    {
        while (isConnected)
        {

            // Capture the screen as a texture
            /*RenderTexture targetTexture = frontCamera.targetTexture;
            Texture2D texture2D = new Texture2D(targetTexture.width, targetTexture.height, TextureFormat.RGB24, false);
            RenderTexture.active = targetTexture;
            texture2D.ReadPixels(new Rect(0, 0, targetTexture.width, targetTexture.height), 0, 0);
            texture2D.Apply();

            // Convert texture to byte array and encode to base64
            byte[] imageBytes = texture2D.EncodeToJPG();
            string imageBase64 = Convert.ToBase64String(imageBytes);

            // Send image data to Python server
            byte[] data = Encoding.UTF8.GetBytes(imageBase64);
            // udpClient.Send(data, data.Length, serverEndPoint);*/
            byte[] imageBytes = null;
            bool captureSuccessful = false;
            try
            {
                imageBytes = screenshotManager.CaptureScreenshot(); // If transmission freq is slower than refresh rate of camera, this will send the same frame to be processed multiple times
                captureSuccessful = true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Error capturing image on port {serverPort}: {e.Message}");
            }
            finally
            {
                //adam has it destroying the texture here, but we don't get the texture in this function in my code
            }
            if (captureSuccessful)
            {
                bool transmissionSuccessful = false;
                try
                {
                    byte[] sizeBytes = BitConverter.GetBytes(imageBytes.Length);
                    stream.Write(sizeBytes, 0, sizeBytes.Length);
                    stream.Write(imageBytes, 0, imageBytes.Length);
                    stream.Flush();
                    //ReceiveControlResponse();
                    Debug.LogError("You commented out the recievecontrol responses function");
                    transmissionSuccessful = true;
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error during transmission on port {serverPort}: {e.Message}");
                    isConnected = false;
                }

                if (!transmissionSuccessful)
                {
                    stream?.Close();
                    tcpClient?.Close();
                    Debug.Log($"Connection lost on port {serverPort}, attempting to reconnect...");
                    StartCoroutine(ConnectToServer());
                    yield break;
                }
            }

            yield return new WaitForSeconds(transmissionFrequency);
        }
    }

    private void ReceiveControlResponse()
    {
        try
        {
            byte[] lengthBuffer = new byte[4];
            int bytesRead = stream.Read(lengthBuffer, 0, 4);
            if (bytesRead != 4)
            {
                throw new Exception("Failed to read response length");
            }
            int responseLength = BitConverter.ToInt32(lengthBuffer, 0);

            byte[] responseBuffer = new byte[responseLength];
            bytesRead = 0;
            while (bytesRead < responseLength)
            {
                int read = stream.Read(responseBuffer, bytesRead, responseLength - bytesRead);
                if (read == 0)
                {
                    throw new Exception("Connection closed by server");
                }
                bytesRead += read;
            }

            string responseData = Encoding.UTF8.GetString(responseBuffer);




            detectionResults = JsonConvert.DeserializeObject<List<DetectionResult>>(responseData);

                // EXAMPLE of accessing response data
                //if (detectionResults != null && detectionResults.Count > 0)
                //{
                //    var firstDetection = detectionResults[0];
                //    Debug.Log($"Received detection: Class={firstDetection.ClassName}, Confidence={firstDetection.Confidence}");
                //}

            
        }
        catch (Exception e)
        {
            Debug.LogError("Idk, there was an error in ReceiveControlResponse: " + e.Message);
        }
    }

    public List<DetectionResult> GetDetectionResults()
    {
        return detectionsTest;
        return detectionResults;
    }

    private void OnApplicationQuit()
    {
        stream?.Close();
        tcpClient?.Close();
    }
}

