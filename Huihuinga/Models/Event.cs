using Huihuinga.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Huihuinga.Models
{
    public class Event
    {
        public Guid id { get; set; }
        public Guid? concreteConferenceId { get; set; }
        [Required]
        public string name { get; set; }
        [Required]
        public DateTime starttime{ get; set; }
        [Required]
        public DateTime endtime { get; set; }
        public Guid Hallid { get; set; }
        public string PhotoPath { get; set; }
        public string UserId { get; set; }

        public virtual Task DeleteSelf(ApplicationDbContext _context)
        {
            throw new NotImplementedException();
        }
        [JsonIgnore]
        [IgnoreDataMember]
        public ICollection<ApplicationUserEvent> UsersEvents { get; set; }
    }
}
