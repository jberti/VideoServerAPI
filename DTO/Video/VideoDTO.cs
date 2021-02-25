using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VideoServerAPI.DTO.Video
{
    public class VideoDTO
    {
        public Guid? VideoId { get; set; }
        public string Description { get; set; }
        public string VideoDataBase64 { get; set; }
        public DateTime DateAdded { get; set; }
        public int? SizeInBytes { get; set; }
    }
}
