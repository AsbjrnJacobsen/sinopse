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
    [Route("[controller]")]
    public class InventoryController : Controller
    {
        public InventoryController()
        {
        }

        [HttpGet]
        public async Task<Inventory> GetInventory(int id)
        {
            int delay = new Random().Next(1000, 5000); 
            await Task.Delay(delay); 
            return new Inventory();
        }

        [HttpGet]
        public async Task<List<Inventory>> GetAllInventory()
        {
            int delay = new Random().Next(1000, 5000); 
            await Task.Delay(delay); 
            return new List<Inventory>();
        }

        [HttpPost]
        public async Task<IActionResult> CreateInventory()
        {
            int delay = new Random().Next(1000, 5000); 
            await Task.Delay(delay); 
            return await Task.FromResult(Ok());
        }

        [HttpPut]
        public async Task<IActionResult> UpdateInventory()
        {
            int delay = new Random().Next(1000, 5000); 
            await Task.Delay(delay); 
            return await Task.FromResult(Ok());
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteInventory()
        {
            int delay = new Random().Next(1000, 5000); 
            await Task.Delay(delay); 
            return await Task.FromResult(Ok());
        }
    }
}