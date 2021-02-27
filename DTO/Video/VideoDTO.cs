using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VideoServerAPI.DTO.Video
{
    /// <summary>
    /// Classe de entrada e saída de um video.
    /// Quando entrada, só preciso dos campos Description e VideoDatabase64, por isso os outros são passíveis de serem nulos.
    /// Quando a classe é usada como saída, esses campos são utilizados.
    /// </summary>
    public class VideoDTO
    {
        public Guid? VideoId { get; set; }
        public string Description { get; set; }
        public string VideoDataBase64 { get; set; }
        public DateTime? DateAdded { get; set; }
        public int? SizeInBytes { get; set; }
    }
}
