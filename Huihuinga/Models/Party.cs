using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Huihuinga.Models
{
    public class Party:Event
    {
        [Required]
        public string description { get; set; }
        [Required]
        public string image { get; set; }
    }
}
