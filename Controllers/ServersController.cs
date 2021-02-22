using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VideoServerAPI.Data;
using VideoServerAPI.Dto.Server;
using VideoServerAPI.DTO.Video;
using VideoServerAPI.Models;

namespace VideoServerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServersController : ControllerBase
    {
        private readonly VideoServerDbContext _context;

        public ServersController(VideoServerDbContext context)
        {
            _context = context;
        }

        // GET: api/Servers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Server>>> GetServers()
        {
            return await _context.Servers.ToListAsync();
        }

        // GET: api/Servers/5
        [HttpGet("{serverId}")]
        public async Task<ActionResult<Server>> GetServer(Guid serverId)
        {
            var server = await _context.Servers.FindAsync(serverId);

            if (server == null)
            {
                return NotFound();
            }

            return server;
        }

        [HttpGet("{serverId}")]
        [Route("api/[controller]/available/{id}")]
        public async Task<ActionResult<string>> IsServerOnline(Guid serverId)
        {
            string response = "";

            var server = await _context.Servers.FindAsync(serverId);
            if (server == null)
            {
                response = "Not found";
            }
            else
            {
                var serverIp = server.Ip;
                var serverPort = server.Port;

                TcpClient telnet = new TcpClient(serverIp, serverPort);
                NetworkStream telnetStream = telnet.GetStream();
                string requestResponse = new StreamReader(telnetStream).ToString();

                string prompt = requestResponse.TrimEnd();
                prompt = requestResponse.Substring(prompt.Length - 1, 1);
                if (prompt == "$" || prompt == ">")
                {
                    response = "Server online";
                }
                else
                {
                    response = "Server offline or unreachable";
                }                    
            }

            return response;
        }

        // POST: api/Servers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Server>> PostServer([FromBody]ServerDTO serverDto)
        {
            if (ServerExists(serverDto.Ip, serverDto.Port)){
                return StatusCode(409, "Servidor já existente");
            }

            // TODO: Load state from previously suspended application
            Server server = new Server
            {
                Id = Guid.NewGuid(),
                Ip = serverDto.Ip,
                Name = serverDto.Name,
                Port = serverDto.Port
            };

            _context.Servers.Add(server);

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetServer", new { id = server.Id }, server);
        }

        [HttpPost]
        [Route("api/[controller]/{serverId}/videos")]
        public async Task<IActionResult> Addvideo(Guid serverId, [FromBody] VideoInputDTO videoDTO)
        {
            var server = await _context.Servers.FindAsync(serverId);
            if (server == null)
            {
                return NotFound();
            }
            else
            {
                Video video = new Video
                {
                    Id = new Guid(),
                    Description = videoDTO.Description,
                    VideoContent = Convert.FromBase64String(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(videoDTO.VideoDataBase64))) 
                };            

                server.Videos.Add(video); 
                _context.Entry(server).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;                    
                }
            }
            return NoContent();            
        }

        [HttpGet]
        [Route("api/[controller]/{serverId}/videos/{videoId}")]
        public async Task<ActionResult<VideoInformationDTO>> GetVideoInformation(Guid serverId, Guid videoId)
        {
            Server server;
            Video video;
            VideoInformationDTO videoInfoDTO;

            server = await _context.Servers.FindAsync(serverId);
            if (server == null)
            {
                return NotFound();
            }
            else
            {
                video = await _context.Videos.FindAsync(videoId);
                if (video == null)
                {
                    return NotFound();
                }
            }

            videoInfoDTO = new VideoInformationDTO
            {
                VideoId = video.Id,
                Description = video.Description,
                SizeInBytes = video.VideoContent.Length
            };
            
            return videoInfoDTO;    

        }

        [HttpDelete]
        [Route("/api/servers/{serverId}/videos/{videoId}")]
        public async Task<IActionResult> DeleteVideo(Guid serverId, Guid videoId)
        {
            Server server;
            Video video;

            server = await _context.Servers.FindAsync(serverId);
            if (server == null)
            {
                return NotFound();
            }
            else
            {
                video = await _context.Videos.FindAsync(videoId);
                if (video == null)
                {
                    return NotFound();
                }
            }
            server.Videos.Remove(video);
            _context.Videos.Remove(video);
            _context.Entry(server).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return NoContent();

        }

        // DELETE: api/Servers/5
        [HttpDelete("{serverId}")]
        public async Task<IActionResult> DeleteServer(Guid serverId)
        {
            var server = await _context.Servers.FindAsync(serverId);
            if (server == null)
            {
                return NotFound();
            }

            _context.Servers.Remove(server);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ServerExists(string ip, int port)
        {
            return _context.Servers.Any(server => server.Ip == ip && server.Port == port);
        }
    }
}
