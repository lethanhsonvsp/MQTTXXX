using Newtonsoft.Json;


namespace MQTTXXX.Shared;

public class InstantActionData
{
    [JsonProperty("headerId")]
    public int HeaderId { get; set; }
    [JsonProperty("timestamp")]
    public string? Timestamp { get; set; }
    [JsonProperty("version")]
    public string? Version { get; set; }
    [JsonProperty("manufacturer")]
    public string? Manufacturer { get; set; }
    [JsonProperty("serialNumber")]
    public string? SerialNumber { get; set; }
    [JsonProperty("orderId")]
    public string? OrderId { get; set; }
    [JsonProperty("orderUpdateId")]
    public int OrderUpdateId { get; set; }
    [JsonProperty("zoneSetId")]
    public string? ZoneSetId { get; set; }
    [JsonProperty("actions")]
    public ActionData[]? Actions { get; set; }
}

public class ActionData
{
    [JsonProperty("actionId")]
    public string? ActionId { get; set; }
    [JsonProperty("actionType")]
    public string? ActionType { get; set; }
    [JsonProperty("actionStatus")]
    public string? ActionStatus { get; set; }
    [JsonProperty("blockingType")]
    public string? BlockingType { get; set; }
    [JsonProperty("resultDescription")]
    public string? ActionDescription { get; set; }
    [JsonProperty("actionParameters")]
    public Parameter[]? ActionParameters { get; set; } // Sửa thành actionParameters
}

public class Parameter
{
    [JsonProperty("key")]
    public string? Key { get; set; }
    [JsonProperty("value")]
    public string? Value { get; set; }
}
// Helper class for result passing
public class ExecutionResult
{
    public bool ActionResult { get; set; }
    public string? ActionStatus { get; set; }
    public string? ResultDescription { get; set; }
}
public class NodePosition
{
    [JsonProperty("x")]
    public float X { get; set; }
    [JsonProperty("y")]
    public float Y { get; set; }
    [JsonProperty("theta")]
    public float Theta { get; set; }
    [JsonProperty("allowedDeviationXY")]
    public float AllowedDeviationXY { get; set; }
    [JsonProperty("allowedDeviationTheta")]
    public float AllowedDeviationTheta { get; set; }
    [JsonProperty("mapId")]
    public string? MapId { get; set; }
    [JsonProperty("mapDescription")]
    public string? MapDescription { get; set; }
}

public class FilteredInstantActionData
{
    [JsonProperty("timestamp")]
    public string? Timestamp { get; set; }
    [JsonProperty("orderId")]
    public string? OrderId { get; set; }
    [JsonProperty("actions")]
    public FilteredActionData[]? Actions { get; set; }
}

public class FilteredActionData
{
    [JsonProperty("actionId")]
    public string? ActionId { get; set; }
    [JsonProperty("actionType")]
    public string? ActionType { get; set; }
    [JsonProperty("actionStatus")]
    public string? ActionDescription { get; set; }
    [JsonProperty("actionParameters")]
    public Parameter[]? ActionParameters { get; set; }
}

public class FilteredOrderData
{
    [JsonProperty("timestamp")]
    public string? Timestamp { get; set; }
    [JsonProperty("orderId")]
    public string? OrderId { get; set; }
    [JsonProperty("orderUpdateId")]
    public int? OrderUpdateId { get; set; }
    [JsonProperty("zoneSetId")]
    public string? ZoneSetId { get; set; }
    [JsonProperty("lastNodeId")]
    public string? LastNodeId { get; set; }
    [JsonProperty("lastNodeSequenceId")]
    public int? LastNodeSequenceId { get; set; }
    [JsonProperty("driving")]
    public bool Driving { get; set; }
    [JsonProperty("paused")]
    public bool Paused { get; set; }
    [JsonProperty("newBaseRequest")]
    public bool NewBaseRequest { get; set; }
    [JsonProperty("distanceSinceLastNode")]
    public float DistanceSinceLastNode { get; set; }
    [JsonProperty("operatingMode")]
    public string? OperatingMode { get; set; }
}