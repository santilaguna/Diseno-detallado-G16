using Huihuinga.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Huihuinga.Models
{
    public class Chat : Event, ITopical
    {
        public HashSet<Topic> Topics { get; set; }
    }
}
