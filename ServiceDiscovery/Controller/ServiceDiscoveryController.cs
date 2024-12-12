using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ServiceDiscovery.Model;
using ServiceDiscovery.Services;

namespace ServiceDiscovery.Controller
{
    [ApiController]
    [Route("[controller]")]
    public class ServiceDiscoveryController : ControllerBase
    {
        private readonly ServiceDiscoveryService _serviceDiscoveryService;
        public ServiceDiscoveryController([FromBody] ServiceDiscoveryService serviceDiscoveryService)
        {
            _serviceDiscoveryService = serviceDiscoveryService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] MicroServiceInstance instance)
        {
            instance.LastHeartbeat = DateTime.Now;
            await _serviceDiscoveryService.Register(instance);
            return Ok("Service registered successfully");
        }

        [HttpDelete("deregister")]
        public async Task<IActionResult> Deregister(MicroServiceInstance instance)
        {
            await _serviceDiscoveryService.Deregister(instance);
            return Ok("Service deregistered successfully");
        }

        [HttpPost("heartbeat")]
        public async Task<IActionResult> ReceiveHeartbeat([FromQuery] string serviceName, [FromQuery] string ipAddress)
        {
            await _serviceDiscoveryService.ReceiveHeartbeat(serviceName, ipAddress);
            return Ok("Heartbeat received");
        }

    }
}