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

        public async Task<bool> Edit(Guid id, string name, DateTime starttime, DateTime endtime, Guid Hallid, string description)
        {
            var talktoupdate = await _context.Talks.FirstOrDefaultAsync(s => s.id == id);
            talktoupdate.name = name;
            talktoupdate.starttime = starttime;
            talktoupdate.endtime = endtime;
            talktoupdate.Hallid = Hallid;
            talktoupdate.description = description;
            _context.Update(talktoupdate);
            var saveResult = await _context.SaveChangesAsync(); return saveResult == 1;
        }

        public async Task<bool> Delete(Guid id)
        {
            var talktodelete = await _context.Talks.FirstOrDefaultAsync(s => s.id == id);
            _context.Talks.Attach(talktodelete);
            _context.Talks.Remove(talktodelete);
            var saveResult = await _context.SaveChangesAsync();
            return saveResult == 1;
        }
    }
}
