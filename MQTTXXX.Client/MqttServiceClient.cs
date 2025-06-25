using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using MQTTXXX.Shared;
using MudBlazor;
using System.Net.Http.Json;
using System.Text.Json;

namespace MQTTXXX.Client;

public class MqttServiceClient : IAsyncDisposable
{
    private readonly NavigationManager _navigationManager;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IJSRuntime _jsRuntime;
    private readonly ISnackbar _snackbar;
    private HubConnection? _hubConnection;
    private readonly Dictionary<string, object> _latestMessages = new();
    private readonly Dictionary<string, DateTime> _lastUpdateTimes = new();
    private readonly HashSet<string> _subscribedTopics = new();
    private readonly List<(bool Success, string Message)> _publishResults = new();
    private bool _connectionStatus;
    private bool _isPublishing;
    private double _publishProgress;
    private readonly List<string> _predefinedTopics = new() { "visualization", "state", "order", "instantActions" };

    public MqttConnection Connection { get; set; } = new()
    {
        BrokerUri = "172.20.235.170",
        Port = 1886,
        ClientId = "Demo",
        Username = "robotics",
        Password = "robotics"
    };

    public MqttMessage Message { get; set; } = new();
    public IReadOnlyDictionary<string, object> LatestMessages => _latestMessages;
    public bool ConnectionStatus => _connectionStatus;
    public IReadOnlyCollection<string> SubscribedTopics => _subscribedTopics;
    public IReadOnlyList<(bool Success, string Message)> PublishResults => _publishResults;
    public bool IsPublishing => _isPublishing;
    public double PublishProgress => _publishProgress;
    public IReadOnlyList<string> PredefinedTopics => _predefinedTopics;

    public event Action? StateChanged;
    private readonly TimeSpan _debounceInterval = TimeSpan.FromMilliseconds(100);
    private DateTime _lastStateChange = DateTime.MinValue;

    private void NotifyStateChanged()
    {
        if ((DateTime.UtcNow - _lastStateChange) > _debounceInterval)
        {
            _lastStateChange = DateTime.UtcNow;
            StateChanged?.Invoke();
        }
    }

    public MqttServiceClient(NavigationManager navigationManager, IHttpClientFactory httpClientFactory, IJSRuntime jsRuntime, ISnackbar snackbar)
    {
        _navigationManager = navigationManager;
        _httpClientFactory = httpClientFactory;
        _jsRuntime = jsRuntime;
        _snackbar = snackbar;
    }

