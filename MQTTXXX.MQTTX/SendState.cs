using Newtonsoft.Json;


namespace MQTTXXX.Shared;


public class SendState
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

    [JsonProperty("maps")]
    public Map[]? Maps { get; set; }

    [JsonProperty("orderId")]
    public string? OrderId { get; set; }

    [JsonProperty("orderUpdateId")]
    public int OrderUpdateId { get; set; }

    [JsonProperty("zoneSetId")]
    public string? ZoneSetId { get; set; }
    [JsonProperty("lastNodeId")]
    public string? LastNodeId { get; set; }
    [JsonProperty("lastNodeSequenceId")]
    public int LastNodeSequenceId { get; set; }
    [JsonProperty("nodeStates")]
    public NodeState[]? NodeStates { get; set; }
    [JsonProperty("edgeStates")]
    public EdgeState[]? EdgeStates { get; set; }
    [JsonProperty("driving")]
    public bool Driving { get; set; }
    [JsonProperty("paused")]
    public bool Paused { get; set; }
    [JsonProperty("newBaseRequest")]
    public bool NewBaseRequest { get; set; }
    [JsonProperty("distanceSinceLastNode")]
    public float DistanceSinceLastNode { get; set; }
    [JsonProperty("agvPosition")]
    public AgvPosition? AgvPosition { get; set; }
    [JsonProperty("velocity")]
    public Velocity? Velocity { get; set; }
    [JsonProperty("loads")]
    public Load[]? Loads { get; set; }
    [JsonProperty("actionStates")]
    public ActionState[]? ActionStates { get; set; }
    [JsonProperty("batteryState")]
    public BatteryState? BatteryState { get; set; }
    [JsonProperty("operatingMode")]
    public string? OperatingMode { get; set; }
    [JsonProperty("errors")]
    public Error[]? Errors { get; set; }
    [JsonProperty("information")]
    public Information[]? Information { get; set; }
    [JsonProperty("safetyState")]
    public SafetyState? SafetyState { get; set; }
}

public class CancelConfirmationData
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
    [JsonProperty("actionStates")]
    public ActionState[]? ActionStates { get; set; }
}

public class Map
{
    [JsonProperty("mapId")]
    public string? MapId { get; set; }
    [JsonProperty("mapVersion")]
    public string? MapVersion { get; set; }
    [JsonProperty("mapDescription")]
    public string? MapDescription { get; set; }
    [JsonProperty("mapStatus")]
    public string? MapStatus { get; set; }
}

public class NodeState
{
    [JsonProperty("nodeId")]
    public string? NodeId { get; set; }
    [JsonProperty("sequenceId")]
    public int SequenceId { get; set; }
    [JsonProperty("nodeDescription")]
    public string? NodeDescription { get; set; }
    [JsonProperty("released")]
    public bool Released { get; set; }
    [JsonProperty("nodeStatus")]
    public NodePosition? NodePosition { get; set; }
}

public class EdgeState
{
    [JsonProperty("edgeId")]
    public string? EdgeId { get; set; }
    [JsonProperty("sequenceId")]
    public int SequenceId { get; set; }
    [JsonProperty("edgeDescription")]
    public string? EdgeDescription { get; set; }
    [JsonProperty("released")]
    public bool Released { get; set; }
    [JsonProperty("trajectory")]
    public Trajectory? Trajectory { get; set; }
}

public class AgvPosition
{
    [JsonProperty("x")]
    public float X { get; set; }
    [JsonProperty("y")]
    public float Y { get; set; }
    [JsonProperty("theta")]
    public float Theta { get; set; }
    [JsonProperty("mapId")]
    public string? MapId { get; set; }
    [JsonProperty("mapDescription")]
    public string? MapDescription { get; set; }
    [JsonProperty("positionInitialized")]
    public bool PositionInitialized { get; set; }
    [JsonProperty("positionStatus")]
    public float LocalizationScore { get; set; }
    [JsonProperty("deviationRange")]
    public float DeviationRange { get; set; }
}

