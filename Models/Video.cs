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
        public Guid Id { get; set; }
        [Required]
        public string Descricao { get; set; }

        /// <summary>
        /// ○	Conteúdo binário dos vídeos deve ser mantido no sistema de arquivos
        /// </summary>
        [Required]       
        public string NomeArquivoVideo { get; set; }
        
        public Server Server { get; set; }
        public Guid ServerId { get; set; }
    }
}
