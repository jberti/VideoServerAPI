using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VideoServerAPI.Models;

namespace VideoServerAPI.Data
{
    public class VideoServerDbContext : DbContext
    {
        public VideoServerDbContext(DbContextOptions<VideoServerDbContext> options) : base(options)
        {

        }

        public DbSet<Video> Videos { get; set; }

        public DbSet<Server> Servers { get; set; }
        
    }
}
