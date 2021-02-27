using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VideoServerAPI.DTO.Server
{
    /// <summary>
    /// Classe de entrada e saída de um servidor.
    /// Quando entrada, não preciso do campo ServerId, por isso ele pode ser nulo.
    /// Quando a classe é usada como saída, esse campos é utilizado.
    /// </summary>
    public class ServerDTO
    {
        public Guid? ServerId { get; set; }
        public string Name { get; set; }
        public string Ip { get; set; }
        public int Port { get; set; }
    }
}
