using Huihuinga.Data;
using Huihuinga.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Huihuinga.Services
{
    public class EventCenterService : IEventCenterService
    {
        private readonly ApplicationDbContext _context;
        public EventCenterService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<EventCenter[]> GetEventCentersAsync()
        {
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
            var eventcenters = await _context.EventCenters.ToArrayAsync();
            return eventcenters;
        }

        public async Task<bool> Create(EventCenter newEventCenter)
        {
            newEventCenter.id = Guid.NewGuid();
            _context.EventCenters.Add(newEventCenter);
            var saveResult = await _context.SaveChangesAsync(); return saveResult == 1;

        }

        public async Task<EventCenter> Details(Guid id)
        {
            var eventcenters = await _context.EventCenters.Where(x => x.id == id).ToArrayAsync();
            return eventcenters[0];
        }

    }

}
