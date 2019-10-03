using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Huihuinga.Models
{
    public class Sponsor
    {
        public Guid id { get; set; }
        [Required]
        public string name { get; set; }
    }
}
