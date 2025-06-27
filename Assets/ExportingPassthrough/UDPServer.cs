using System; // Adam & Unity
using System.Collections; //Adam & Unity
using System.Collections.Generic; // Adam & Unity
using System.Net.Sockets; //Adam & Unity
using System.Net; // Adam & Unity
using System.Text; //Adam & Unity
using UnityEngine;
using UnityEngine.InputSystem.XR; // Unity
using Newtonsoft.Json;

public class UDPServer : MonoBehaviour
{
    // ML perception
    public Camera frontCamera;
    public float transmissionFrequency = 0.1f;
    // Car controller 
    //UNCOMMENT THIS OUT LATER
    //private CarController m_Car;
    private float steer;
    private float throttle;
    private float brake;
    // Networking
    private string serverIP = "127.0.0.1";
    private int serverPort = 5005;
    private int maxChunkSize = 65536;
    private Socket socket;
    private UdpClient udpClient;
    private IPEndPoint serverEndPoint;

    private void Awake()
    {
        // Set the car controller
        //UNCOMMENT THIS OUT LATER
        //m_Car = GetComponent<CarController>();
    }

    private void Start()
    {
        // Set up UDP client
        udpClient = new UdpClient();
        serverEndPoint = new IPEndPoint(IPAddress.Parse(serverIP), serverPort);

        // Start sending data
        StartCoroutine(TransmitPerception());
    }

    private IEnumerator TransmitPerception()
    {
        while (true)
        {
            // Capture the screen as a texture
            RenderTexture targetTexture = frontCamera.targetTexture;
            Texture2D texture2D = new Texture2D(targetTexture.width, targetTexture.height, TextureFormat.RGB24, false);
            RenderTexture.active = targetTexture;
            texture2D.ReadPixels(new Rect(0, 0, targetTexture.width, targetTexture.height), 0, 0);
            texture2D.Apply();

            // Convert texture to byte array and encode to base64
            byte[] imageBytes = texture2D.EncodeToJPG();
            string imageBase64 = Convert.ToBase64String(imageBytes);

            // Send image data to Python server
            byte[] data = Encoding.UTF8.GetBytes(imageBase64);
            // udpClient.Send(data, data.Length, serverEndPoint);

            // Send the image data in chunks
            int bytesSent = 0;
            while (bytesSent < imageBytes.Length)
            {
                // Calculate the chunk size for this iteration
                int chunkSize = Math.Min(maxChunkSize, imageBytes.Length - bytesSent);
                byte[] chunk = new byte[chunkSize];
                Array.Copy(imageBytes, bytesSent, chunk, 0, chunkSize);

                // Send the chunk to the Python server
                udpClient.Send(chunk, chunk.Length, serverEndPoint);

                // Move the byte index forward
                bytesSent += chunkSize;
            }

            // Receive control commands from the server
            ReceiveControlResponse();

            yield return new WaitForSeconds(transmissionFrequency);  // Send data at the specified frequency
        }
    }

    private void ReceiveControlResponse()
    {
        try
        {
            // Check if there's data available from the server
            if (udpClient.Available > 0)
            {
                IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, serverPort);
                byte[] receivedBytes = udpClient.Receive(ref remoteEndPoint);

                // Decode and parse control commands
                string responseData = Encoding.UTF8.GetString(receivedBytes);
                var detectionResults = JsonConvert.DeserializeObject<List<DetectionResult>>(responseData);

                // EXAMPLE of accessing response data
                if (detectionResults != null && detectionResults.Count > 0)
                {
                    var firstDetection = detectionResults[0];
                    Debug.Log($"Received detection: Class={firstDetection.ClassName}, Confidence={firstDetection.Confidence}");
                }

            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error receiving control response: " + e.Message);
        }
    }

    void FixedUpdate()
    {
        // Update the car's movement based on the steering and throttle values
        //UNCOMMENT THIS OUT LATER
        //m_Car.Move(steer, throttle, 0f, 0f);
    }

    private void OnApplicationQuit()
    {
        udpClient.Close();  // Close the UDP client when the application quits
    }
}

