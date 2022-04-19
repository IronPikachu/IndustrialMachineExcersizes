using IndustrialMachine.Data.Models;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Excersize18IndustrialMachine;
public static partial class DeviceApi
{
    [FunctionName("QueueUpdate")]
    public static async Task QueueUpdate(
        [QueueTrigger("Deleted", Connection = "AzureWebJobsStorage")]
                Device device,
        [Blob("deleted", Connection = "AzureWebJobsStorage")]
                CloudBlobContainer blobContainer,
                ILogger log)
    {
        await blobContainer.CreateIfNotExistsAsync();
        var blob = blobContainer.GetBlockBlobReference($"{device.Id}.txt");

        StringBuilder sb = new StringBuilder($"{device.Name} - {device.Id}\n");

        sb.Append($"{System.Text.Encoding.ASCII.GetString( device.Data)}");

        await blob.UploadTextAsync(sb.ToString());
    }
}
