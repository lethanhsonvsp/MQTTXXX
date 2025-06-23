
using Newtonsoft.Json;

namespace MQTTXXX.Shared;


public class OrderData
{
    public uint HeaderId { get; set; }
    public string? Timestamp { get; set; }
    public string? Version { get; set; }
    public string? Manufacturer { get; set; }
    public string? SerialNumber { get; set; }
    public string? OrderId { get; set; }
    public int OrderUpdateId { get; set; }
    public string? ZoneSetId { get; set; }
    [JsonProperty("nodes")]
    public Node[]? Nodes { get; set; }
    [JsonProperty("edges")]
    public Edge[]? Edges { get; set; }
}
public class Node
{
    [JsonProperty("nodeId")]
    public string? NodeId { get; set; }
    [JsonProperty("sequenceId")]
    public int SequenceId { get; set; }
    [JsonProperty("nodeDescription")]
    public string? NodeDescription { get; set; }
    [JsonProperty("released")]
    public bool Released { get; set; }
    [JsonProperty("nodePosition")]
    public NodePosition? NodePosition { get; set; }
    [JsonProperty("actions")]
    public ActionData[]? Actions { get; set; }
}


public class Edge
{
    [JsonProperty("edgeId")]
    public string? EdgeId { get; set; }
    [JsonProperty("sequenceId")]
    public int SequenceId { get; set; }
    [JsonProperty("edgeDescription")]
    public string? EdgeDescription { get; set; }
    [JsonProperty("released")]
    public bool Released { get; set; }
    [JsonProperty("startNodeId")]
    public string? StartNodeId { get; set; }
    [JsonProperty("endNodeId")]
    public string? EndNodeId { get; set; }
    [JsonProperty("maxSpeed")]
    public float MaxSpeed { get; set; }
    [JsonProperty("minSpeed")]
    public float MaxHeight { get; set; }
    [JsonProperty("minHeight")]
    public float MinHeight { get; set; }
    [JsonProperty("orientation")]
    public float Orientation { get; set; }
    [JsonProperty("orientationType")]
    public string? OrientationType { get; set; }
    [JsonProperty("direction")]
    public string? Direction { get; set; }
    [JsonProperty("rotationAllowed")]
    public bool RotationAllowed { get; set; }
    [JsonProperty("maxRotationSpeed")]
    public float MaxRotationSpeed { get; set; }
    [JsonProperty("length")]
    public float Length { get; set; }
    [JsonProperty("trajectory")]
    public Trajectory? Trajectory { get; set; }

    [JsonProperty("actions")]
    public ActionData[]? Actions { get; set; }

}
