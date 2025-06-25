using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MQTTXXX.Client;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddHttpClient("MqttApi", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"] ?? "https://192.168.110.225:7197/");
});
builder.Services.AddMudServices();
builder.Services.AddScoped<MqttServiceClient>();

await builder.Build().RunAsync();