using System.Text;
using System.Text.Json;
using OrderService;
using OrderService.Model;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();

AppDomain.CurrentDomain.ProcessExit += (sender, args) =>
{
    var task = RedisterAndDeredister("http://servicediscovery:8085/ServiceDiscovery/deregister");
    task.Wait(); // Wait for the task to complete
};


await Task.Run(async () =>
{
    await RedisterAndDeredister("http://servicediscovery:8085/ServiceDiscovery/register");
});

static async Task RedisterAndDeredister(string Url)
{
    try
    {
        await Task.Delay(5000);
        var ipFinder = new ReplicaIpFinder();

        string ownIp = ipFinder.GetOwnIpAddress();

        string targetApiUrl = Url; 

        var payload = new MicroServiceInstance { IpAddress = ownIp, Port = 8082, ServiceName = "OrderService" };
        string jsonPayload = JsonSerializer.Serialize(payload);

        using var httpClient = new HttpClient();
        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync(targetApiUrl, content);

        response.EnsureSuccessStatusCode();
        Console.WriteLine("Successfully sent IP to the target API.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error sending IP to the target API: {ex.Message}");
    }
}