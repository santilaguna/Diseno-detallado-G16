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
    public class ChatService : IChatService
    {
        private readonly ApplicationDbContext _context;
        public ChatService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Chat[]> GetChatsAsync()
        {
            var chats = await _context.Chats.Where(e => e.concreteConferenceId == null && e.endtime > DateTime.Now).ToArrayAsync();
            return chats;
        }
        [ValidateAntiForgeryToken]
        public async Task<bool> Create(Chat newchat)
        {
            newchat.id = Guid.NewGuid();
            _context.Chats.Add(newchat);
            var saveResult = await _context.SaveChangesAsync(); return saveResult == 1;

        }

        public async Task<Chat> Details(Guid id)
        {
            var chats = await _context.Chats.Where(x => x.id == id).Include(e => e.EventTopics)
                .ThenInclude(et => et.Topic).ToArrayAsync();
            return chats[0];
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
            var chattoupdate = await _context.Chats.FirstOrDefaultAsync(s => s.id == id);
            chattoupdate.name = name;
            chattoupdate.starttime = starttime;
            chattoupdate.endtime = endtime;
            chattoupdate.Hallid = Hallid;
            _context.Update(chattoupdate);
            var saveResult = await _context.SaveChangesAsync(); return saveResult == 1;
        }

        public async Task<bool> Delete(Guid id)
        {
            var chattodelete = await _context.Chats.FirstAsync(s => s.id == id);
            if (chattodelete.concreteConferenceId != null)
            {
                var conference = await _context.ConcreteConferences.Where(x => x.id == chattodelete.concreteConferenceId).FirstAsync();
                conference.Events.Remove(chattodelete);
            }
            chattodelete.EventTopics.Clear();
            await _context.SaveChangesAsync();

            var ets_to_delete = _context.EventTopics.Where(et => et.EventId == id);
            _context.EventTopics.AttachRange(ets_to_delete);
            _context.EventTopics.RemoveRange(ets_to_delete);
            await _context.SaveChangesAsync();

            _context.Chats.Attach(chattodelete);
            _context.Chats.Remove(chattodelete);
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
            var chats = await _context.Chats.Where(x => x.id == id).Include(c => c.EventTopics).ToArrayAsync();
            var chat = chats[0];
            var topics = await _context.Topics.Where(x => x.name == newTopic.name).ToArrayAsync();
            if (topics.Any()) {
                return false;
            }
            newTopic.id = Guid.NewGuid();
            _context.Topics.Add(newTopic);
            var saveResult = await _context.SaveChangesAsync();

            var newEventTopic = new EventTopic
            {
                Event = chat,
                Topic = newTopic,
                EventId = id,
                TopicId = newTopic.id
            };
            _context.EventTopics.Add(newEventTopic);
            chat.EventTopics.Add(newEventTopic);

            var saveResult2 = await _context.SaveChangesAsync();
            return saveResult == 1 && saveResult2 == 1;
        }

        public async Task<bool> AddTopic(Guid id, Guid topicId)
        {
            var chats = await _context.Chats.Where(x => x.id == id).Include(c => c.EventTopics).ToArrayAsync();
            var chat = chats[0];
            var topics = await _context.Topics.Where(x => x.id == topicId).ToArrayAsync();
            var topic = topics[0];

            var newEventTopic = new EventTopic
            {
                Event = chat,
                Topic = topic,
                EventId = id,
                TopicId = topicId
            };
            _context.EventTopics.Add(newEventTopic);
            chat.EventTopics.Add(newEventTopic);

            var saveResult = await _context.SaveChangesAsync();
            return saveResult == 1;
        }

        public async Task<bool> CheckUser(Guid id, string UserId)
        {
            var chat = await _context.Chats.FirstOrDefaultAsync(x => x.id == id);
            return (chat.UserId == UserId);
        }

        public async Task<bool> AddExpositor(string expositorid, Guid eventid)
        {
            var chat = await _context.Chats.FirstOrDefaultAsync(x => x.id == eventid);
            if (!chat.ExpositorsId.Contains(expositorid))
            {
                chat.ExpositorsId.Add(expositorid);
            }
            var saveResult = await _context.SaveChangesAsync();
            return saveResult == 1;

        }

        public async Task<bool> DeleteExpositor(string expositormail, Guid eventid)
        {
            var user = await _context.ApplicationUsers.FirstOrDefaultAsync(x => x.Email == expositormail);
            var chat = await _context.Chats.FirstOrDefaultAsync(x => x.id == eventid);
            chat.ExpositorsId.Remove(user.Id);
            var saveResult = await _context.SaveChangesAsync();
            return saveResult == 1;

        }

        public async Task<List<string>> GetExpositors(List<string> expositorsid)
        {
            var expositors = await _context.ApplicationUsers.Where(x => expositorsid.Contains(x.Id)).ToArrayAsync();
            var expositorsname = new List<string> { };
            foreach (ApplicationUser expositor in expositors)
            {
                expositorsname.Add(expositor.FullName);
            }
            return expositorsname;
        }

        public async Task<Chat[]> GetChatsWithPendingFeedbacks(string UserId)
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
            var chats = await _context.Chats.Where(e => EventsId.Contains(e.id) && !EventsWithFeedbackId.Contains(e.id)
                            && e.concreteConferenceId == null && e.endtime < DateTime.Now).ToArrayAsync();

            return chats;
        }

        public async Task<bool> CreateFeedback(Feedback feedback, Guid event_id)
        {
            var chat = await _context.Chats.FirstOrDefaultAsync(e => e.id == event_id);
            feedback.id = Guid.NewGuid();
            _context.Feedbacks.Add(feedback);
            chat.feedbacks.Add(feedback);
            var saveResult = await _context.SaveChangesAsync();
            return saveResult == 1;
        }

        public async Task<Chat[]> GetFinishedChats()
        {
            var chats = await _context.Chats.Where(e => e.concreteConferenceId == null && e.endtime < DateTime.Now).ToArrayAsync();
            return chats;
        }

        public async Task<double> PlaceQuality(Guid eventId)
        {
            var feedbacks = await _context.Feedbacks.Where(e => e.EventId == eventId).ToArrayAsync();
            int Quality = 0;
            foreach (Feedback feedback in feedbacks)
            {
                Quality += feedback.PlaceQuality;
            }

            return Quality / feedbacks.Length;
        }

        public async Task<double> DiscussionQuality(Guid eventId)
        {
            var feedbacks = await _context.Feedbacks.Where(e => e.EventId == eventId).ToArrayAsync();
            int Quality = 0;
            foreach (Feedback feedback in feedbacks)
            {
                Quality += feedback.DiscussionQuality;
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
    }
}
