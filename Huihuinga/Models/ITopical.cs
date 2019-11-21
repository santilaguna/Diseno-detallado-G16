using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Huihuinga.Models
{
    public interface ITopical
    {
        HashSet<EventTopic> EventTopics { get; set; }
    }
}
