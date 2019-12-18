using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Huihuinga.Models
{
    public class EventCenterCreateViewModel
    {
        [Required]
        public Guid id { get; set; }
        [Required]
        [Remote(action: "VerifyNewCenter", controller: "EventCenter", ErrorMessage = "Este centro ya existe")]
        public string name { get; set; }
        [Required]
        public string address { get; set; }
        public List<Hall> Halls { get; set; }

        public IFormFile Photo { get; set; }

        public int capacity { get; set; }
    }
}