    public async Task InitializeAsync()
    {
        _hubConnection = new HubConnectionBuilder()
                    .WithUrl(_navigationManager.ToAbsoluteUri("/mqtthub"))
                    .WithAutomaticReconnect()
                    .Build();

        _hubConnection.Reconnecting += error =>
        {
            Console.WriteLine($"SignalR reconnecting: {error?.Message}");
            ShowToast("Đang tái kết nối SignalR...", Severity.Warning);
            return Task.CompletedTask;
        };

        _hubConnection.On<bool>("ConnectionStatus", async status =>
        {
            _connectionStatus = status;
            if (!status) _subscribedTopics.Clear();
            NotifyStateChanged();
        });

        _hubConnection.On<MqttMessage>("ReceiveMessage", async msg =>
        {
            if (msg == null || string.IsNullOrEmpty(msg.Topic) || string.IsNullOrEmpty(msg.Payload))
            {
                Console.WriteLine("Received invalid message");
                return;
            }

            try
            {
                object data = msg.Topic switch
                {
                    "visualization" => Newtonsoft.Json.JsonConvert.DeserializeObject<VisualizationData>(msg.Payload) ?? new VisualizationData(),
                    "state" => Newtonsoft.Json.JsonConvert.DeserializeObject<StateMessage>(msg.Payload) ?? new StateMessage(),
                    "order" => Newtonsoft.Json.JsonConvert.DeserializeObject<OrderData>(msg.Payload) ?? new OrderData(),
                    "instantActions" => Newtonsoft.Json.JsonConvert.DeserializeObject<InstantActionData>(msg.Payload) ?? new InstantActionData(),
                    _ => msg
                };

                if (data != null)
                {
                    _latestMessages[msg.Topic] = data;
                    _lastUpdateTimes[msg.Topic] = DateTime.UtcNow;
                    CleanupOldMessages();
                    NotifyStateChanged();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deserializing message for topic {msg.Topic}: {ex.Message}");
            }
        });

        try
        {
            var savedTopics = await _jsRuntime.InvokeAsync<string[]>("localStorage.getItem", "subscribedTopics");
            if (savedTopics != null)
            {
                foreach (var topic in savedTopics)
                {
                    _subscribedTopics.Add(topic);
                }
            }
        }
        catch
        {
            // Ignore
        }

        try
        {
            await _hubConnection.StartAsync();
            await ShowToast("Kết nối SignalR thành công.", Severity.Success);
        }
        catch (Exception ex)
        {
            await ShowToast($"Không thể kết nối SignalR: {ex.Message}", Severity.Error);
        }
    }
    private void CleanupOldMessages()
    {
        var cutoff = DateTime.UtcNow.AddMinutes(-5);
        var oldKeys = _lastUpdateTimes.Where(kvp => kvp.Value < cutoff).Select(kvp => kvp.Key).ToList();
        foreach (var key in oldKeys)
        {
            _latestMessages.Remove(key);
            _lastUpdateTimes.Remove(key);
        }
    }
    public async Task ConnectAsync()
    {
        if (_connectionStatus)
        {
            await ShowToast("Đang kết nối tới broker MQTT.", Severity.Warning);
            return;
        }

        if (string.IsNullOrEmpty(Connection.BrokerUri) || Connection.Port <= 0 || string.IsNullOrEmpty(Connection.ClientId))
        {
            await ShowToast("Vui lòng điền đầy đủ thông tin kết nối.", Severity.Warning);
            return;
        }

        try
        {
            var httpClient = _httpClientFactory.CreateClient("MqttApi");
            var response = await httpClient.PostAsJsonAsync("api/mqtt/connect", Connection);
            response.EnsureSuccessStatusCode();
            await ShowToast("Đã kết nối với broker MQTT.", Severity.Success);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi khi kết nối với MQTT: {ex.Message}");
            await ShowToast($"Kết nối thất bại: {ex.Message}", Severity.Error);
        }
    }

    public async Task DisconnectAsync()
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient("MqttApi");
            var response = await httpClient.PostAsync("api/mqtt/disconnect", null);
            response.EnsureSuccessStatusCode();
            _subscribedTopics.Clear();
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "subscribedTopics", _subscribedTopics.ToArray());
            await ShowToast("Đã ngắt kết nối với broker MQTT.", Severity.Success);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi khi ngắt kết nối với MQTT: {ex.Message}");
            await ShowToast($"Ngắt kết nối thất bại: {ex.Message}", Severity.Error);
        }
    }

    public async Task ToggleSubscribeAsync(string topic, bool subscribe)
    {
        if (string.IsNullOrEmpty(topic))
        {
            await ShowToast("Chủ đề không hợp lệ.", Severity.Warning);
            return;
        }

        try
        {
            var httpClient = _httpClientFactory.CreateClient("MqttApi");
            var response = subscribe
                ? await httpClient.PostAsJsonAsync("api/mqtt/subscribe", topic)
                : await httpClient.PostAsJsonAsync("api/mqtt/unsubscribe", topic);
            response.EnsureSuccessStatusCode();

            if (subscribe)
                _subscribedTopics.Add(topic);
            else
            {
                _subscribedTopics.Remove(topic);
                _latestMessages.Remove(topic);
            }

            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "subscribedTopics", _subscribedTopics.ToArray());
            await ShowToast($"{(subscribe ? "Đã đăng ký" : "Đã hủy đăng ký")} chủ đề: {topic}", Severity.Success);
            NotifyStateChanged();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi khi {(subscribe ? "đăng ký" : "hủy đăng ký")} chủ đề {topic}: {ex.Message}");
            await ShowToast($"Thất bại khi {(subscribe ? "đăng ký" : "hủy đăng ký")} chủ đề: {ex.Message}", Severity.Error);
        }
    }

    public async Task SubscribeAllAsync()
    {
        try
        {
            int subscribedCount = 0;
            foreach (var topic in _predefinedTopics)
            {
                if (!_subscribedTopics.Contains(topic))
                {
                    await ToggleSubscribeAsync(topic, true);
                    subscribedCount++;
                }
            }
            await ShowToast($"Đã đăng ký {subscribedCount} chủ đề.", Severity.Success);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi trong SubscribeAllAsync: {ex.Message}");
            await ShowToast($"Lỗi khi đăng ký tất cả: {ex.Message}", Severity.Error);
        }
    }

    public async Task UnsubscribeAllAsync()
    {
        foreach (var topic in _subscribedTopics.ToList())
        {
            await ToggleSubscribeAsync(topic, false);
        }
    }

    public async Task PublishAsync()
    {
        if (string.IsNullOrEmpty(Message.Topic) || string.IsNullOrEmpty(Message.Payload))
        {
            await ShowToast("Vui lòng chọn chủ đề và nhập nội dung.", Severity.Warning);
            return;
        }

        _isPublishing = true;
        _publishResults.Clear();
        _publishProgress = 0;
        NotifyStateChanged();

        var payloads = new List<string>();
        try
        {
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(Message.Payload);
            if (jsonElement.ValueKind == JsonValueKind.Array)
                payloads.AddRange(jsonElement.EnumerateArray().Select(e => JsonSerializer.Serialize(e)));
            else
                payloads.Add(Message.Payload);
        }
        catch (JsonException ex)
        {
            _isPublishing = false;
            await ShowToast($"Nội dung JSON không hợp lệ: {ex.Message}", Severity.Error);
            NotifyStateChanged();
            return;
        }

        var httpClient = _httpClientFactory.CreateClient("MqttApi");
        for (int i = 0; i < payloads.Count; i++)
        {
            var payload = payloads[i];
            for (int attempt = 1; attempt <= 3; attempt++)
            {
                try
                {
                    var tempMessage = new MqttMessage { Topic = Message.Topic, Payload = payload };
                    var response = await httpClient.PostAsJsonAsync("api/mqtt/publish", tempMessage);
                    response.EnsureSuccessStatusCode();
                    _publishResults.Add((true, $"Đã gửi nội dung {i + 1}/{payloads.Count} tới chủ đề {Message.Topic}"));
                    break;
                }
                catch (Exception ex)
                {
                    if (attempt == 3)
                    {
                        Console.WriteLine($"Không thể gửi nội dung {i + 1} sau 3 lần thử: {ex.Message}");
                        _publishResults.Add((false, $"Thất bại nội dung {i + 1}/{payloads.Count}: {ex.Message}"));
                    }
                    else
                    {
                        await Task.Delay(500 * attempt);
                    }
                }
            }
            _publishProgress = (double)(i + 1) / payloads.Count;
            if (_publishResults.Count > 50)
                _publishResults.RemoveAt(0);
            NotifyStateChanged();
        }

        _isPublishing = false;
        _publishProgress = 1;
        NotifyStateChanged  ();
        await ShowToast($"Đã gửi {_publishResults.Count(p => p.Success)}/{payloads.Count} nội dung.", _publishResults.Any(p => !p.Success) ? Severity.Warning : Severity.Success);
    }


    public async Task ClearPayload()
    {
        Message.Payload = string.Empty;
        _publishResults.Clear();
        NotifyStateChanged();
        await ShowToast("Đã xóa nội dung.", Severity.Success);
    }

    public async Task FormatPayload()
    {
        try
        {
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(Message.Payload);
            Message.Payload = JsonSerializer.Serialize(jsonElement, new JsonSerializerOptions { WriteIndented = true });
            NotifyStateChanged();
            await ShowToast("Đã định dạng JSON.", Severity.Success);
        }
        catch (JsonException ex)
        {
            await ShowToast($"JSON không hợp lệ: {ex.Message}", Severity.Error);
        }
    }

    public async Task ValidatePayload()
    {
        try
        {
            JsonSerializer.Deserialize<JsonElement>(Message.Payload);
            await ShowToast("JSON hợp lệ.", Severity.Success);
        }
        catch (JsonException ex)
        {
            await ShowToast($"JSON không hợp lệ: {ex.Message}", Severity.Error);
        }
    }

    public async Task ClearPublishResults()
    {
        _publishResults.Clear();
        NotifyStateChanged();
    }

    public string FormatJson(string payload)
    {
        try
        {
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(payload);
            return JsonSerializer.Serialize(jsonElement, new JsonSerializerOptions { WriteIndented = true });
        }
        catch
        {
            return payload;
        }
    }

    private async Task ShowToast(string message, Severity severity)
    {
        _snackbar.Add(message, severity);
        await Task.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        if (_hubConnection != null)
            await _hubConnection.DisposeAsync();
    }
}

public class StateMessage
{
    public Sate? OrderData { get; set; }
    public ActionState[]? ActionStates { get; set; }
    public BatteryState? BatteryState { get; set; }
    public Error[]? Errors { get; set; }
    public Information[]? Information { get; set; }
}