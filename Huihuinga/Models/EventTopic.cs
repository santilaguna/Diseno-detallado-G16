using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Huihuinga.Models
{
    public class EventTopic
    {
        public Guid EventId { get; set; }
        public Event Event { get; set; }
        public Guid TopicId { get; set; }
        public Topic Topic { get; set; }
    }
}
