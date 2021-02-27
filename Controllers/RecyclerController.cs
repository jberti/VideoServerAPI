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
    public class RecyclerController : ControllerBase
    {
        private readonly IServiceScopeFactory _scopeFactory;        
        BlockingCollection<Guid> _recyclableVideosGuidList;

        public RecyclerController(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
            _recyclableVideosGuidList = new BlockingCollection<Guid>();            
        }

        [Route("process/{days}")]
        [HttpPost]
        public async Task<IActionResult> RecycleVideos(int days)
        {
            return await DoRecycleVideos(days);
        }

        [Route("status")]
        [HttpGet]
        public string GetStatus()
        {
            if (_recyclableVideosGuidList.Count > 0)
            {
                return "Running";
            }
            else
            {
                return "Not Running";
            }
        }

        private async Task<IActionResult> DoRecycleVideos(int days)
        {
            try
            {
                await BuildRecyclableVideosListAsync(days);
                await DeleteVideosToBeRecycled();

                return StatusCode(202);
            }
            catch (Exception)
            {
                return BadRequest(); 
            }
        }

        private async Task BuildRecyclableVideosListAsync(int days)
        {            
            using var dbContext = GetNewDbContext();
            var videoList = await dbContext.Videos.Where(video => video.DateAdded.AddDays(days) < DateTime.Today).ToListAsync();
            foreach (Video video in videoList)
            {
                _recyclableVideosGuidList.TryAdd(video.VideoId);
            }            
        }

        private async Task DeleteVideosToBeRecycled()
        {            
            while (!_recyclableVideosGuidList.IsCompleted)
            {
                _recyclableVideosGuidList.TryTake(out Guid videoId, 100);
                if (videoId != Guid.Empty)
                {
                    await DeleteVideo(videoId);
                }
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
