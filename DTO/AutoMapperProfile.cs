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

            ///Essa regra de map é para quando um objeto tipo VideoDTO é usado como saída, o campo SizeInBytes é calculado a partir do tamanho
            /// do array de bytes armazenado.
            CreateMap<Models.Video, VideoDTO>().ForMember(dest => dest.SizeInBytes, options => options.MapFrom(source => source.VideoContent.Length)).ReverseMap();
        }
    }
}
