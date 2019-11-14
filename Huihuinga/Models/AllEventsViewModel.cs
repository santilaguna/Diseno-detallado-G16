using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Huihuinga.Models
{
    public class AllEventsViewModel
    {
        public IEnumerable<Chat> Chats { get; set; }
        public IEnumerable<Meal> Meals { get; set; }
        public IEnumerable<Party> Parties { get; set; }
        public IEnumerable<PracticalSession> PracticalSessions { get; set; }
        public IEnumerable<Talk> Talks { get; set; }
        public bool Show_chats { get; set; }
        public bool Show_meals { get; set; }
        public bool Show_parties { get; set; }
        public bool Show_practical { get; set; }
        public bool Show_talks { get; set; }
    }
}
