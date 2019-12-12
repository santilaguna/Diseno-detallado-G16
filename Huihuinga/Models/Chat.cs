using Huihuinga.Controllers;
using Huihuinga.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Huihuinga.Models
{
    public class Chat : Event, ITopical
    {
        [JsonIgnore]
        [IgnoreDataMember]
        public HashSet<EventTopic> EventTopics { get; set; }

        public override async Task DeleteSelf(ApplicationDbContext _context)
        {
            _context.Entry(this).Collection(e => e.EventTopics).Load();
            EventTopics.Clear();
            await _context.SaveChangesAsync();

            var ets_to_delete = _context.EventTopics.Where(et => et.EventId == id);
            _context.EventTopics.AttachRange(ets_to_delete);
            _context.EventTopics.RemoveRange(ets_to_delete);
            await _context.SaveChangesAsync();

            _context.Chats.Attach(this);
            _context.Chats.Remove(this);
            await _context.SaveChangesAsync();
        }

        public string ModeratorId { get; set; }
        public List<string> ExpositorsId { get; set; }
    }
}
