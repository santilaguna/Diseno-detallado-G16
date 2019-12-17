using Huihuinga.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Huihuinga.Models
{
    public class HallCreateViewModel
    {

        public Guid id { get; set; }
        [Required]
        [Remote(action: "VerifyNewHall", controller: "Hall", ErrorMessage = "Este hall ya existe",
                AdditionalFields = "EventCenterid")]
        public string name { get; set; }

        public EventCenter EventCenter { get; set; }

        public Guid EventCenterid { get; set; }
        [Required]
        public int capacity { get; set; }
        [Required]
        public string location { get; set; }
        public bool projector { get; set; }
        [Required]
        public int plugs { get; set; }
        [Required]
        public int computers { get; set; }

        public IFormFile Photo { get; set; }

    }
}
