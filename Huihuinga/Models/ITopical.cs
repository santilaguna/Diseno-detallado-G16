using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Huihuinga.Models
{
    interface ITopical
    {
        HashSet<Topic> Topics { get; set; }
    }
}
