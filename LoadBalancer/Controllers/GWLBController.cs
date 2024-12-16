using System.Security.Cryptography;
using LoadBalancer.Model;
using LoadBalancer.Service;
using Microsoft.AspNetCore.Mvc;

namespace LoadBalancer.Controllers;

[ApiController]
[Route("[controller]")]
public class GWLBController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly Instances _instances;

    public GWLBController(HttpClient httpClient, Instances instances)
    {
        _httpClient = httpClient;
        _instances = instances;
    }

    [HttpPost("updateInstance")]
    public async Task<IActionResult> UpdateInstance([FromBody] List<MicroServiceInstance> incInstance)
    {
        _instances.OrderServiceInstances.Clear();
        _instances.InventoryServiceInstances.Clear();

        foreach (var instance in incInstance)
        {
            if (instance.ServiceName == "OrderService")
            {
                _instances.OrderServiceInstances.Add(instance);
            }
            else if (instance.ServiceName == "InventoryService")
            {
                _instances.InventoryServiceInstances.Add(instance);
            }
        }
        return await Task.FromResult<IActionResult>(Ok());
    }
    
    [HttpGet("order")]
    public async Task<IActionResult> ForwardToOrderService(string action, [FromQuery] int id = 0)
    {
        return await ForwardRequest("OrderService", "Order", action, id);
    }

    [HttpGet("inventory")]
    public async Task<IActionResult> ForwardToInventoryService(string action, [FromQuery] int id = 0)
    {
        return await ForwardRequest("InventoryService", "Inventory", action, id);
    }


    private async Task<IActionResult> ForwardRequest(string serviceName, string controllerName, string action, int id)
    {
        // Round Robin Load Balancing
        var instanceUrl = GetNextInstance(serviceName);
        
        // Forward the request
        var targetUrl = $"http://{instanceUrl.IpAddress}:{instanceUrl.Port}/{controllerName}/{action}";
        if(id > 0) targetUrl += $"?id={id}";
        System.Console.WriteLine($"Forwarding request to: {targetUrl}");
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

    private MicroServiceInstance GetNextInstance(string serviceName)
    {
        if (serviceName == "OrderService")
        {
            var index = _instances.RoundRobinIndex.GetValueOrDefault(serviceName, -1);

            System.Console.WriteLine($"Index {index}");
            System.Console.WriteLine($"OrderService Count {_instances.OrderServiceInstances.Count}");

            index = (index + 1) % _instances.OrderServiceInstances.Count;
            
            _instances.RoundRobinIndex[serviceName] = index;
            return _instances.OrderServiceInstances[index];   
        }
        else
        {
            var index = _instances.RoundRobinIndex.GetValueOrDefault(serviceName, -1);
            index = (index + 1) % _instances.InventoryServiceInstances.Count;
            
            System.Console.WriteLine($"Index {index}");
            System.Console.WriteLine($"OrderService Count {_instances.InventoryServiceInstances.Count}");

            _instances.RoundRobinIndex[serviceName] = index;
            return _instances.InventoryServiceInstances[index];
        }
    }
}