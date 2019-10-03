using Huihuinga.Data;
using Huihuinga.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Huihuinga.Services
{
    public class TalkService: ITalkService
    {
        private readonly ApplicationDbContext _context;
        public TalkService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Talk[]> GetTalksAsync()
        {
            var talks = await _context.Talks.ToArrayAsync();
            return talks;
        }

        public async Task<bool> Create(Talk newTalk)
        {
            newTalk.id = Guid.NewGuid();
            _context.Talks.Add(newTalk);
            var saveResult = await _context.SaveChangesAsync(); return saveResult == 1;

        }

        public async Task<Talk> Details(Guid id)
        {
            var talks = await _context.Talks.Where(x => x.id == id).ToArrayAsync();
            return talks[0];
        }

        public async Task<Hall[]> GetHalls()
        {
            var halls = await _context.Halls.ToArrayAsync();
            return halls;
        }
    }
}
