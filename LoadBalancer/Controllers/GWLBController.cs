using LoadBalancer.Model;
using Microsoft.AspNetCore.Mvc;

namespace LoadBalancer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GWLBController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly Dictionary<string, int> _roundRobinIndex = new();
    
    private List<MicroServiceInstance> _microServiceInstances = new();
    public GWLBController(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    [HttpPost("updateInstance")]
    public async Task<IActionResult> UpdateInstance([FromBody] List<MicroServiceInstance> incInstance)
    {
        _microServiceInstances = incInstance;
        return await Task.FromResult<IActionResult>(Ok());
    }
    
    [HttpGet("order/{action}")]
    public async Task<IActionResult> ForwardToOrderService(string action, [FromQuery] int id = 0)
    {
        return await ForwardRequest("OrderService", "Order", action, id);
    }

    [HttpGet("inventory/{action}")]
    public async Task<IActionResult> ForwardToInventoryService(string action, [FromQuery] int id = 0)
    {
        return await ForwardRequest("InventoryService", "Inventory", action, id);
    }


    private async Task<IActionResult> ForwardRequest(string serviceName, string controllerName, string action, int id)
    {
        // Hent services fra Service Discovery
         var instances = await GetServiceInstances(serviceName);
        
        if (instances == null || instances.Count == 0)
        {
            return StatusCode(503, $@"The service ""{serviceName}"" is not available.");
        }
        
        // Round Robin Load Balancing
        var instanceUrl = GetNextInstance(serviceName, instances);
        
        // Forward the request
        var targetUrl = $"{instanceUrl}/api/{controllerName}/{action}";
        if(id > 0) targetUrl += $"?id={id}";

        try
        {
            HttpResponseMessage response;

            // Determine HTTP method from the current request
            switch (HttpContext.Request.Method.ToUpper())
            {
                case "GET":
                    Console.WriteLine($"Get case - {serviceName}");
                    response = await _httpClient.GetAsync(targetUrl);
                    break;
                case "POST":
                    var postContent = new StreamContent(HttpContext.Request.Body);
                    postContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(HttpContext.Request.ContentType);
                    response = await _httpClient.PostAsync(targetUrl, postContent);
                    break;
                case "PUT":
                    var putContent = new StreamContent(HttpContext.Request.Body);
                    putContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(HttpContext.Request.ContentType);
                    response = await _httpClient.PutAsync(targetUrl, putContent);
                    break;
                case "DELETE":
                    response = await _httpClient.DeleteAsync(targetUrl);
                    break;
                default:
                    return StatusCode(405, "Method Not Allowed"); // Return 405 if method not supported
            }

            // Handle response from the target service
            var content = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, content);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Exception in forwarding request: {e.Message}");
            return StatusCode(500, $"Error forwarding request: {e.Message}");
        }
    }

    private async Task<IList<string>> GetServiceInstances(string serviceName)
    {
        /*
        // Get info from ServDisco 
        //var response = await _httpClient.GetAsync($"{_serviceDiscoveryUrl}{serviceName}");
        var response ;
        
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }
        
        return await response.Content.ReadFromJsonAsync<List<string>>(); */
        return null;
    }

    private string GetNextInstance(string serviceName, IList<string> instances)
    {
        var index = _roundRobinIndex.GetValueOrDefault(serviceName, -1);
        index = (index + 1) % instances.Count;
        
        _roundRobinIndex[serviceName] = index;
        return instances[index];
    }
}