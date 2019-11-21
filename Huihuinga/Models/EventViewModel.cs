using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Huihuinga.Models
{
    public class EventViewModel
    {
        public Event[] Events { get; set; }

        public SelectList TopicsList { get; set; }

        public string SearchString { get; set; }
        public string EventTopic { get; set; }
        public string EventType { get; set; }

        public Dictionary<String, String> TypeTranslation { get; set; }
    }
}
