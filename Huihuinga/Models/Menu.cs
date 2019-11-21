using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Huihuinga.Models
{
    public class Menu
    {
        public Guid Id { get; set; }

        [Required]
        public string filename { get; set; }
        [Required]
        public string name { get; set; }

        public string menu { get; set; }

        public Event Event { get; set; }

        public Guid EventId { get; set; }
    }
}
