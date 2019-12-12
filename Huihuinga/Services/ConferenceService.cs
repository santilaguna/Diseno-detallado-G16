using Huihuinga.Data;
using Huihuinga.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Huihuinga.Services
{
    public class ConferenceService : IConferenceService
    {
        private readonly ApplicationDbContext _context;
        public ConferenceService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Conference[]> GetConferencesAsync()
        {
            var conferences = await _context.Conferences.ToArrayAsync();
            return conferences;
        }

        public async Task<bool> Create(Conference newConference)
        {
            newConference.id = Guid.NewGuid();
            _context.Conferences.Add(newConference);
            var saveResult = await _context.SaveChangesAsync(); return saveResult == 1;

        }

        public async Task<Conference> Details(Guid id)
        {
            var conferences = await _context.Conferences.Where(x => x.id == id).Include(c => c.Instance).ToArrayAsync();
            return conferences[0];
        }

        public async Task<bool> Edit(Guid id, string name, string description, CalendarRepetition calendarRepetition)
        {
            var conferencetoupdate = await _context.Conferences.FirstOrDefaultAsync(s => s.id == id);
            conferencetoupdate.name = name;
            conferencetoupdate.description = description;
            conferencetoupdate.calendarRepetition = calendarRepetition;
            _context.Update(conferencetoupdate);
            var saveResult = await _context.SaveChangesAsync(); return saveResult == 1;
        }

        public async Task<bool> Delete(Guid id)
        {
            var conferencetodelete = await _context.Conferences.Include(c => c.Instance).ThenInclude(i => i.Events)
                .FirstOrDefaultAsync(s => s.id == id);

            if (conferencetodelete.Instance != null)
            {
                // eliminamos relación con padre
                var instance = conferencetodelete.Instance;
                conferencetodelete.Instance = null;
                _context.Update(conferencetodelete);
                await _context.SaveChangesAsync();

                // eliminamos versión
                foreach (var event_ in instance.Events.ToList())
                {
                    instance.Events.Remove(event_);
                    await event_.DeleteSelf(_context);
                }
                _context.ConcreteConferences.Attach(instance);
                _context.ConcreteConferences.Remove(instance);
                await _context.SaveChangesAsync();
            }

            _context.Conferences.Attach(conferencetodelete);
            _context.Conferences.Remove(conferencetodelete);
            var saveResult = await _context.SaveChangesAsync();
            return saveResult == 1;
        }

        public async Task<bool> CheckUser(Guid id, string UserId)
        {
            var concreteconference = await _context.Conferences.FirstOrDefaultAsync(x => x.id == id);
            return (concreteconference.UserId == UserId);
        }

    }
}