public class Velocity
{
    [JsonProperty("vx")]
    public float Vx { get; set; }
    [JsonProperty("vy")]
    public float Vy { get; set; }
    [JsonProperty("omega")]
    public float Omega { get; set; }
}

public class Load
{
    [JsonProperty("loadId")]
    public string? LoadId { get; set; }
    [JsonProperty("loadType")]
    public string? LoadType { get; set; }
    [JsonProperty("loadPosition")]
    public string? LoadPosition { get; set; }
    [JsonProperty("boundingBoxReference")]
    public BoundingBoxReference? BoundingBoxReference { get; set; }
    [JsonProperty("loadDimensions")]
    public LoadDimensions? LoadDimensions { get; set; }
    [JsonProperty("weight")]
    public float Weight { get; set; }
}

public class BoundingBoxReference
{
    [JsonProperty("x")]
    public float X { get; set; }
    [JsonProperty("y")]
    public float Y { get; set; }
    [JsonProperty("z")]
    public float Z { get; set; }
    [JsonProperty("theta")]
    public float Theta { get; set; }
}

public class LoadDimensions
{
    [JsonProperty("length")]
    public float Length { get; set; }
    [JsonProperty("width")]
    public float Width { get; set; }
    [JsonProperty("height")]
    public float Height { get; set; }
}

public class ActionState
{
    [JsonProperty("actionId")]
    public string? ActionId { get; set; }
    [JsonProperty("actionType")]
    public string? ActionType { get; set; }
    [JsonProperty("actionDescription")]
    public string? ActionDescription { get; set; }
    [JsonProperty("actionStatus")]
    public string? ActionStatus { get; set; }
    [JsonProperty("resultDescription")]
    public string? ResultDescription { get; set; }
}

public class BatteryState
{
    [JsonProperty("batteryCharge")]
    public float BatteryCharge { get; set; }
    [JsonProperty("batteryVoltage")]
    public float BatteryVoltage { get; set; }
    [JsonProperty("batteryHealth")]
    public float BatteryHealth { get; set; }
    [JsonProperty("charging")]
    public bool Charging { get; set; }
    [JsonProperty("reach")]
    public float Reach { get; set; }
}

public class Error
{
    [JsonProperty("errorType")]
    public string? ErrorType { get; set; }
    [JsonProperty("errorReferences")]
    public ErrorReference[]? ErrorReferences { get; set; }
    [JsonProperty("errorDescription")]
    public string? ErrorDescription { get; set; }
    [JsonProperty("errorHint")]
    public string? ErrorHint { get; set; }
    [JsonProperty("errorLevel")]
    public string? ErrorLevel { get; set; }
}

public class ErrorReference
{
    [JsonProperty("referenceKey")]
    public string? ReferenceKey { get; set; }
    [JsonProperty("referenceValue")]
    public string? ReferenceValue { get; set; }
}

public class Information
{
    [JsonProperty("infoType")]
    public string? InfoType { get; set; }
    [JsonProperty("infoReferences")]
    public InfoReference[]? InfoReferences { get; set; }
    [JsonProperty("infoDescription")]
    public string? InfoDescription { get; set; }
    [JsonProperty("infoLevel")]
    public string? InfoLevel { get; set; }
}

public class InfoReference
{
    [JsonProperty("referenceKey")]
    public string? ReferenceKey { get; set; }
    [JsonProperty("referenceValue")]
    public string? ReferenceValue { get; set; }
}

public class SafetyState
{
    [JsonProperty("eStop")]
    public string? EStop { get; set; }
    [JsonProperty("fieldViolation")]
    public bool FieldViolation { get; set; }
}

public class Trajectory
{
    [JsonProperty("degree")]
    public int Degree { get; set; }
    [JsonProperty("knotVector")]
    public float[]? KnotVector { get; set; }
    [JsonProperty("controlPoints")]
    public ControlPoint[]? ControlPoints { get; set; }
}

public class ControlPoint
{
    [JsonProperty("x")]
    public float X { get; set; }
    [JsonProperty("y")]
    public float Y { get; set; }
    [JsonProperty("weight")]
    public float Weight { get; set; }
}
