using Newtonsoft.Json;

public class DetectionResult
{
    [JsonProperty("class")]
    public string ClassName { get; set; }

    [JsonProperty("confidence")]
    public float Confidence { get; set; }

    [JsonProperty("xmin")]
    public float XMin { get; set; }

    [JsonProperty("ymin")]
    public float YMin { get; set; }

    [JsonProperty("xmax")]
    public float XMax { get; set; }

    [JsonProperty("ymax")]
    public float YMax { get; set; }
}