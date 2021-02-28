using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VideoServerAPI.Data;
using VideoServerAPI.Models;
using VideoServerAPI.Services;

namespace VideoServerAPI.Controllers
{
    /// <summary>
    /// Minha primeira tentativa foi tentar usar um backgroundcontroller, mas o mesmo não pode ser acessado dentro do contexto da aplicação.
    /// Fiz entao um controle comum utilizando uma BlockinCollection pois ela é thread safe e isso pode me resolver situações de paralelimos.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]    
    public class RecyclerController : ApplicationControllerBase
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private RecylerInterPoser _recyclerInterposer;
        

        public RecyclerController(VideoServerDbContext context, IMapper mapper, IServiceScopeFactory scopeFactory, RecylerInterPoser recylerInterPoser) : base(context, mapper)
        {
            _scopeFactory = scopeFactory;
            _recyclerInterposer = recylerInterPoser;           
        }

        [Route("process/{days}")]
        [HttpPost]
        public async Task<IActionResult> RecycleVideos(int days)
        {
            var videoList = await Context.Videos.Where(video => video.DateAdded.AddDays(days) < DateTime.Today).ToListAsync();
            foreach (Video video in videoList)
            {
                _recyclerInterposer.Videos.Add(video.VideoId);
            }

            return StatusCode(202);
        }

        [Route("status")]
        [HttpGet]
        public string GetStatus()
        {
            if (_recyclerInterposer.Videos.Count > 0)
            {
                return "Running";
            }
            else
            {
                return "Not Running";
            }
        }

        private async Task DeleteVideo(Guid videoId)
        {   
            //Propositalmente, para cada video que vou deletar, crio uma nova instânca do DBContext.
            //O motivo é que isso evita problemas de paralelismo.
            using var newContext = GetNewDbContext();
            var video = await (newContext.FindAsync<Video>(videoId));
            newContext.Entry(video).State = EntityState.Deleted;
            
            try
            {
                await newContext.SaveChangesAsync();                
            }
            catch
            {
                throw;    
            }
        }

        private VideoServerDbContext GetNewDbContext()
        {
            var scope = _scopeFactory.CreateScope();
            return scope.ServiceProvider.GetRequiredService<VideoServerDbContext>();
        }
    }
}
