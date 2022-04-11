using Excersize18IndustrialMachine.TableModels;
using IndustrialMachine.Data.Dto;
using IndustrialMachine.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Excersize18IndustrialMachine.Helpers;
public static class DeviceConverters
{
    public static TableDevice GetTableDevice(this Device device, string partitionKey)
    {
        return new TableDevice()
        {
            Name = device.Name,
            RowKey = device.Id.ToString(),
            Status = device.Status,
            Data = device.Data,
            PartitionKey = partitionKey
        };
    }

    public static Device GetDevice(this TableDevice tableDevice)
    {
        return new Device()
        {
            Name = tableDevice.Name,
            Id = Guid.Parse(tableDevice.RowKey),
            Status = tableDevice.Status,
            Data = tableDevice.Data,
            SendData = null
        };
    }
    
    public static Device ConvertFromDeviceDto(this DeviceDto dto)
    {
        return new Device()
        {
            Name = dto.Name,
            Status = false,
            Data = Encoding.ASCII.GetBytes(dto.Data),
            SendData = null
        };
    }
}
