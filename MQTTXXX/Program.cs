using Microsoft.EntityFrameworkCore;
using MQTTXXX;
using MQTTXXX.Client;
using MQTTXXX.Components;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();
builder.Services.AddSignalR();
builder.Services.AddMudServices();
builder.Services.AddHttpClient("MqttApi", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"] ?? "https://192.168.110.225:7197/");
});
builder.Services.AddControllers();
builder.Services.AddSingleton<MqttService>();
builder.Services.AddScoped<MqttServiceClient>();

// Cấu hình SQLite cho MqttDbContext
builder.Services.AddDbContext<MqttDbContext>(options =>
{
    options.UseSqlite("Data Source=mqtt.db");
});

var app = builder.Build();

// Tạo cơ sở dữ liệu nếu chưa tồn tại
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<MqttDbContext>();
    dbContext.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(MQTTXXX.Client._Imports).Assembly);
app.MapHub<MqttHub>("/mqtthub");

app.MapControllers();
app.Run();