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
        public ICollection<Event> Events { get; set; }
        public ICollection<Sponsor> Sponsors { get; set; }
        public ICollection<ApplicationUserConcreteConference> UsersConferences { get; set; }
        public int Maxassistants { get; set; }
    }
}
