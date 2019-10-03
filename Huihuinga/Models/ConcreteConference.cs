using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Huihuinga.Models
{
    public class ConcreteConference
    {
        public Guid id { get; set; }
        [Required]
        public string name { get; set; }
        [Required]
        public DateTime starttime { get; set; }
        [Required]
        public DateTime endtime { get; set; }
        public List<Event> Events { get; set; }
        public List<Sponsor> Sponsors { get; set; }
    }
}
