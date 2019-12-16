using Huihuinga.Data;
using Huihuinga.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Huihuinga.Services
{
    public class EventService : IEventService
    {
        private readonly ApplicationDbContext _context;

        public EventService(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task<Event[]> GetAllEvents()
        {
            var events = new List<Event> { };

            var chats = await _context.Chats.Include(e => e.EventTopics).ThenInclude(et => et.Topic).ToArrayAsync();
            events.AddRange(chats);

            var practicalsessions = await _context.PracticalSessions.Include(e => e.EventTopics).ThenInclude(et => et.Topic).ToArrayAsync();
            events.AddRange(practicalsessions);

            var talks = await _context.Talks.Include(e => e.EventTopics).ThenInclude(et => et.Topic).ToArrayAsync();
            events.AddRange(talks);

            var parties = await _context.Parties.ToArrayAsync();
            events.AddRange(parties);

            var meals = await _context.Meals.ToArrayAsync();
            events.AddRange(meals);

            return events.ToArray();
        }

        public async Task<Event[]> GetAllEventsHome()
        {
            var events = new List<Event> { };

            var chats = await _context.Chats.ToArrayAsync();
            events.AddRange(chats);

            var practicalsessions = await _context.PracticalSessions.ToArrayAsync();
            events.AddRange(practicalsessions);

            var talks = await _context.Talks.ToArrayAsync();
            events.AddRange(talks);

            var parties = await _context.Parties.ToArrayAsync();
            events.AddRange(parties);

            var meals = await _context.Meals.ToArrayAsync();
            events.AddRange(meals);

            return events.ToArray();
        }

        public async Task<Event[]> GetAllEventsProfile(string id)
        {
            var userEvents = await _context.UserEvents.Where(x => x.UserId == id).ToArrayAsync();
            var ids_list = from e in userEvents select e.EventId;
            var events_ids =  new HashSet<Guid>(ids_list);

            var events = new List<Event> { };

            var chats = await _context.Chats.Where(e => events_ids.Contains(e.id)).ToArrayAsync();
            events.AddRange(chats);

            var practicalsessions = await _context.PracticalSessions.Where(e => events_ids.Contains(e.id)).ToArrayAsync();
            events.AddRange(practicalsessions);

            var talks = await _context.Talks.Where(e => events_ids.Contains(e.id)).ToArrayAsync();
            events.AddRange(talks);

            var parties = await _context.Parties.Where(e => events_ids.Contains(e.id)).ToArrayAsync();
            events.AddRange(parties);

            var meals = await _context.Meals.Where(e => events_ids.Contains(e.id)).ToArrayAsync();
            events.AddRange(meals);

            return events.ToArray();
        }

        public async Task<bool> CheckSubscribedUser(string userId, Guid eventId)
        {
            var userEvent = await _context.UserEvents.Where(x => x.UserId == userId && x.EventId == 
                                                                           eventId).ToArrayAsync();
            return !(userEvent == null || userEvent.Length == 0);
        }

        public async Task<int> GetMaxAssistants(Guid hallId)
        {
            var hall = await _context.Halls.FirstOrDefaultAsync(x => x.id == hallId);
            return hall.capacity;
        }
        
        public async Task<bool> CheckLimitUsers(Event Event)
        {
            var actualUsers = await GetActualUsers(Event);
            var maxAssistants = await GetMaxAssistants(Event.Hallid);
            return (actualUsers < maxAssistants);
        }

        public async Task<int> GetActualUsers(Event Event)
        {
            var userEvents = await _context.UserEvents.Where(x => x.EventId == Event.id).ToArrayAsync();
            return userEvents.Length;
        }

        public async Task<bool> AddUser(ApplicationUser user, Guid eventId)
        {
            var Event = await _context.Events.Where(x => x.id == eventId).ToArrayAsync();
            var newUserEvent = new ApplicationUserEvent();
            if (Event == null || Event.Length == 0 || user == null)
            {
                return false;
            }
            newUserEvent.UserId = user.Id;
            newUserEvent.User = user;
            newUserEvent.EventId = eventId;
            newUserEvent.Event = Event[0];

            var alreadyJoin = await _context.UserEvents.Where(x => x.UserId == user.Id && 
                                                                   x.EventId == eventId).ToArrayAsync();
            var saveResult = 1;
            if (alreadyJoin.Length != 0)
            {
                return saveResult == 0;
            }
            _context.UserEvents.Add(newUserEvent);
            saveResult = await _context.SaveChangesAsync();
            return saveResult == 1;
        }

        public async Task<bool> DeleteUser(ApplicationUser user, Guid eventId)
        {
            var userEvent =
                await _context.UserEvents.FirstOrDefaultAsync(x => x.UserId == user.Id &&
                                                                   x.EventId == eventId);
            _context.UserEvents.Attach(userEvent);
            _context.UserEvents.Remove(userEvent);
            var saveResult = await _context.SaveChangesAsync();
            return saveResult == 1;
        }

        public async Task<ApplicationUser[]> GetAllUsers()
        {
            var users = await _context.ApplicationUsers.ToArrayAsync();
            return users;
        }

        public async Task<string> GetUserName(string userid)
        {
            var username = await _context.ApplicationUsers.FirstOrDefaultAsync(x => x.Id == userid);
            return username.FullName;
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

        public async Task<ExpositorQualityEventList> GetExpositorEvents(string UserId)
        {
            var talks = await _context.Talks.Where(e => e.ExpositorId == UserId ).ToArrayAsync();
            var sessions = await _context.PracticalSessions.Where(e => e.ExpositorId == UserId).ToArrayAsync();

            var talks_id = new List<Guid> { };
            var session_id = new List<Guid> { };


            foreach (var talk in talks)
            {
                talks_id.Add(talk.id);
            }

            foreach (var session in sessions)
            {
                session_id.Add(session.id);
            }

            var feedbacks = await _context.Feedbacks.Where(e => talks_id.Contains(e.EventId) ||
                             session_id.Contains(e.EventId)).ToArrayAsync();

            var conferenceFeedbacks = await _context.ConferenceFeedbacks.Where(e => talks_id.Contains(e.EventId) ||
                             session_id.Contains(e.EventId)).ToArrayAsync();

            var events_quality = new ExpositorQualityEventList
            {
                list = new List<ExpositorQualityEvent> { }
            };
            var j = 0;
            foreach (var feedback in feedbacks.OrderByDescending(e => e.dateTime)) 
            {
                if (j >= 7)
                {
                    break;
                }
                var talk = await _context.Talks.FirstOrDefaultAsync(e => e.id == feedback.EventId);
                var session = await _context.PracticalSessions.FirstOrDefaultAsync(e => e.id == feedback.EventId);

                if (talk != null)
                {
                    var expositorEvent = new ExpositorQualityEvent { EventName = talk.name, Quality = feedback.ExpositorQuality };
                    events_quality.list.Add(expositorEvent);
                    j += 1;
                }
                else if (session != null)
                {
                    var expositorEvent = new ExpositorQualityEvent { EventName = session.name, Quality = feedback.ExpositorQuality };
                    events_quality.list.Add(expositorEvent);
                    j += 1;
                }

            }

            foreach (var feedback in conferenceFeedbacks.OrderByDescending(e => e.dateTime))
            {
                if (j >= 7)
                {
                    break;
                }
                var talk = await _context.Talks.FirstOrDefaultAsync(e => e.id == feedback.EventId);
                var session = await _context.PracticalSessions.FirstOrDefaultAsync(e => e.id == feedback.EventId);

                if (talk != null)
                {
                    var expositorEvent = new ExpositorQualityEvent { EventName = talk.name, Quality = feedback.ExpositorQuality };
                    events_quality.list.Add(expositorEvent);
                    j += 1;
                }
                else if (session != null)
                {
                    var expositorEvent = new ExpositorQualityEvent { EventName = session.name, Quality = feedback.ExpositorQuality };
                    events_quality.list.Add(expositorEvent);
                    j += 1;
                }

            }
            return events_quality;



        }
    }
}
