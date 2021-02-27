using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using VideoServerAPI.Data;

namespace VideoServerAPI.Controllers
{
    [ApiController]
    public class ApplicationControllerBase: ControllerBase
    {
        protected readonly VideoServerDbContext Context;
        protected readonly IMapper Mapper;

        
        public ApplicationControllerBase(VideoServerDbContext context, IMapper mapper)
        {
            Context = context;
            Mapper = mapper;

        }
    }
}