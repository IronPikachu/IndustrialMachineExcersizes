//using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Excersize18IndustrialMachine.TableModels
{
    public class TableDevice : TableEntity
    {
        public string Name { get; set; }
       // public Guid Id { get; set; }
        public bool Status { get; set; }
        public byte[] Data { get; set; }
    }
}