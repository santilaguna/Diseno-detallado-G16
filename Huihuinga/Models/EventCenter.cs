using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Huihuinga.Models
{
    public class EventCenter
    {
        [Required]
        public Guid id { get; set; }
        [Required]
        public string name { get; set; }
        [Required]
        public string address { get; set; }
    }
}
