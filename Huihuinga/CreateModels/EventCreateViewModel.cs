using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Huihuinga.Models
{
    public class EventCreateViewModel
    {
        public Guid id { get; set; }
        public Guid? concreteConferenceId { get; set; }
        [Required]
        [Remote(action: "VerifyNewEvent", controller: "Event", ErrorMessage = "Este evento ya existe", AdditionalFields = "concreteConferenceId")]
        public string name { get; set; }
        [Required]
        [Remote(action: "VerifyStartTime", controller: "Event", ErrorMessage = "Este evento ya existe")]
        public DateTime starttime { get; set; }
        [Required]
        public DateTime endtime { get; set; }
        [Required]
        public Guid Hallid { get; set; }

        public IFormFile Photo { get; set; }
    }
}
