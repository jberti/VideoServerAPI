using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VideoServerAPI.Models
{
    /// <summary>
    ///     Um arquivo de vídeo é composto por ID (guid), descrição (string) e conteúdo binário do vídeo
    /// </summary>
    public class Video
    {
        [Key]
        public Guid VideoId { get; set; }
        [Required]
        public string Description { get; set; }
                
        [Required]       
        public byte[] VideoContent { get; set; }

        public DateTime DateAdded { get; set; }
        
        public Server Server { get; set; }
        public Guid ServerId { get; set; }
    }
}
