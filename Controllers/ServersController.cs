using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VideoServerAPI.Data;
using VideoServerAPI.DTO.Server;
using VideoServerAPI.DTO.Video;
using VideoServerAPI.Models;

namespace VideoServerAPI.Controllers
{
    [Route("api/[controller]")]    
    public class ServersController : ApplicationControllerBase
    {
        public ServersController(VideoServerDbContext context, IMapper mapper): base(context, mapper)
        {
            
        }

        // GET: api/Servers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ServerDTO>>> GetServers()
        {          
            var servers = (await Context.Servers.ToListAsync()).Select(server => Mapper.Map<ServerDTO>(server));

            return Ok(servers);
        }

        // GET: api/Servers/5
        [HttpGet("{serverId}")]
        public async Task<ActionResult<ServerDTO>> GetServer(Guid serverId)
        {            
            var server = await Context.Servers.FindAsync(serverId);
            if (server == null) return NotFound();

            var serverInfoDTO = Mapper.Map<ServerDTO>(server);
            
            return Ok(serverInfoDTO);
        }

        [Route("available/{serverId}")]
        [HttpGet]        
        public async Task<ActionResult<string>> IsServerOnline(Guid serverId)
        {
            var server = await Context.Servers.FindAsync(serverId);
            if (server == null) return "Not found";            
            
            var serverIp = server.Ip;
            var serverPort = server.Port;

            TcpClient tcpClient = new TcpClient(serverIp, serverPort);            

            try 
            {
                await tcpClient.ConnectAsync(serverIp, serverPort);
                if (tcpClient.Connected)
                {
                    return Ok("Server online");
                }
                else
                {
                    return NotFound("Server offline or unreachable");
                }

            }
            catch
            {
                tcpClient.Close();
                return BadRequest();
            }   
            
        }

        // POST: api/Servers        
        [HttpPost]
        public async Task<ActionResult<ServerDTO>> AddServer([FromBody]ServerDTO serverDto)
        {
            if (await ServerExistsAsync(serverDto.Ip, serverDto.Port))
            {
                return StatusCode(409, "Server already exists.");
            }

            var server = Mapper.Map<Server>(serverDto);
            server.ServerId = Guid.NewGuid();

            Context.Servers.Add(server);
            Context.Entry(server).State = EntityState.Added;
            
            try
            {
                await Context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetServer), new { serverId = server.ServerId }, server);
            }
            catch
            {
                return BadRequest();
            }            
        }        

        // DELETE: api/Servers/5
        [HttpDelete("{serverId}")]
        public async Task<IActionResult> DeleteServer(Guid serverId)
        {
            var server = await Context.Servers.FindAsync(serverId);
            if (server == null)  return NotFound("Server "+serverId+" not found");
                        
            Context.Entry(server).State = EntityState.Deleted;

            try
            {
                await Context.SaveChangesAsync();
            }
            catch
            {
                throw;
            }

            return Ok();
        }

        /// <summary>
        /// A regra é que não podem haver servidores com mesmo ip e porta.
        /// </summary>
        private async Task<bool> ServerExistsAsync(string ip, int port)
        {
            return await Context.Servers.AnyAsync(server => server.Ip == ip && server.Port == port);
        }
    }
}
