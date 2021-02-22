using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VideoServerAPI.DTO.Video
{
    public class VideoInputDTO
    {
        public string Description { get; set; }
        public string VideoDataBase64 { get; set; }
    }   
}
