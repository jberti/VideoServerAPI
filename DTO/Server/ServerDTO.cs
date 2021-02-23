using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VideoServerAPI.Dto.Server
{
    public class ServerDTO
    {
        public string Name { get; set; }
        public string Ip { get; set; }
        public int Port { get; set; }
    }
}
