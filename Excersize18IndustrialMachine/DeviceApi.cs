using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using IndustrialMachine.Data.Models;
using Excersize18IndustrialMachine.Helpers;
using Excersize18IndustrialMachine.TableModels;
//using System.ComponentModel.DataAnnotations.Schema;
//using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.Cosmos.Table.Queryable;
using System.Linq;
using System.Collections.Generic;
using IndustrialMachine.Data.Dto;

namespace Excersize18IndustrialMachine
{
    public static class DeviceApi
    {
        [FunctionName("Post")]
        public static async Task<IActionResult> Post(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "post")] 
                HttpRequest req,
            [Table("Machines", Connection = "AzureWebJobsStorage")] 
                IAsyncCollector<TableDevice> table,
                ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            var createDevice = JsonConvert.DeserializeObject<DeviceDto>(requestBody);

            if (createDevice == null) return new BadRequestResult();

            var device = DeviceConverters.ConvertFromDeviceDto(createDevice);

            // Map or convert device to device table entity
            await table.AddAsync(device.GetTableDevice("Banan"));

            return new OkObjectResult(device);
        }

        [FunctionName("Get")]
        public static async Task<IActionResult> Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "get")]
                HttpRequest req,
            [Table("Machines", Connection = "AzureWebJobsStorage")]
                CloudTable table,
                ILogger log)
        {
            var query = new TableQuery<TableDevice>();
            var res = await table.ExecuteQuerySegmentedAsync(query, null);

            List<Device> response = res.Select(DeviceConverters.GetDevice).ToList();

            return new OkObjectResult(response);
        }

        [FunctionName("PutStatus")]
        public static async Task<IActionResult> PutStatus(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "putstat/{id}")] 
                HttpRequest req,
            [Table("Machines", Connection = "AzureWebJobsStorage")] 
                CloudTable table,
                Guid id,
                ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            var itemToUpdate = JsonConvert.DeserializeObject<Device>(requestBody);

            if (itemToUpdate is null || itemToUpdate.Id != id) return new BadRequestResult();

            var itemEntity = itemToUpdate.GetTableDevice("Banan");
            itemEntity.ETag = "*";

            var operation = TableOperation.Replace(itemEntity);
            await table.ExecuteAsync(operation);

            return new NoContentResult();
        }

        [FunctionName("PutData")]
        public static async Task<IActionResult> PutData(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "putdata/{id}")]
                HttpRequest req,
            [Table("Machines", Connection = "AzureWebJobsStorage")]
                CloudTable table,
                Guid id,
                ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            var itemToUpdate = JsonConvert.DeserializeObject<Device>(requestBody);

            if (itemToUpdate is null || itemToUpdate.Id != id) return new BadRequestResult();

            itemToUpdate.Data = System.Text.Encoding.ASCII.GetBytes(itemToUpdate.SendData);
            itemToUpdate.SendData = null;

            var itemEntity = itemToUpdate.GetTableDevice("Banan");
            itemEntity.ETag = "*";

            var operation = TableOperation.Replace(itemEntity);
            await table.ExecuteAsync(operation);

            return new OkObjectResult($"8 ball response: \"Reply hazy, try again\"");
        }
    }
}
