using Microsoft.AspNetCore.SignalR;
using MQTTnet;
using MQTTnet.Protocol;
using MQTTXXX.Shared;
using Newtonsoft.Json;
using System.Buffers;
using System.Text;

namespace MQTTXXX;

public class MqttService(IHubContext<MqttHub> hubContext)
{
    private readonly IMqttClient _client = new MqttClientFactory().CreateMqttClient();
    private readonly IHubContext<MqttHub> _hubContext = hubContext;

    public async Task ConnectAsync(MqttConnection connection)
    {
        var options = new MqttClientOptionsBuilder()
            .WithClientId(connection.ClientId)
            .WithTcpServer(connection.BrokerUri, connection.Port)
            .WithCredentials(connection.Username, connection.Password)
            .Build();

        _client.ConnectedAsync += async e =>
            await _hubContext.Clients.All.SendAsync("ConnectionStatus", true);

        _client.DisconnectedAsync += async e =>
            await _hubContext.Clients.All.SendAsync("ConnectionStatus", false);

        if (_client.IsConnected)
        {
            await _client.DisconnectAsync();
        }

        _client.ApplicationMessageReceivedAsync += async e =>
        {
            try
            {

                var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload.ToArray());
                if (string.IsNullOrEmpty(payload) || string.IsNullOrEmpty(e.ApplicationMessage.Topic))
                    return;

                object filteredData = e.ApplicationMessage.Topic switch
                {
                    "visualization" => FilterVisualizationData(payload),
                    "instantActions" => FilterInstantActionData(payload),
                    _ => new MqttMessage { Topic = e.ApplicationMessage.Topic, Payload = payload }
                };

                await _hubContext.Clients.All.SendAsync("ReceiveMessage", new MqttMessage
                {
                    Topic = e.ApplicationMessage.Topic,
                    Payload = JsonConvert.SerializeObject(filteredData)
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing MQTT message for topic {e.ApplicationMessage.Topic}: {ex.Message}");
            }
        };

        await _client.ConnectAsync(options, CancellationToken.None);
    }

    private static FilteredVisualizationData FilterVisualizationData(string payload)
    {
        var data = JsonConvert.DeserializeObject<VisualizationData>(payload);
        if (data == null) return null!;
        return new FilteredVisualizationData
        {
            HeaderId = data.HeaderId,
            Timestamp = data.Timestamp,
            AgvPosition = data.AgvPosition,
            Velocity = data.Velocity
        };
    }

    private static FilteredInstantActionData FilterInstantActionData(string payload)
    { 
        var data = JsonConvert.DeserializeObject<InstantActionData>(payload);
        if (data == null) return null!;
        return new FilteredInstantActionData
        {
            Timestamp = data.Timestamp,
            OrderId = data.OrderId,
            Actions = data.Actions?.Select(a => new FilteredActionData
            {
                ActionId = a.ActionId,
                ActionType = a.ActionType,
                ActionDescription = a.ActionStatus,
                ActionParameters = a.ActionParameters
            }).ToArray() ?? []
        };
    }

    public async Task DisconnectAsync() => await _client.DisconnectAsync();

    public async Task SubscribeAsync(string topic)
    {
        var filter = new MqttTopicFilterBuilder()
            .WithTopic(topic)
            .WithAtLeastOnceQoS()
            .Build();
        await _client.SubscribeAsync(filter);
    }

    public async Task UnsubscribeAsync(string topic)
    {
        await _client.UnsubscribeAsync(topic);
    }

    public async Task PublishAsync(MqttMessage message)
    {
        var appMessage = new MqttApplicationMessageBuilder()
            .WithTopic(message.Topic)
            .WithPayload(Encoding.UTF8.GetBytes(message.Payload))
            .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
            .Build();
        await _client.PublishAsync(appMessage);
    }
}