﻿using System;
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

        [Route("available/{serverId}")]
        [HttpGet]        
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
            if (await ServerExistsAsync(serverDto.Ip, serverDto.Port))
            {
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

            return CreatedAtAction("GetServer", new { serverId = server.Id }, server);
        }

        [HttpPost]
        [Route("{serverId}/videos")]
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
                catch
                {
                    throw;                    
                }
            }
            return Ok();            
        }

        // Pela documentação apresentada, não existe situação de existir um vídeo que não esteja associado a um servidor.
        // Por isso, e somente isso, acredito que não seria necessário passar o id do servidor como parâmetro, o id do video seria suficiente.
        // De qualquer forma meu modelo de dados permite saber qual o servidor que possui um determinado video, porque um video tem o id de seru servidor e
        // aí eu consigo validar o id do servidor.       
        [HttpGet]
        [Route("{serverId}/videos/{videoId}")]                
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
        [HttpGet]
        [Route("{serverId}/videos")]
        public async Task<ActionResult<List<VideoInformationDTO>>> GetServerVideos(Guid serverId)
        {
            Server server;
            List<VideoInformationDTO> videoList;

            server = await _context.Servers.FindAsync(serverId);
            if (server == null)
            {
                return NotFound();
            }
            else
            {
                // fazer mapping
                videoList = new List<VideoInformationDTO>();
                foreach (Video video in server.Videos)
                {
                    videoList.Add(new VideoInformationDTO
                    {
                        VideoId = video.Id,
                        Description = video.Description,
                        SizeInBytes = video.VideoContent.Length
                    });                    
                }
                return videoList;
            }
        }
               

        [HttpDelete]
        [Route("{serverId}/videos/{videoId}")]
        // Situação análoga ao método GetVideoInformation. A partir do id do video eu sei qual o servidor.
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
            catch 
            {
                throw;
            }

            return Ok();

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
            try
            {
                await _context.SaveChangesAsync();
            }
            catch
            {
                throw;
            }

            return Ok();
        }

        
        private async Task<bool> ServerExistsAsync(string ip, int port)
        {
            return await _context.Servers.AnyAsync(server => server.Ip == ip && server.Port == port);
        }
    }
}
