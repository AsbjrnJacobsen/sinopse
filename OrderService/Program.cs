using System.Text;
using System.Text.Json;
using OrderService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
    var task = RedisterAndDeredister("");
    task.Wait(); // Wait for the task to complete
};


await Task.Run(async () =>
{
    await RedisterAndDeredister("");
});

static async Task RedisterAndDeredister(string Url)
{
    try
    {
        // Wait for the app to fully start
        await Task.Delay(5000); // Adjust delay as needed for app readiness

        var ipFinder = new ReplicaIpFinder();

        // Get own IP address
        string ownIp = ipFinder.GetOwnIpAddress();

        // Define the target API URL
        string targetApiUrl = Url; // Replace with your target API's URL

        // Create the payload
        var payload = new { ip = ownIp };
        string jsonPayload = JsonSerializer.Serialize(payload);

        // Send HTTP POST request
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