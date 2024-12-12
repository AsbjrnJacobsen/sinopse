using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ServiceDiscovery.Model;

namespace ServiceDiscovery.Services
{
    public class ServiceDiscoveryService : IServiceDiscoveryService
    {
        private List<MicroServiceInstance> _instances = new List<MicroServiceInstance>();
        private readonly TimeSpan _heartbeatTimeout = TimeSpan.FromSeconds(30);

        public async Task Register(MicroServiceInstance instance)
        {

            System.Console.WriteLine("Service {0} at {1} registered", instance.ServiceName, instance.IpAddress);
            _instances.Add(instance);

            //TODO : Implement Service Discovery send updated list to LoadBalancer
            await NotifyGateway();
        }

        public async Task Deregister(MicroServiceInstance instance)
        {
            System.Console.WriteLine("Service {0} at {1} deregistered", instance.ServiceName, instance.IpAddress);
            _instances.RemoveAll(x => x.IpAddress == instance.IpAddress && x.ServiceName == instance.ServiceName);

            //TODO : Implement Service Discovery send updated list to LoadBalancer
            await NotifyGateway();
        }

        public async Task ReceiveHeartbeat(string serviceName, string ipAddress)
        {
            // Update LastHeartbeat for the specified service instance
            System.Console.WriteLine("Received heartbeat for {0} at {1}", serviceName, ipAddress);
            var instance = _instances.FirstOrDefault(x => x.ServiceName == serviceName && x.IpAddress == ipAddress);
            if (instance != null)
            {
                instance.LastHeartbeat = DateTime.Now;
            }
        }

        public async Task CleanStaleInstances()
        {
            // Remove instances that haven't sent a heartbeat within the timeout period
            System.Console.WriteLine("Cleaning stale instances...");
            var now = DateTime.Now;
            var instanceCount = _instances.Count();

            _instances.RemoveAll(x => now - x.LastHeartbeat > _heartbeatTimeout);

            if (_instances.Count != instanceCount)
            {
                await NotifyGateway();
            }
        }

        public async Task<List<MicroServiceInstance>> GetInstances()
        {
            return _instances;
        }

        private async Task NotifyGateway()
        {
            var gatewayUrl = "http://localhost:????/update-services";
            using (var httpClient = new HttpClient())
            {
                try
                {
                    var json = JsonSerializer.Serialize(_instances);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await httpClient.PostAsync(gatewayUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Gateway notified successfully.");
                    }
                    else
                    {
                        Console.WriteLine($"Failed to notify gateway: {response.ReasonPhrase}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error notifying gateway: {ex.Message}");
                }
            }
        }
    }
}