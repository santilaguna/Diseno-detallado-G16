using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Huihuinga.Models
{
    public class Hall
    {   [Required]
        public Guid id { get; set; }
        [Required]
        public string name { get; set; }
        [Required]
        public EventCenter EventCenter { get; set; }
        [Required]
        public int capacity { get; set; }
        [Required]
        public string location { get; set; }
        public bool projector { get; set; }
        [Required]
        public int plugs { get; set; }
        [Required]
        public int computers { get; set; }

        

    }
}
