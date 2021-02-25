using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VideoServerAPI.Data;
using VideoServerAPI.DTO.Video;
using VideoServerAPI.Models;

namespace VideoServerAPI.Controllers
{
    [Route("api/servers")]
    [ApiController]
    public class VideosController : ApplicationControllerBase
    {

        public VideosController(VideoServerDbContext context, IMapper mapper) : base(context, mapper)
        {

        }

        [HttpPost]
        [Route("{serverId}/videos")]
        public async Task<IActionResult> Addvideo(Guid serverId, [FromBody] VideoDTO videoDTO)
        {
            var server = await Context.Servers.FindAsync(serverId);
            if (server == null) return NotFound();

            var video = Mapper.Map<Video>(videoDTO);
            video.VideoId = Guid.NewGuid();
            video.VideoContent = Convert.FromBase64String(videoDTO.VideoDataBase64);
            video.ServerId = serverId;
            video.DateAdded = DateTime.Today;

            Context.Entry(video).State = EntityState.Added;

            try
            {
                await Context.SaveChangesAsync();
            }
            catch
            {
                return BadRequest();
            }


            return Ok();
        }

        // Pela documentação apresentada, não existe situação de existir um vídeo que não esteja associado a um servidor.
        // Por isso, e somente isso, acredito que não seria necessário passar o id do servidor como parâmetro, o id do video seria suficiente.
        // De qualquer forma meu modelo de dados permite saber qual o servidor que possui um determinado video, porque um video tem o id de seru servidor e
        // aí eu consigo validar o id do servidor.       
        [HttpGet]
        [Route("{serverId}/videos/{videoId}")]
        public async Task<ActionResult<VideoDTO>> GetVideoInformation(Guid serverId, Guid videoId)
        {

            var server = await Context.Servers.FindAsync(serverId);
            if (server == null) return NotFound();

            var video = await Context.Videos.FindAsync(videoId);
            if (video == null) return NotFound();

            var videoDTO = Mapper.Map<VideoDTO>(video);

            return Ok(videoDTO);
        }

        [HttpGet]
        [Route("{serverId}/videos")]
        public async Task<ActionResult<IEnumerable<VideoDTO>>> GetServerVideos(Guid serverId)
        {
            var server = await Context.Servers.FindAsync(serverId);
            if (server == null) return NotFound();


            var videoList = (await Context.Videos.Where(video => video.ServerId == serverId).ToListAsync());

            var videoDTOList = videoList.Select(video => Mapper.Map<VideoDTO>(video));
            return Ok(videoDTOList);
        }

        [HttpGet]
        [Route("{serverId}/videos/{videoId}/binary")]
        public async Task<ActionResult> DownloadVideo(Guid serverId, Guid videoId)
        {
            var server = await Context.Servers.FindAsync(serverId);
            if (server == null) return NotFound();

            var video = await Context.Videos.FindAsync(videoId);
            if (video == null) return NotFound();

            return File(video.VideoContent, "application/octet-stream");
        }

        [HttpDelete]
        [Route("{serverId}/videos/{videoId}")]
        // Situação análoga ao método GetVideoInformation. A partir do id do video eu sei qual o servidor.
        public async Task<IActionResult> DeleteVideo(Guid serverId, Guid videoId)
        {
            Server server;
            Video video;

            server = await Context.Servers.FindAsync(serverId);
            if (server == null) return NotFound();

            video = await Context.Videos.FindAsync(videoId);
            if (video == null) return NotFound();

            server.Videos.Remove(video);
            //Context.Videos.Remove(video);
            Context.Entry(server).State = EntityState.Modified;
            Context.Entry(video).State = EntityState.Deleted;


            try
            {
                await Context.SaveChangesAsync();
            }
            catch
            {
                return BadRequest();
            }

            return Ok();
        }
    }
        
    
}
