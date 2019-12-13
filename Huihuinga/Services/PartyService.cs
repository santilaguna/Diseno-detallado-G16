using Huihuinga.Data;
using Huihuinga.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Huihuinga.Services
{
    public class PartyService : IPartyService
    {
        private readonly ApplicationDbContext _context;
        public PartyService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Party[]> GetPartiesAsync()
        {
            var parties = await _context.Parties.Where(e => e.concreteConferenceId == null && e.endtime > DateTime.Now).ToArrayAsync();
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

        public async Task<Hall[]> GetHalls(Guid? conferenceId)
        {
            if (conferenceId == null)
            {
                var halls = await _context.Halls.ToArrayAsync();
                return halls;
            }
            else
            {
                var conference = await _context.ConcreteConferences.FirstAsync(x => x.id == conferenceId);
                var halls = await _context.Halls.Where(x => x.EventCenterid == conference.centerId).ToArrayAsync();
                return halls;
            }
        }

        public async Task<bool> Edit(Guid id, string name, DateTime starttime, DateTime endtime, Guid Hallid, string description)
        {
            var partytoupdate = await _context.Parties.FirstOrDefaultAsync(s => s.id == id);
            partytoupdate.name = name;
            partytoupdate.starttime = starttime;
            partytoupdate.endtime = endtime;
            partytoupdate.Hallid = Hallid;
            partytoupdate.description = description;
            _context.Update(partytoupdate);
            var saveResult = await _context.SaveChangesAsync(); return saveResult == 1;
        }

        public async Task<bool> Delete(Guid id)
        {
            var partytodelete = await _context.Parties.FirstOrDefaultAsync(s => s.id == id);
            if (partytodelete.concreteConferenceId != null)
            {
                var conference = await _context.ConcreteConferences.Where(x => x.id == partytodelete.concreteConferenceId).FirstAsync();
                conference.Events.Remove(partytodelete);
            }
            _context.Parties.Attach(partytodelete);
            _context.Parties.Remove(partytodelete);
            var saveResult = await _context.SaveChangesAsync();
            return saveResult == 1;
        }

        public async Task<bool> CheckUser(Guid id, string UserId)
        {
            var party = await _context.Parties.FirstOrDefaultAsync(x => x.id == id);
            return (party.UserId == UserId);
        }

        public async Task<Party[]> GetPartiesWithPendingFeedbacks(string UserId)
        {
            var UsersEvent = await _context.UserEvents.Where(e => e.UserId == UserId).ToArrayAsync();
            var EventsId = new List<Guid> { };
            foreach (ApplicationUserEvent userevent in UsersEvent)
            {
                EventsId.Add(userevent.EventId);
            }
            var feedbacks = await _context.Feedbacks.Where(e => e.UserId == UserId).ToArrayAsync();
            var EventsWithFeedbackId = new List<Guid> { };
            foreach (Feedback feedback in feedbacks)
            {
                EventsWithFeedbackId.Add(feedback.EventId);
            }
            var parties = await _context.Parties.Where(e => EventsId.Contains(e.id) && !EventsWithFeedbackId.Contains(e.id)
                            && e.concreteConferenceId == null && e.endtime < DateTime.Now).ToArrayAsync();

            return parties;
        }

        public async Task<bool> CreateFeedback(Feedback feedback, Guid event_id)
        {
            var party = await _context.Parties.FirstOrDefaultAsync(e => e.id == event_id);
            feedback.id = Guid.NewGuid();
            _context.Feedbacks.Add(feedback);
            party.feedbacks.Add(feedback);
            var saveResult = await _context.SaveChangesAsync();
            return saveResult == 1;
        }

        public async Task<Party[]> GetFinishedParties()
        {
            var parties = await _context.Parties.Where(e => e.concreteConferenceId == null && e.endtime < DateTime.Now).ToArrayAsync();
            return parties;
        }

        public async Task<double> MusicQuality(Guid eventId)
        {
            var feedbacks = await _context.Feedbacks.Where(e => e.EventId == eventId).ToArrayAsync();
            int Quality = 0;
            foreach (Feedback feedback in feedbacks)
            {
                Quality += feedback.MusicQuality;
            }

            if (feedbacks.Length == 0)
            {
                return 0;
            }

            return Quality / feedbacks.Length;
        }

        public async Task<double> PlaceQuality(Guid eventId)
        {
            var feedbacks = await _context.Feedbacks.Where(e => e.EventId == eventId).ToArrayAsync();
            int Quality = 0;
            foreach (Feedback feedback in feedbacks)
            {
                Quality += feedback.PlaceQuality;
            }

            if (feedbacks.Length == 0)
            {
                return 0;
            }

            return Quality / feedbacks.Length;
        }

        public async Task<List<string>> Comments(Guid eventId)
        {
            var feedbacks = await _context.Feedbacks.Where(e => e.EventId == eventId).ToArrayAsync();
            var comments = new List<string> { };
            foreach (Feedback feedback in feedbacks)
            {
                comments.Add(feedback.comment);
            }
            return comments;
        }

        public async Task<bool> CanFeedback(string UserId, Guid EventId)
        {
            var UsersEvent = await _context.UserEvents.Where(e => e.UserId == UserId).ToArrayAsync();
            var EventsId = new List<Guid> { };
            foreach (ApplicationUserEvent userevent in UsersEvent)
            {
                EventsId.Add(userevent.EventId);
            }
            var feedbacks = await _context.ConferenceFeedbacks.Where(e => e.UserId == UserId).ToArrayAsync();
            var EventsWithFeedbackId = new List<Guid> { };
            foreach (ConferenceFeedback feedback in feedbacks)
            {
                EventsWithFeedbackId.Add(feedback.EventId);
            }
            var parties = await _context.Parties.Where(e => EventsId.Contains(e.id) && !EventsWithFeedbackId.Contains(e.id)
                            && e.concreteConferenceId != null && e.endtime < DateTime.Now).ToArrayAsync();

            var EventsCanFeedback = new List<Guid> { };
            foreach (var party in parties)
            {
                EventsCanFeedback.Add(party.id);
            }

            return EventsCanFeedback.Contains(EventId);
        }
    }
}
