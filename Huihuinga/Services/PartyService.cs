using Huihuinga.Data;
using Huihuinga.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Huihuinga.Services
{
    public class PartyService:IPartyService
    {
        private readonly ApplicationDbContext _context;
        public PartyService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Party[]> GetPartiesAsync()
        {
            var parties = await _context.Parties.ToArrayAsync();
            return parties;
        }

        public async Task<bool> Create(Party newparty)
        {
            newparty.id = Guid.NewGuid();
            _context.Parties.Add(newparty);
            var saveResult = await _context.SaveChangesAsync(); return saveResult == 1;

        }

        public async Task<Party> Details(Guid id)
        {
            var parties = await _context.Parties.Where(x => x.id == id).ToArrayAsync();
            return parties[0];
        }

        public async Task<Hall[]> GetHalls()
        {
            var halls = await _context.Halls.ToArrayAsync();
            return halls;
        }
    }
}
