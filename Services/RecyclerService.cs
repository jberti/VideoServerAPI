using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VideoServerAPI.Controllers;
using VideoServerAPI.Data;
using VideoServerAPI.Models;

namespace VideoServerAPI.Services
{
    public class RecyclerService : BackgroundService, IHostedService
    {
        
        private readonly IServiceScopeFactory _scopeFactory;
        private RecylerInterPoser _recyclerInterposer;
        private int _days;
        public string status { get; set; }
        
        public RecyclerService(IServiceScopeFactory scopeFactory, RecylerInterPoser recylerInterPoser)
        {
            _scopeFactory = scopeFactory;
            _recyclerInterposer = recylerInterPoser;
        }

        private VideoServerDbContext GetNewDbContext()
        {
            var scope = _scopeFactory.CreateScope();
            return scope.ServiceProvider.GetRequiredService<VideoServerDbContext>();
        }
        
        

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Guid videoId;
             

            await Task.Yield();
            status = "Not Running";

            while (!stoppingToken.IsCancellationRequested)            {   
                
                videoId =_recyclerInterposer.Videos.Take();
                if (videoId != Guid.Empty)
                {
                    status = "Running";
                    await DeleteVideo(videoId);
                    status = "Not Running";
                }
            }
        }

        private async Task DeleteVideo(Guid videoId)
        {
            
            using var newContext = GetNewDbContext();
            newContext.Videos.Remove(await(newContext.FindAsync<Video>(videoId)));
            try
            {
                await newContext.SaveChangesAsync();
            }
            catch
            {

            }
        }

       
    }

}
