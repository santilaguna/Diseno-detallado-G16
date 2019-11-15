using Huihuinga.Controllers;
using Huihuinga.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Huihuinga.Models
{
    public class Chat : Event, ITopical
    {
        public HashSet<Topic> Topics { get; set; }

        public override async Task DeleteSelf(ApplicationDbContext _context)
        {
            _context.Entry(this).Collection(e => e.Topics).Load();
            Topics.Clear();
            _context.Chats.Attach(this);
            _context.Chats.Remove(this);
            await _context.SaveChangesAsync();
        }
    }
}
