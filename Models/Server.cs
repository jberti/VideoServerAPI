﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VideoServerAPI.Models
{
    /// <summary>
    /// 	Um servidor é composto por ID (guid), nome (string), endereço IP (string), porta IP (integer)
    /// </summary>
    public class Server
    {
        [Key]
        public Guid ServerId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Ip { get; set; }
        [Required]
        public int Port { get; set; }
        public List<Video> Videos { get; } = new();        
    }
}
