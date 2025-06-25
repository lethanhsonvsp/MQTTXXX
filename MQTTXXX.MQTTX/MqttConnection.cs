

namespace MQTTXXX.Shared;

public class MqttConnection
{
    public int Id { get; set; } = 1; // Chỉ lưu 1 connection
    public string BrokerUri { get; set; } = string.Empty;
    public int Port { get; set; }
    public string ClientId { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty; // Lưu ý: Cần mã hóa
}

public class MqttMessage
{
    public string Topic { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

}
public class SubscribedTopic
{
    public string ClientId { get; set; } = string.Empty;
    public string Topic { get; set; } = string.Empty;
}

public class MqttMessageEntity
{
    public string ClientId { get; set; } = string.Empty;
    public string Topic { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}

public class PublishResult
{
    public int Id { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}