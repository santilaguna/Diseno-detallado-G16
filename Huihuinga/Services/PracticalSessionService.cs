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
    public class PracticalSessionService: IPracticalSessionService
    {
        private readonly ApplicationDbContext _context;
        public PracticalSessionService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<PracticalSession[]> GetSessionsAsync()
        {
            var sessions = await _context.PracticalSessions.Where(e => e.concreteConferenceId == null && e.endtime > DateTime.Now).ToArrayAsync();
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
            var sessions = await _context.PracticalSessions.Where(x => x.id == id).Include(e => e.EventTopics)
                .ThenInclude(et => et.Topic).ToArrayAsync();
            return sessions[0];
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
            if (sessiontodelete.concreteConferenceId != null)
            {
                var conference = await _context.ConcreteConferences.Where(x => x.id == sessiontodelete.concreteConferenceId).FirstAsync();
                conference.Events.Remove(sessiontodelete);
            }
            sessiontodelete.EventTopics.Clear();
            await _context.SaveChangesAsync();

            var ets_to_delete = _context.EventTopics.Where(et => et.EventId == id);
            _context.EventTopics.AttachRange(ets_to_delete);
            _context.EventTopics.RemoveRange(ets_to_delete);
            await _context.SaveChangesAsync();

            _context.PracticalSessions.Attach(sessiontodelete);
            _context.PracticalSessions.Remove(sessiontodelete);
            var saveResult = await _context.SaveChangesAsync();
            return saveResult == 1;
        }

        public async Task<Topic[]> NewTopic(Guid id)
        {
            var eventTopics = await _context.EventTopics.Where(et => et.EventId == id).Include(et => et.Topic).ToArrayAsync();     
            var current_topics = (from et in eventTopics select et.Topic).ToHashSet();
            var topics = await _context.Topics.Where(t => !current_topics.Contains(t)).ToArrayAsync();
            return topics;
        }
        [ValidateAntiForgeryToken]
        public async Task<bool> AddNewTopic(Guid id, Topic newTopic)
        {
            var practicalsessions = await _context.PracticalSessions.Where(x => x.id == id).Include(c => c.EventTopics).ToArrayAsync();
            var practicalsession = practicalsessions[0];
            var topics = await _context.Topics.Where(x => x.name == newTopic.name).ToArrayAsync();
            if (topics.Any())
            {
                return false;
            }
            newTopic.id = Guid.NewGuid();
            _context.Topics.Add(newTopic);
            var saveResult = await _context.SaveChangesAsync();

            var newEventTopic = new EventTopic
            {
                Event = practicalsession,
                Topic = newTopic,
                EventId = id,
                TopicId = newTopic.id
            };
            _context.EventTopics.Add(newEventTopic);
            practicalsession.EventTopics.Add(newEventTopic);

            var saveResult2 = await _context.SaveChangesAsync();
            return saveResult == 1 && saveResult2 == 1;
        }

        public async Task<bool> AddTopic(Guid id, Guid topicId)
        {
            var practicalsessions = await _context.PracticalSessions.Where(x => x.id == id).Include(c => c.EventTopics).ToArrayAsync();
            var practicalsession = practicalsessions[0];
            var topics = await _context.Topics.Where(x => x.id == topicId).ToArrayAsync();
            var topic = topics[0];

            var newEventTopic = new EventTopic
            {
                Event = practicalsession,
                Topic = topic,
                EventId = id,
                TopicId = topicId
            };
            _context.EventTopics.Add(newEventTopic);
            practicalsession.EventTopics.Add(newEventTopic);

            var saveResult = await _context.SaveChangesAsync();
            return saveResult == 1;
        }

        public async Task<bool> CheckUser(Guid id, string UserId)
        {
            var session = await _context.PracticalSessions.FirstOrDefaultAsync(x => x.id == id);
            return (session.UserId == UserId);
        }

        public async Task<bool> CreateMaterial(Material material)
        {
            material.Id = Guid.NewGuid();
            _context.Materials.Add(material);
            var saveResult = await _context.SaveChangesAsync();
            return saveResult == 1;
        }

        public async Task<Material[]> GetMaterial(Guid id)
        {
            var Materials = await _context.Materials.Where(x => x.EventId == id).ToArrayAsync();
            return Materials;
        }

        public async Task<bool> DeleteMaterial(Guid MaterialId)
        {
            var materialtodelete = await _context.Materials.FirstOrDefaultAsync(s => s.Id == MaterialId);
            _context.Materials.Attach(materialtodelete);
            _context.Materials.Remove(materialtodelete);
            var saveResult = await _context.SaveChangesAsync();
            return saveResult == 1;
        }

        public async Task<PracticalSession[]> GetSessionsWithPendingFeedbacks(string UserId)
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
            var sessions = await _context.PracticalSessions.Where(e => EventsId.Contains(e.id) && !EventsWithFeedbackId.Contains(e.id)
                            && e.concreteConferenceId == null && e.endtime < DateTime.Now).ToArrayAsync();

            return sessions;
        }

        public async Task<bool> CreateFeedback(Feedback feedback, Guid event_id)
        {
            var session = await _context.PracticalSessions.FirstOrDefaultAsync(e => e.id == event_id);
            feedback.id = Guid.NewGuid();
            _context.Feedbacks.Add(feedback);
            session.feedbacks.Add(feedback);
            var saveResult = await _context.SaveChangesAsync();
            return saveResult == 1;
        }

        public async Task<PracticalSession[]> GetFinishedSessions()
        {
            var sessions = await _context.PracticalSessions.Where(e => e.concreteConferenceId == null && e.endtime < DateTime.Now).ToArrayAsync();
            return sessions;
        }

        public async Task<double> MaterialQuality(Guid eventId)
        {
            var feedbacks = await _context.Feedbacks.Where(e => e.EventId == eventId).ToArrayAsync();
            int Quality = 0;
            foreach (Feedback feedback in feedbacks)
            {
                Quality += feedback.MaterialQuality;
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

        public async Task<double> ExpositorQuality(Guid eventId)
        {
            var feedbacks = await _context.Feedbacks.Where(e => e.EventId == eventId).ToArrayAsync();
            int Quality = 0;
            foreach (Feedback feedback in feedbacks)
            {
                Quality += feedback.ExpositorQuality;
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
            var sessions = await _context.PracticalSessions.Where(e => EventsId.Contains(e.id) && !EventsWithFeedbackId.Contains(e.id)
                            && e.concreteConferenceId != null && e.endtime < DateTime.Now).ToArrayAsync();

            var EventsCanFeedback = new List<Guid> { };
            foreach (var session in sessions)
            {
                EventsCanFeedback.Add(session.id);
            }

            return EventsCanFeedback.Contains(EventId);
        }
    }
}
