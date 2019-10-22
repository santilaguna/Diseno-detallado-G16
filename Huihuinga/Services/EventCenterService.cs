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

        public async Task<bool> Edit(Guid id, string name, string address)
        {
            var eventcentertoupdate = await _context.EventCenters.FirstOrDefaultAsync(s => s.id == id);
            eventcentertoupdate.address = address;
            eventcentertoupdate.name = name;
            _context.Update(eventcentertoupdate);
            var saveResult = await _context.SaveChangesAsync(); return saveResult == 1;
        }

        public async Task<bool> Delete(Guid id)
        {
            var centertodelete = await _context.EventCenters.FirstOrDefaultAsync(s => s.id == id);
            _context.EventCenters.Attach(centertodelete);
            _context.EventCenters.Remove(centertodelete);
            var saveResult = await _context.SaveChangesAsync();
            return saveResult == 1;
        }

    }

}
