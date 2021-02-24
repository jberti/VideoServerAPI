using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VideoServerAPI.DTO.Server
{
    public class ServerDTO
    {
        public Guid? ServerId { get; set; }
        public string Name { get; set; }
        public string Ip { get; set; }
        public int Port { get; set; }
    }
}
