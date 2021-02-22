using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VideoServerAPI.DTO.Video
{
    public class VideoInformationDTO
    {
        public Guid VideoId { get; set; }
        public string Description { get; set; }
        public int SizeInBytes { get; set; }
    }
}
