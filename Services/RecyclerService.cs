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
        public BlockingCollection<Guid> Videos { get; set; }
        private readonly IServiceScopeFactory _scopeFactory;
        private int _days;
        
        public RecyclerService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
            Videos = new BlockingCollection<Guid>();
        }

        private VideoServerDbContext GetNewDbContext()
        {
            var scope = _scopeFactory.CreateScope();
            return scope.ServiceProvider.GetRequiredService<VideoServerDbContext>();
        }
        
        public async void RecycleVideos(int days)
        {
            _days = days;
            using var dbContext = GetNewDbContext();
            var videoList = await dbContext.Videos.Where(video => video.DateAdded.AddDays(_days) < DateTime.Today).ToListAsync();
            foreach (Video video in videoList)
            {
                Videos.TryAdd(video.VideoId);
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Guid videoId;

            await Task.Yield();
            while (!Videos.IsCompleted)
            {                
                Videos.TryTake(out videoId,100);
                if (videoId != Guid.Empty)
                {
                    await DeleteVideo(videoId);
                }
            }
        }

        private async Task DeleteVideo(Guid videoId)
        {
            var optionsBuilder = new DbContextOptionsBuilder<VideoServerDbContext>();
            optionsBuilder.UseSqlServer("DefaultConnection");
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
