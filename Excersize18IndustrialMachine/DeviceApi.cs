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
using Microsoft.Azure.Storage.Blob;

namespace Excersize18IndustrialMachine;
public static partial class DeviceApi
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

    [FunctionName("GetDetails")]
    public static async Task<IActionResult> GetDetails(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "details/{id}")]
                HttpRequest req,
        [Table("Machines", Connection = "AzureWebJobsStorage")]
                CloudTable table,
        [Table("History", Connection = "AzureWebJobsStorage")]
                CloudTable tableHistory,
            Guid id,
            ILogger log)
    {
        var query = new TableQuery<TableDevice>();
        var res = await table.ExecuteQuerySegmentedAsync(query, null);

        Device response = res.Select(DeviceConverters.GetDevice).ToList().First(d => d.Id == id);

        var dataQuery = new TableQuery<TableDevice>();

        var data = (await tableHistory.ExecuteQuerySegmentedAsync(dataQuery, null)).Select(t => t.Data).ToList();

        DetailsDto dto = new DetailsDto()
        {
            Device = response,
            DeviceData = data
        };

        return new OkObjectResult(dto);
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

        if (string.IsNullOrEmpty(itemToUpdate.SendData)) return new NoContentResult();

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
                CloudTable tableMachine,
        [Table("History", Connection = "AzureWebJobsStorage")]
                CloudTable tableHistory,
            Guid id,
            ILogger log)
    {
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

        var deviceToUpdate = JsonConvert.DeserializeObject<Device>(requestBody);

        if (deviceToUpdate is null || deviceToUpdate.Id != id) return new BadRequestResult();

        deviceToUpdate.Data = System.Text.Encoding.ASCII.GetBytes(deviceToUpdate.SendData);
        deviceToUpdate.SendData = null;

        var itemEntity = deviceToUpdate.GetTableDevice("Banan");
        var historyDevice = deviceToUpdate.GetTableDevice($"{deviceToUpdate.Id}");
        historyDevice.RowKey = DateTime.Now.Ticks.ToString();
        itemEntity.ETag = "*";

        await tableHistory.ExecuteAsync(TableOperation.Insert(historyDevice));

        // Get item from Table, store that item in a Table Storage acting as a history

        var operation = TableOperation.Replace(itemEntity);
        await tableMachine.ExecuteAsync(operation);

        return new NoContentResult();
    }

    [FunctionName("Delete")]
    public static async Task<IActionResult> Delete(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "delete/{id}")]
                HttpRequest req,
        [Table("Machines", "Device", "{id}", Connection = "AzureWebJobsStorage")]
                TableDevice tableDevice,
        [Table("Machines", Connection = "AzureWebJobsStorage")]
                CloudTable table,
        [Queue("todoqueue", Connection = "AzureWebJobsStorage")]
                IAsyncCollector<Device> queueDevices,
            Guid id,
            ILogger log)
    {
        if (tableDevice == null) return new BadRequestResult();

        var operation = TableOperation.Delete(tableDevice);
        var res = await table.ExecuteAsync(operation);

        return new NoContentResult();
    }

}

