﻿using Huihuinga.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Huihuinga.Models
{
    public class PracticalSession : Event, ITopical
    {
        public HashSet<Topic> Topics { get; set; }

        public override async Task DeleteSelf(ApplicationDbContext _context)
        {
            _context.Entry(this).Collection(e => e.Topics).Load();
            Topics.Clear();
            _context.PracticalSessions.Attach(this);
            _context.PracticalSessions.Remove(this);
            await _context.SaveChangesAsync();
        }
    }
}
