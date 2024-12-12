using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class testController : Controller
    {
        
        public testController()
        {
            
        }

        [HttpGet("get")]
        public async Task<string> get()
        {
            var ip = new ReplicaIpFinder().GetOwnIpAddress();
            return ip;
        }
    }
}