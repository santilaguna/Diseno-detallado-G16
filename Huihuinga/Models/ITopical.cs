using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Huihuinga.Models
{
    public interface ITopical
    {
        [JsonIgnore]
        [IgnoreDataMember]
        HashSet<EventTopic> EventTopics { get; set; }
    }
}
