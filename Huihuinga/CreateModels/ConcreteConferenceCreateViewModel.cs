﻿using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;


namespace Huihuinga.Models
{
    public class ConcreteConferenceCreateViewModel
    {
        public Guid abstractConferenceId { get; set; }
        [Required]
        public string name { get; set; }
        [Required]
        public DateTime starttime { get; set; }
        [Required]
        public DateTime endtime { get; set; }
        [Required]
        public int Maxassistants { get; set; }
        public IFormFile Photo { get; set; }
    }
}