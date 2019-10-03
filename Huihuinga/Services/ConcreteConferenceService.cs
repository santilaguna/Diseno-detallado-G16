using Huihuinga.Data;
using Huihuinga.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Huihuinga.Services
{
    public class ConcreteConferenceService : IConcreteConferenceService
    {
        private readonly ApplicationDbContext _context;
        public ConcreteConferenceService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<ConcreteConference[]> GetConcreteConferencesAsync()
        {
            var concreteConferences = await _context.ConcreteConferences.ToArrayAsync();
            return concreteConferences;
        }

        public async Task<bool> Create(ConcreteConference newConcreteConference)
        {
            newConcreteConference.id = Guid.NewGuid();
            _context.ConcreteConferences.Add(newConcreteConference);
            var saveResult = await _context.SaveChangesAsync(); return saveResult == 1;

        }

        public async Task<ConcreteConference> Details(Guid id)
        {
            var halls = await _context.ConcreteConferences.Where(x => x.id == id).ToArrayAsync();
            return halls[0];
        }

    }
}
