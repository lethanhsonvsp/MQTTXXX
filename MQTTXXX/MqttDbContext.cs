using Microsoft.EntityFrameworkCore;
using MQTTXXX.Shared;

namespace MQTTXXX;

public class MqttDbContext : DbContext
{
    public DbSet<MqttConnection> Connections { get; set; }
    public DbSet<SubscribedTopic> SubscribedTopics { get; set; }
    public DbSet<MqttMessageEntity> LatestMessages { get; set; }
    public DbSet<PublishResult> PublishResults { get; set; }

    public MqttDbContext(DbContextOptions<MqttDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MqttConnection>().HasKey(c => c.Id);
        modelBuilder.Entity<SubscribedTopic>().HasKey(t => new { t.ClientId, t.Topic });
        modelBuilder.Entity<MqttMessageEntity>().HasKey(m => new { m.ClientId, m.Topic });
        modelBuilder.Entity<PublishResult>().HasKey(r => r.Id);

        // Thêm index để tăng hiệu suất truy vấn
        modelBuilder.Entity<SubscribedTopic>().HasIndex(t => t.ClientId);
        modelBuilder.Entity<MqttMessageEntity>().HasIndex(m => m.ClientId);
        modelBuilder.Entity<PublishResult>().HasIndex(r => r.Timestamp);
    }
}
