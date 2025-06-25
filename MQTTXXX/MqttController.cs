using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MQTTXXX.Shared;
using System.Text;

namespace MQTTXXX;

[ApiController]
[Route("api/[controller]")]
public class MqttController : ControllerBase
{
    private readonly MqttService _mqttService;
    private readonly MqttDbContext _dbContext;

    public MqttController(MqttService mqttService, MqttDbContext dbContext)
    {
        _mqttService = mqttService;
        _dbContext = dbContext;
    }

    [HttpPost("connect")]
    public async Task<IActionResult> Connect([FromBody] MqttConnection connection)
    {
        try
        {
            await _mqttService.ConnectAsync(connection);

            // Lưu connection vào DB (mã hóa Password)
            var dbConnection = await _dbContext.Connections.FirstOrDefaultAsync(c => c.Id == 1);
            if (dbConnection == null)
            {
                dbConnection = new MqttConnection
                {
                    Id = 1,
                    BrokerUri = connection.BrokerUri,
                    Port = connection.Port,
                    ClientId = connection.ClientId,
                    Username = connection.Username,
                    Password = Encrypt(connection.Password)
                };
                _dbContext.Connections.Add(dbConnection);
            }
            else
            {
                dbConnection.BrokerUri = connection.BrokerUri;
                dbConnection.Port = connection.Port;
                dbConnection.ClientId = connection.ClientId;
                dbConnection.Username = connection.Username;
                dbConnection.Password = Encrypt(connection.Password);
            }
            await _dbContext.SaveChangesAsync();

            return Ok();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to connect to MQTT: {ex.Message}");
            return StatusCode(500, new { Error = ex.Message });
        }
    }

    [HttpPost("disconnect")]
    public async Task<IActionResult> Disconnect()
    {
        try
        {
            await _mqttService.DisconnectAsync();
            return Ok();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to disconnect: {ex.Message}");
            return StatusCode(500, new { Error = ex.Message });
        }
    }

    [HttpPost("subscribe")]
    public async Task<IActionResult> Subscribe([FromBody] string topic)
    {
        try
        {
            await _mqttService.SubscribeAsync(topic);

            // Lưu topic vào DB
            var clientId = (await _dbContext.Connections.FirstOrDefaultAsync(c => c.Id == 1))?.ClientId;
            if (clientId != null && !await _dbContext.SubscribedTopics.AnyAsync(t => t.ClientId == clientId && t.Topic == topic))
            {
                _dbContext.SubscribedTopics.Add(new SubscribedTopic
                {
                    ClientId = clientId,
                    Topic = topic
                });
                await _dbContext.SaveChangesAsync();
            }

            return Ok();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to subscribe to topic '{topic}': {ex.Message}");
            return StatusCode(500, new { Error = ex.Message });
        }
    }

    [HttpPost("unsubscribe")]
    public async Task<IActionResult> Unsubscribe([FromBody] string topic)
    {
        try
        {
            await _mqttService.UnsubscribeAsync(topic);

            // Xóa topic khỏi DB
            var clientId = (await _dbContext.Connections.FirstOrDefaultAsync(c => c.Id == 1))?.ClientId;
            if (clientId != null)
            {
                var existingTopic = await _dbContext.SubscribedTopics
                    .FirstOrDefaultAsync(t => t.ClientId == clientId && t.Topic == topic);
                if (existingTopic != null)
                {
                    _dbContext.SubscribedTopics.Remove(existingTopic);
                    await _dbContext.SaveChangesAsync();
                }
            }

            return Ok();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to unsubscribe from topic '{topic}': {ex.Message}");
            return StatusCode(500, new { Error = ex.Message });
        }
    }

    [HttpPost("publish")]
    public async Task<IActionResult> Publish([FromBody] MqttMessage message)
    {
        try
        {
            await _mqttService.PublishAsync(message);

            // Lưu kết quả publish vào DB
            var clientId = (await _dbContext.Connections.FirstOrDefaultAsync(c => c.Id == 1))?.ClientId;
            if (clientId != null)
            {
                _dbContext.PublishResults.Add(new PublishResult
                {
                    Success = true,
                    Message = $"Đã gửi nội dung tới chủ đề {message.Topic}",
                    Timestamp = DateTime.UtcNow
                });
                await _dbContext.SaveChangesAsync();
            }

            return Ok();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to publish to topic '{message.Topic}': {ex.Message}");

            // Lưu kết quả lỗi
            var clientId = (await _dbContext.Connections.FirstOrDefaultAsync(c => c.Id == 1))?.ClientId;
            if (clientId != null)
            {
                _dbContext.PublishResults.Add(new PublishResult
                {
                    Success = false,
                    Message = $"Thất bại: {ex.Message}",
                    Timestamp = DateTime.UtcNow
                });
                await _dbContext.SaveChangesAsync();
            }

            return StatusCode(500, new { Error = ex.Message });
        }
    }

