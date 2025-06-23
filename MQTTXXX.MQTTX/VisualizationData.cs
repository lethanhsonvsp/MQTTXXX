
namespace MQTTXXX.Shared;

using Newtonsoft.Json;

public class VisualizationData
{
    [JsonProperty("headerId")]
    public uint HeaderId { get; set; }
    [JsonProperty("timestamp")]
    public string? Timestamp { get; set; }
    [JsonProperty("version")]
    public string? Version { get; set; }
    [JsonProperty("manufacturer")]
    public string? Manufacturer { get; set; }
    [JsonProperty("serialNumber")]
    public string? SerialNumber { get; set; }
    [JsonProperty("mapId")]
    public string? MapId { get; set; }
    [JsonProperty("mapDescription")]
    public string? MapDescription { get; set; }
    [JsonProperty("agvPosition")]
    public AgvPosition? AgvPosition { get; set; }
    [JsonProperty("velocity")]
    public Velocity? Velocity { get; set; }
}
public class FilteredVisualizationData
{
    [JsonProperty("timestamp")]
    public string? Timestamp { get; set; }
    [JsonProperty("agvPosition")]
    public AgvPosition? AgvPosition { get; set; }
    [JsonProperty("velocity")]
    public Velocity? Velocity { get; set; }
}