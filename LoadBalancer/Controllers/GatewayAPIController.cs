using Microsoft.AspNetCore.Mvc;
using ServiceDiscovery.Model;

namespace LoadBalancer;

[ApiController]
[Route("api/[controller]")]
public class GatewayAPIController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly string _serviceDiscoveryUrl = "http://servicediscovery/api/services/";
    private readonly Dictionary<string, int> _roundRobinIndex = new();
    private List<MicroServiceInstance> _microServiceInstances = new();
    
    public GatewayAPIController()
    {
    }

    [HttpPost("{incInstance}")]
    public async Task<IActionResult> UpdateInstance([FromBody] List<MicroServiceInstance> incInstance)
    {
        _microServiceInstances = incInstance;
        return Ok();
    }
}