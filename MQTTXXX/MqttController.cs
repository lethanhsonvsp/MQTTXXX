using Microsoft.AspNetCore.Mvc;
using MQTTXXX.Shared;

namespace MQTTXXX;

[ApiController]
[Route("api/[controller]")]
public class MqttController(MqttService mqttService) : ControllerBase
{
    private readonly MqttService _mqttService = mqttService;

    [HttpPost("connect")]
    public async Task<IActionResult> Connect([FromBody] MqttConnection connection)
    {
        try
        {
            await _mqttService.ConnectAsync(connection);

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to connect to MQTT: {ex.Message}");
            throw; // Hoặc trả về một ngoại lệ tùy chỉnh
        }   
        return Ok();
    }

    [HttpPost("disconnect")]
    public async Task<IActionResult> Disconnect()
    {
        await _mqttService.DisconnectAsync();
        return Ok();
    }

    [HttpPost("subscribe")]
    public async Task<IActionResult> Subscribe([FromBody] string topic)
    {
        try
        {
            await _mqttService.SubscribeAsync(topic);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to subscribe to topic '{topic}': {ex.Message}");
            throw; // Hoặc trả về một ngoại lệ tùy chỉnh
        }
        return Ok();
    }

    [HttpPost("unsubscribe")]
    public async Task<IActionResult> Unsubscribe([FromBody] string topic)
    {
        try
        {
            await _mqttService.UnsubscribeAsync(topic);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to unsubscribe from topic '{topic}': {ex.Message}");
            throw; // Hoặc trả về một ngoại lệ tùy chỉnh
        }
        return Ok();
    }

    [HttpPost("publish")]
    public async Task<IActionResult> Publish([FromBody] MqttMessage message)
    {
        try
        {
            await _mqttService.PublishAsync(message);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to subscribe to topic '{message}': {ex.Message}");
            throw; // Hoặc trả về một ngoại lệ tùy chỉnh
        }
        return Ok();
    }
}