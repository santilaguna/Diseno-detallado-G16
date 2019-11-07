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

        public async Task<bool> AddUser(ApplicationUser user, Guid conferenceId)
        {
            var conference = await _context.ConcreteConferences.Where(x => x.id == conferenceId).ToArrayAsync();
            var newUserConference = new ApplicationUserConcreteConference();
            if (conference == null || conference.Length == 0 || user == null)
            {
                return false;
            }
            newUserConference.UserId = user.Id;
            newUserConference.User = user;
            newUserConference.ConferenceId = conferenceId;
            newUserConference.Conference = conference[0];
            _context.UserConferences.Add(newUserConference);
            var saveResult = await _context.SaveChangesAsync();
            return saveResult == 1;
        }

        public async Task<bool> CheckUser(string userId, Guid conferenceId)
        {
            var userConference = await _context.UserConferences.Where(x => x.UserId == userId && x.ConferenceId == 
                                                                           conferenceId).ToArrayAsync();
            return !(userConference == null || userConference.Length == 0);
        }

        public async Task<bool> CheckLimitUsers(ConcreteConference conference)
        {
            var userConferences =
                await _context.UserConferences.Where(x => x.ConferenceId == conference.id).ToArrayAsync();
            return (userConferences.Length < conference.Maxassistants);
        }

    }
}
