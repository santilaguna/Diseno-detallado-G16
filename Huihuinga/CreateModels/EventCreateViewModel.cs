using Microsoft.AspNetCore.Http;
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
        public Guid concreteConferenceId { get; set; }
        [Required]
        public string name { get; set; }
        [Required]
        public DateTime starttime { get; set; }
        [Required]
        public DateTime endtime { get; set; }
        [Required]
        public Guid Hallid { get; set; }

        public IFormFile Photo { get; set; }
    }
}
