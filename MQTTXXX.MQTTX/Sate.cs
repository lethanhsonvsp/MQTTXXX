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
    public Node[]? Nodes { get; set; }
    public Edge[]? Edges { get; set; }
}
public class Node
{
    public string? NodeId { get; set; }
    public int SequenceId { get; set; }
    public string? NodeDescription { get; set; }
    public bool Released { get; set; }
    public NodePosition? NodePosition { get; set; }
    public ActionData[]? Actions { get; set; }
}

public class Edge
{
    public string? EdgeId { get; set; }
    public int SequenceId { get; set; }
    public string? EdgeDescription { get; set; }
    public bool Released { get; set; }
    public string? StartNodeId { get; set; }
    public string? EndNodeId { get; set; }
    public float MaxSpeed { get; set; }
    public float MaxHeight { get; set; }
    public float MinHeight { get; set; }
    public float Orientation { get; set; }
    public string? OrientationType { get; set; }
    public string? Direction { get; set; }
    public bool RotationAllowed { get; set; }
    public float MaxRotationSpeed { get; set; }
    public float Length { get; set; }
    public Trajectory? Trajectory { get; set; }
    public ActionData[]? Actions { get; set; }

}

public class VisualizationData
{
    public uint HeaderId { get; set; }
    public string? Timestamp { get; set; }
    public string? Version { get; set; }
    public string? Manufacturer { get; set; }
    public string? SerialNumber { get; set; }
    public string? MapId { get; set; }
    public string? MapDescription { get; set; }
    public AgvPosition? AgvPosition { get; set; }
    public Velocity? Velocity { get; set; }
}

public class Sate
{
    public uint HeaderId { get; set; }

    public string? Timestamp { get; set; }

    public string? Version { get; set; }

    public string? Manufacturer { get; set; }

    public string? SerialNumber { get; set; }

    public Map[]? Maps { get; set; }

    public string? OrderId { get; set; }

    public int OrderUpdateId { get; set; }

    public string? ZoneSetId { get; set; }
    public string? LastNodeId { get; set; }
    public int LastNodeSequenceId { get; set; }
    public NodeState[]? NodeStates { get; set; }
    public EdgeState[]? EdgeStates { get; set; }
    public bool Driving { get; set; }
    public bool Paused { get; set; }
    public bool NewBaseRequest { get; set; }
    public float DistanceSinceLastNode { get; set; }
    public AgvPosition? AgvPosition { get; set; }
    public Velocity? Velocity { get; set; }
    public Load[]? Loads { get; set; }
    public ActionState[]? ActionStates { get; set; }
    public BatteryState? BatteryState { get; set; }
    public string? OperatingMode { get; set; }
    public Error[]? Errors { get; set; }
    public Information[]? Information { get; set; }
    public SafetyState? SafetyState { get; set; }
}

public class Map
{
    public string? MapId { get; set; }
    public string? MapVersion { get; set; }
    public string? MapDescription { get; set; }
    public string? MapStatus { get; set; }
}

public class NodeState
{
    public string? NodeId { get; set; }
    public int SequenceId { get; set; }
    public string? NodeDescription { get; set; }
    public bool Released { get; set; }
    public NodePosition? NodePosition { get; set; }
}

public class EdgeState
{
    public string? EdgeId { get; set; }
    public int SequenceId { get; set; }
    public string? EdgeDescription { get; set; }
    public bool Released { get; set; }
    public Trajectory? Trajectory { get; set; }
}

public class AgvPosition
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Theta { get; set; }
    public string? MapId { get; set; }
    public string? MapDescription { get; set; }
    public bool PositionInitialized { get; set; }
    public float LocalizationScore { get; set; }
    public float DeviationRange { get; set; }
}

public class Velocity
{
    public float Vx { get; set; }
    public float Vy { get; set; }
    public float Omega { get; set; }
}

public class Load
{
    public string? LoadId { get; set; }
    public string? LoadType { get; set; }
    public string? LoadPosition { get; set; }
    public BoundingBoxReference? BoundingBoxReference { get; set; }
    public LoadDimensions? LoadDimensions { get; set; }
    public float Weight { get; set; }
}

public class BoundingBoxReference
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
    public float Theta { get; set; }
}

public class LoadDimensions
{
    public float Length { get; set; }
    public float Width { get; set; }
    public float Height { get; set; }
}

public class ActionState
{
    public string? ActionId { get; set; }
    public string? ActionType { get; set; }
    public string? ActionDescription { get; set; }
    public string? ActionStatus { get; set; }
    public string? ResultDescription { get; set; }
}

public class BatteryState
{
    public float BatteryCharge { get; set; }
    public float BatteryVoltage { get; set; }
    public float BatteryHealth { get; set; }
    public bool Charging { get; set; }
    public float Reach { get; set; }
}

public class Error
{
    public string? ErrorType { get; set; }
    public ErrorReference[]? ErrorReferences { get; set; }
    public string? ErrorDescription { get; set; }
    public string? ErrorHint { get; set; }
    public string? ErrorLevel { get; set; }
}

public class ErrorReference
{
    public string? ReferenceKey { get; set; }
    public string? ReferenceValue { get; set; }
}

public class Information
{
    public string? InfoType { get; set; }
    public InfoReference[]? InfoReferences { get; set; }
    public string? InfoDescription { get; set; }
    public string? InfoLevel { get; set; }
}

public class InfoReference
{
    public string? ReferenceKey { get; set; }
    public string? ReferenceValue { get; set; }
}

public class SafetyState
{
    public string? EStop { get; set; }
    public bool FieldViolation { get; set; }
}

public class Trajectory
{
    public int Degree { get; set; }
    public float[]? KnotVector { get; set; }
    public ControlPoint[]? ControlPoints { get; set; }
}

public class ControlPoint
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Weight { get; set; }
}
