using IndustrialMachine.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialMachine.Data.Dto;
public class DetailsDto
{
    public Device Device { get; set; }
    public List<byte[]> DeviceData { get; set; }
}
