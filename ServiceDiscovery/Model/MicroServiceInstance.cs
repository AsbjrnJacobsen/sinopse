using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceDiscovery.Model
{
    public class MicroServiceInstance
    {
        public required string InstanceId { get; set; }
        public required string ServiceName { get; set; }
        public required string IpAddress { get; set; }
        public required int Port { get; set; }
        public required string HealthCheckUrl { get; set; }
        public DateTime LastHeartbeat { get; set; }
    }
}