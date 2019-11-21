using Huihuinga.Data;
using Huihuinga.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Huihuinga.Services
{
    public class EventService : IEventService
    {
        private readonly ApplicationDbContext _context;

        public EventService(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task<Event[]> GetAllEvents()
        {
            var events = new List<Event> { };

            var chats = await _context.Chats.Include(e => e.EventTopics).ThenInclude(et => et.Topic).ToArrayAsync();
            events.AddRange(chats);

            var practicalsessions = await _context.PracticalSessions.Include(e => e.EventTopics).ThenInclude(et => et.Topic).ToArrayAsync();
            events.AddRange(practicalsessions);

            var talks = await _context.Talks.Include(e => e.EventTopics).ThenInclude(et => et.Topic).ToArrayAsync();
            events.AddRange(talks);

            var parties = await _context.Parties.ToArrayAsync();
            events.AddRange(parties);

            var meals = await _context.Meals.ToArrayAsync();
            events.AddRange(meals);

            return events.ToArray();
        }
    }
}
