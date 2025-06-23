

namespace MQTTXXX.Shared;

public class MqttConnection
{
    public string? BrokerUri { get; set; }
    public int Port { get; set; }
    public string? ClientId { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
}

public class MqttMessage
{
    public string Topic { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

}