using Huihuinga.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Huihuinga.Models
{
    public class PracticalSession : Event, ITopical
    {
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

            _context.PracticalSessions.Attach(this);
            _context.PracticalSessions.Remove(this);
            await _context.SaveChangesAsync();
        }

        public List<Material> Material { get; set; }
        public string ExpositorId { get; set; }
    }
}
