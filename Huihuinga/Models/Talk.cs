using Huihuinga.Data;
using Huihuinga.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Huihuinga.Models
{
    public class Talk : Event, ITopical
    {
        public string description { get; set; }
        public HashSet<Topic> Topics { get; set; }

        public override async Task DeleteSelf(ApplicationDbContext _context)
        {
            _context.Entry(this).Collection(e => e.Topics).Load();
            Topics.Clear();
            _context.Talks.Attach(this);
            _context.Talks.Remove(this);
            await _context.SaveChangesAsync();
        }

        public List<Material> Material { get; set; }
    }

}
