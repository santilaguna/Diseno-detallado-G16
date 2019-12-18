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
            var saveResult = await _context.SaveChangesAsync();

            // lo agregamos como instancia del padre
            var conference = await FindConference(newConcreteConference.abstractConferenceId);
            conference.Instance = newConcreteConference;
            _context.Update(conference);
            var saveResult2 = await _context.SaveChangesAsync();
            return saveResult == 1 && saveResult2 == 1;

        }

        public async Task<ConcreteConference> Details(Guid id)
        {
            var halls = await _context.ConcreteConferences.Where(x => x.id == id).Include(e => e.Events).ToArrayAsync();
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

        public async Task<bool> Edit(Guid id, string name, DateTime starttime, DateTime endtime, int maxAssistants)
        {
            var conferencetoupdate = await _context.ConcreteConferences.FirstOrDefaultAsync(s => s.id == id);
            conferencetoupdate.name = name;
            conferencetoupdate.starttime = starttime;
            conferencetoupdate.endtime = endtime;
            conferencetoupdate.Maxassistants = maxAssistants;
            _context.Update(conferencetoupdate);
            var saveResult = await _context.SaveChangesAsync(); return saveResult == 1;
        }

        public async Task<bool> Delete(Guid id)
        {
            var conferencetodelete = await _context.ConcreteConferences.Include(c => c.Events).FirstOrDefaultAsync(s => s.id == id);
            foreach(var event_ in conferencetodelete.Events.ToList())
            {
                conferencetodelete.Events.Remove(event_);
                await event_.DeleteSelf(_context);
            }

            // eliminamos relación con padre
            var conference = await FindConference(conferencetodelete.abstractConferenceId);
            conference.Instance = null;
            _context.Update(conference);
            var saveResult2 = await _context.SaveChangesAsync();
            

            // eliminamos versión
            _context.ConcreteConferences.Attach(conferencetodelete);
            _context.ConcreteConferences.Remove(conferencetodelete);
            var saveResult = await _context.SaveChangesAsync();
            return saveResult == 1 && saveResult2 == 1;
        }

        public async Task<Conference> FindConference(Guid abstractConferenceId)
        {
            var conferences = await _context.Conferences.Where(x => x.id == abstractConferenceId).ToArrayAsync();
            return conferences[0];
        }

        public async Task<Event[]> ShowEvents(Guid id)
        {
            var events = new List<Event> { };

            var chats = await _context.Chats.Where(e => e.concreteConferenceId == id)
                .Include(e => e.EventTopics).ThenInclude(et => et.Topic).ToArrayAsync();
            events.AddRange(chats);

            var practicalsessions = await _context.PracticalSessions.Where(e => e.concreteConferenceId == id)
                .Include(e => e.EventTopics).ThenInclude(et => et.Topic).ToArrayAsync();
            events.AddRange(practicalsessions);

            var talks = await _context.Talks.Where(e => e.concreteConferenceId == id)
                .Include(e => e.EventTopics).ThenInclude(et => et.Topic).ToArrayAsync();
            events.AddRange(talks);

            var parties = await _context.Parties.Where(e => e.concreteConferenceId == id).ToArrayAsync();
            events.AddRange(parties);

            var meals = await _context.Meals.Where(e => e.concreteConferenceId == id).ToArrayAsync();
            events.AddRange(meals);

            return events.ToArray();
        }

        public async Task<EventCenter[]> GetEventCenters()
        {
            var centers = await _context.EventCenters.ToArrayAsync();
            return centers;
        }

        public async Task<bool> CheckOwner(Guid id, string UserId)
        {
            var concreteconference = await _context.ConcreteConferences.FirstOrDefaultAsync(x => x.id == id);
            return (concreteconference.UserId == UserId);
        }

        public async Task<Guid> ObtainConference(Guid ConcreteConferenceId)
        {
            var concreteconference = await _context.ConcreteConferences.FirstOrDefaultAsync(e => e.id == ConcreteConferenceId);
            return concreteconference.abstractConferenceId;
        }

        public async Task<bool> CreateConferenceFeedback(ConferenceFeedback feedback)
        {
            feedback.id = Guid.NewGuid();
            _context.ConferenceFeedbacks.Add(feedback);
            var saveResult = await _context.SaveChangesAsync();
            return saveResult == 1;
        }

        public async Task<ApplicationUserConcreteConference[]> GetUsersAsync(Guid id)
        {
            var userConferences = await _context.UserConferences.Where(u => u.ConferenceId == id).ToArrayAsync();
            foreach (var user in userConferences)
            {
                var findUsers = await _context.Users.Where(u => u.Id == user.UserId).ToArrayAsync();
                user.User = findUsers[0];
                var findConferences = await _context.ConcreteConferences.Where(c => c.id == user.ConferenceId).ToArrayAsync();
                user.Conference = findConferences[0];
            }
            return userConferences;
        }
        
        public async Task<List<string>> Comments(Guid concreteConferenceId)
        {
            var feedbacks = await _context.ConferenceFeedbacks.Where(e => e.ConcreteConferenceId == concreteConferenceId).ToArrayAsync();
            var comments = new List<string> { };
            foreach (ConferenceFeedback feedback in feedbacks)
            {
                if (feedback.comment != null)
                {
                    comments.Add(feedback.comment);
                }
            }
            return comments;
        }

        public async Task<double> MusicQuality(Guid concreteConferenceId)
        {
            var feedbacks = await _context.ConferenceFeedbacks.Where(e => e.ConcreteConferenceId == concreteConferenceId).ToArrayAsync();
            int Quality = 0;
            foreach (ConferenceFeedback feedback in feedbacks)
            {
                if (feedback.MusicQuality != 0)
                {
                    Quality += feedback.MusicQuality;
                }
            }

            var BadFeedbacks = feedbacks.Where(e => e.MusicQuality == 0).ToArray();

            if (feedbacks.Length - BadFeedbacks.Length == 0)
            {
                return 0;
            }

            return Quality / (feedbacks.Length - BadFeedbacks.Length);
        }

        public async Task<double> DiscussionQuality(Guid concreteConferenceId)
        {
            var feedbacks = await _context.ConferenceFeedbacks.Where(e => e.ConcreteConferenceId == concreteConferenceId).ToArrayAsync();
            int Quality = 0;
            foreach (ConferenceFeedback feedback in feedbacks)
            {
                if (feedback.DiscussionQuality != 0)
                {
                    Quality += feedback.DiscussionQuality;
                }
            }

            var BadFeedbacks = feedbacks.Where(e => e.DiscussionQuality == 0).ToArray();

            if (feedbacks.Length - BadFeedbacks.Length == 0)
            {
                return 0;
            }

            return Quality / (feedbacks.Length - BadFeedbacks.Length);
        }

        public async Task<double> FoodQuality(Guid concreteConferenceId)
        {
            var feedbacks = await _context.ConferenceFeedbacks.Where(e => e.ConcreteConferenceId == concreteConferenceId).ToArrayAsync();
            int Quality = 0;
            foreach (ConferenceFeedback feedback in feedbacks)
            {
                if (feedback.FoodQuality != 0)
                {
                    Quality += feedback.FoodQuality;
                }
            }

            var BadFeedbacks = feedbacks.Where(e => e.FoodQuality == 0).ToArray();

            if (feedbacks.Length - BadFeedbacks.Length == 0)
            {
                return 0;
            }

            return Quality / (feedbacks.Length - BadFeedbacks.Length);
        }

        public async Task<double> PlaceQuality(Guid concreteConferenceId)
        {
            var feedbacks = await _context.ConferenceFeedbacks.Where(e => e.ConcreteConferenceId == concreteConferenceId).ToArrayAsync();
            int Quality = 0;
            foreach (ConferenceFeedback feedback in feedbacks)
            {
                if (feedback.PlaceQuality != 0)
                {
                    Quality += feedback.PlaceQuality;
                }
            }

            var BadFeedbacks = feedbacks.Where(e => e.PlaceQuality == 0).ToArray();

            if (feedbacks.Length - BadFeedbacks.Length == 0)
            {
                return 0;
            }

            return Quality / (feedbacks.Length - BadFeedbacks.Length);
        }

        public async Task<double> ExpositorQuality(Guid concreteConferenceId)
        {
            var feedbacks = await _context.ConferenceFeedbacks.Where(e => e.ConcreteConferenceId == concreteConferenceId).ToArrayAsync();
            int Quality = 0;
            foreach (ConferenceFeedback feedback in feedbacks)
            {
                if (feedback.ExpositorQuality != 0)
                {
                    Quality += feedback.ExpositorQuality;
                }

            }

            var BadFeedbacks = feedbacks.Where(e => e.ExpositorQuality == 0).ToArray();

            if (feedbacks.Length - BadFeedbacks.Length == 0)
            {
                return 0;
            }

            return Quality / (feedbacks.Length - BadFeedbacks.Length);
        }

        public async Task<double> MaterialQuality(Guid concreteConferenceId)
        {
            var feedbacks = await _context.ConferenceFeedbacks.Where(e => e.ConcreteConferenceId == concreteConferenceId).ToArrayAsync();
            int Quality = 0;
            foreach (ConferenceFeedback feedback in feedbacks)
            {
                if (feedback.MaterialQuality != 0)
                {
                    Quality += feedback.MaterialQuality;
                }
            }

            var BadFeedbacks = feedbacks.Where(e => e.MaterialQuality == 0).ToArray();

            if (feedbacks.Length - BadFeedbacks.Length == 0)
            {
                return 0;
            }

            return Quality / (feedbacks.Length - BadFeedbacks.Length);
        }

        public async Task<bool> VerifyNewConcreteConference(string concreteConferenceName, Guid ConferenceId)
        { 
            var instances = await _context.ConcreteConferences.Where(t => t.name == concreteConferenceName &&
                                                                     t.abstractConferenceId == ConferenceId).ToArrayAsync();
            if (instances.Any()) return false;
            return true;
        }

        
    }
}
