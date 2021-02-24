using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VideoServerAPI.DTO.Server;
using VideoServerAPI.DTO.Video;

namespace VideoServerAPI.DTO
{
    public class AutoMapperProfile: Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Models.Server, ServerDTO>().ReverseMap();             
            CreateMap<Models.Video, VideoDTO>().ForMember(dest => dest.SizeInBytes, options => options.MapFrom(source => source.VideoContent.Length)).ReverseMap();
        }
    }
}
