using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using InventoryService.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace InventoryService.Controllers
{    
    [ApiController]
    [Route("[controller]")]
    public class InventoryController : Controller
    {
        public InventoryController()
        {
        }

        [HttpGet("GetInventory")]
        public async Task<Inventory> GetInventory(int id)
        {
            int delay = new Random().Next(1000, 5000); 
            await Task.Delay(delay); 
            return new Inventory();
        }

        [HttpGet("GetAllInventory")]
        public async Task<List<Inventory>> GetAllInventory()
        {
            int delay = new Random().Next(1000, 5000); 
            await Task.Delay(delay); 
            return new List<Inventory>();
        }

        [HttpGet("CreateInventory")]
        public async Task<IActionResult> CreateInventory([FromBody] Inventory inventory)
        {
            int delay = new Random().Next(1000, 5000); 
            await Task.Delay(delay); 
            return await Task.FromResult(Ok());
        }

        [HttpGet("UpdateInventory")]
        public async Task<IActionResult> UpdateInventory([FromBody] Inventory inventory)
        {
            int delay = new Random().Next(1000, 5000); 
            await Task.Delay(delay); 
            return await Task.FromResult(Ok());
        }

        [HttpGet("DeleteInventory")]
        public async Task<IActionResult> DeleteInventory([FromBody] Inventory inventory)
        {
            int delay = new Random().Next(1000, 5000); 
            await Task.Delay(delay); 
            return await Task.FromResult(Ok());
        }
    }
}