    [HttpGet("connection")]
    public async Task<IActionResult> GetConnection()
    {
        var connection = await _dbContext.Connections.FirstOrDefaultAsync(c => c.Id == 1);
        if (connection == null)
        {
            return NotFound();
        }

        return Ok(new MqttConnection
        {
            Id = connection.Id,
            BrokerUri = connection.BrokerUri,
            Port = connection.Port,
            ClientId = connection.ClientId,
            Username = connection.Username,
            Password = Decrypt(connection.Password)
        });
    }

    [HttpGet("subscribed-topics")]
    public async Task<IActionResult> GetSubscribedTopics()
    {
        var clientId = (await _dbContext.Connections.FirstOrDefaultAsync(c => c.Id == 1))?.ClientId;
        if (clientId == null)
        {
            return NotFound();
        }

        var topics = await _dbContext.SubscribedTopics
            .Where(t => t.ClientId == clientId)
            .Select(t => t.Topic)
            .ToListAsync();
        return Ok(topics);
    }

    [HttpGet("latest-messages")]
    public async Task<IActionResult> GetLatestMessages()
    {
        var clientId = (await _dbContext.Connections.FirstOrDefaultAsync(c => c.Id == 1))?.ClientId;
        if (clientId == null)
        {
            return NotFound();
        }

        var messages = await _dbContext.LatestMessages
            .Where(m => m.ClientId == clientId)
            .ToListAsync();
        return Ok(messages);
    }

    [HttpGet("publish-results")]
    public async Task<IActionResult> GetPublishResults()
    {
        var results = await _dbContext.PublishResults
            .OrderByDescending(r => r.Timestamp)
            .Take(20)
            .ToListAsync();
        return Ok(results.Select(r => new { r.Success, r.Message }));
    }

    [HttpPost("save-message")]
    public async Task<IActionResult> SaveMessage([FromBody] MqttMessageEntity message)
    {
        try
        {
            var clientId = (await _dbContext.Connections.FirstOrDefaultAsync(c => c.Id == 1))?.ClientId;
            if (clientId == null || message.ClientId != clientId)
            {
                return BadRequest("Invalid client ID");
            }

            var existingMessage = await _dbContext.LatestMessages
                .FirstOrDefaultAsync(m => m.ClientId == clientId && m.Topic == message.Topic);
            if (existingMessage != null)
            {
                existingMessage.Payload = message.Payload;
                existingMessage.Timestamp = message.Timestamp;
            }
            else
            {
                _dbContext.LatestMessages.Add(message);
            }
            await _dbContext.SaveChangesAsync();

            return Ok();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to save message: {ex.Message}");
            return StatusCode(500, new { Error = ex.Message });
        }
    }

    private static string Encrypt(string text)
    {
        using var aes = System.Security.Cryptography.Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes("your-32-byte-key-here-1234567890"); // 32 byte (giữ nguyên)
        aes.IV = Encoding.UTF8.GetBytes("1234567890123456"); // 16 byte
        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream();
        using var cs = new System.Security.Cryptography.CryptoStream(ms, encryptor, System.Security.Cryptography.CryptoStreamMode.Write);
        using var sw = new StreamWriter(cs);
        sw.Write(text);
        sw.Flush();
        cs.FlushFinalBlock();
        return Convert.ToBase64String(ms.ToArray());
    }

    private static string Decrypt(string encrypted)
    {
        using var aes = System.Security.Cryptography.Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes("your-32-byte-key-here-1234567890"); // 32 byte (giữ nguyên)
        aes.IV = Encoding.UTF8.GetBytes("1234567890123456"); // 16 byte
        using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream(Convert.FromBase64String(encrypted));
        using var cs = new System.Security.Cryptography.CryptoStream(ms, decryptor, System.Security.Cryptography.CryptoStreamMode.Read);
        using var sr = new StreamReader(cs);
        return sr.ReadToEnd();
    }
    [HttpGet("state")]
    public async Task<IActionResult> GetState()
    {
        var clientId = (await _dbContext.Connections.FirstOrDefaultAsync(c => c.Id == 1))?.ClientId;
        if (clientId == null)
        {
            return NotFound();
        }

        var connection = await _dbContext.Connections.FirstOrDefaultAsync(c => c.Id == 1);
        var topics = await _dbContext.SubscribedTopics
            .Where(t => t.ClientId == clientId)
            .Select(t => t.Topic)
            .ToListAsync();
        var messages = await _dbContext.LatestMessages
            .Where(m => m.ClientId == clientId)
            .ToListAsync();
        var results = await _dbContext.PublishResults
            .OrderByDescending(r => r.Timestamp)
            .Take(20)
            .Select(r => new { r.Success, r.Message })
            .ToListAsync();

        return Ok(new
        {
            Connection = connection != null ? new MqttConnection
            {
                Id = connection.Id,
                BrokerUri = connection.BrokerUri,
                Port = connection.Port,
                ClientId = connection.ClientId,
                Username = connection.Username,
                Password = Decrypt(connection.Password)
            } : null,
            Topics = topics,
            Messages = messages,
            Results = results
        });
    }
}