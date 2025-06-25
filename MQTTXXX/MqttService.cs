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
                    "state" => new
                    {
                        OrderData = FilteredSateData(payload),
                        ActionStates = FilterActionStates(payload),
                        BatteryState = FilterBatteryState(payload),
                        Errors = FilterErrors(payload),
                        Information = FilterInformation(payload)
                    },
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

    private static VisualizationData FilterVisualizationData(string payload)
    {
        var data = JsonConvert.DeserializeObject<VisualizationData>(payload);
        if (data == null) return null!;
        return new VisualizationData
        {
            HeaderId = data.HeaderId,
            Timestamp = data.Timestamp,
            Version = data.Version,
            Manufacturer = data.Manufacturer,
            SerialNumber = data.SerialNumber,
            MapId = data.MapId,
            MapDescription = data.MapDescription,
            AgvPosition = data.AgvPosition,
            Velocity = data.Velocity
        };
    }
    private static Sate FilteredSateData(string payload)
    {
        var data = JsonConvert.DeserializeObject<Sate>(payload);
        if (data == null) return null!;
        return new Sate
        {
            HeaderId = data.HeaderId,
            Timestamp = data.Timestamp,
            Version = data.Version,
            Manufacturer = data.Manufacturer,
            SerialNumber = data.SerialNumber,
            Maps = data.Maps,
            OrderId = data.OrderId,
            OrderUpdateId = data.OrderUpdateId,
            ZoneSetId = data.ZoneSetId,
            LastNodeId = data.LastNodeId,
            LastNodeSequenceId = data.LastNodeSequenceId,
            NodeStates = data.NodeStates,
            EdgeStates = data.EdgeStates,
            Driving = data.Driving,
            Paused = data.Paused,
            NewBaseRequest = data.NewBaseRequest,
            DistanceSinceLastNode = data.DistanceSinceLastNode,
            AgvPosition = data.AgvPosition,
            Velocity = data.Velocity,
            Loads = data.Loads,
            OperatingMode = data.OperatingMode,
        };
    }

    private static ActionState[] FilterActionStates(string payload)
    {
        var data = JsonConvert.DeserializeObject<Sate>(payload);
        if (data == null || data.ActionStates == null) return [];
        return [.. data.ActionStates.Select(state => new ActionState
        {
            ActionId = state.ActionId,
            ActionType = state.ActionType,
            ActionDescription = state.ActionDescription,
            ActionStatus = state.ActionStatus,
            ResultDescription = state.ResultDescription
        })];
    }

    private static BatteryState FilterBatteryState(string payload)
    {
        var data = JsonConvert.DeserializeObject<Sate>(payload);
        if (data == null || data.BatteryState == null) return null!;
        return new BatteryState
        {
            BatteryCharge = data.BatteryState.BatteryCharge,
            BatteryVoltage = data.BatteryState.BatteryVoltage,
            BatteryHealth = data.BatteryState.BatteryHealth,
            Charging = data.BatteryState.Charging,
            Reach = data.BatteryState.Reach
        };
    }

    private static Error[] FilterErrors(string payload)
    {
        var data = JsonConvert.DeserializeObject<Sate>(payload);
        if (data == null || data.Errors == null) return [];
        return [.. data.Errors.Select(error => new Error
        {
            ErrorType = error.ErrorType,
            ErrorReferences = error.ErrorReferences,
            ErrorDescription = error.ErrorDescription,
            ErrorHint = error.ErrorHint,
            ErrorLevel = error.ErrorLevel
        })];
    }

    private static Information[] FilterInformation(string payload)
    {
        var data = JsonConvert.DeserializeObject<Sate>(payload);
        if (data == null || data.Information == null) return [];
        return [.. data.Information.Select(info => new Information
        {
            InfoType = info.InfoType,
            InfoReferences = info.InfoReferences,
            InfoDescription = info.InfoDescription,
            InfoLevel = info.InfoLevel
        })];
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