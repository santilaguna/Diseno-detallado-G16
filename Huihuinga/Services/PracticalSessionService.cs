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
    }
}
