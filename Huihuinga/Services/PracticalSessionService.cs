using Huihuinga.Data;
using Huihuinga.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Huihuinga.Services
{
    public class PracticalSessionService: IPracticalSessionService
    {
        private readonly ApplicationDbContext _context;
        public PracticalSessionService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<PracticalSession[]> GetSessionsAsync()
        {
            var sessions = await _context.PracticalSessions.ToArrayAsync();
            return sessions;
        }

        public async Task<bool> Create(PracticalSession newsession)
        {
            newsession.id = Guid.NewGuid();
            _context.PracticalSessions.Add(newsession);
            var saveResult = await _context.SaveChangesAsync(); return saveResult == 1;

        }

        public async Task<PracticalSession> Details(Guid id)
        {
            var sessions = await _context.PracticalSessions.Where(x => x.id == id).ToArrayAsync();
            return sessions[0];
        }

        public async Task<Hall[]> GetHalls()
        {
            var halls = await _context.Halls.ToArrayAsync();
            return halls;
        }

        public async Task<bool> Edit(Guid id, string name, DateTime starttime, DateTime endtime, Guid Hallid)
        {
            var sessiontoupdate = await _context.PracticalSessions.FirstOrDefaultAsync(s => s.id == id);
            sessiontoupdate.name = name;
            sessiontoupdate.starttime = starttime;
            sessiontoupdate.endtime = endtime;
            sessiontoupdate.Hallid = Hallid;
            _context.Update(sessiontoupdate);
            var saveResult = await _context.SaveChangesAsync(); return saveResult == 1;
        }

        public async Task<bool> Delete(Guid id)
        {
            var sessiontodelete = await _context.PracticalSessions.FirstOrDefaultAsync(s => s.id == id);
            _context.PracticalSessions.Attach(sessiontodelete);
            _context.PracticalSessions.Remove(sessiontodelete);
            var saveResult = await _context.SaveChangesAsync();
            return saveResult == 1;
        }
    }
}
