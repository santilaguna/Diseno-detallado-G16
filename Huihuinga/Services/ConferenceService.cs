using Huihuinga.Data;
using Huihuinga.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Huihuinga.Services
{
    public class ConferenceService : IConferenceService
    {
        private readonly ApplicationDbContext _context;
        public ConferenceService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Conference[]> GetConferencesAsync()
        {
            var conferences = await _context.Conferences.ToArrayAsync();
            return conferences;
        }

        public async Task<bool> Create(Conference newConference)
        {
            newConference.id = Guid.NewGuid();
            _context.Conferences.Add(newConference);
            var saveResult = await _context.SaveChangesAsync(); return saveResult == 1;

        }

        public async Task<Conference> Details(Guid id)
        {
            var conferences = await _context.Conferences.Where(x => x.id == id).Include(c => c.Instance).ToArrayAsync();
            return conferences[0];
        }

        public async Task<bool> Edit(Guid id, string name, string description, CalendarRepetition calendarRepetition)
        {
            var conferencetoupdate = await _context.Conferences.FirstOrDefaultAsync(s => s.id == id);
            conferencetoupdate.name = name;
            conferencetoupdate.description = description;
            conferencetoupdate.calendarRepetition = calendarRepetition;
            _context.Update(conferencetoupdate);
            var saveResult = await _context.SaveChangesAsync(); return saveResult == 1;
        }

        public async Task<bool> Delete(Guid id)
        {
            var conferencetodelete = await _context.Conferences.Include(c => c.Instance).ThenInclude(i => i.Events)
                .FirstOrDefaultAsync(s => s.id == id);

            if (conferencetodelete.Instance != null)
            {
                // eliminamos relación con padre
                var instance = conferencetodelete.Instance;
                conferencetodelete.Instance = null;
                _context.Update(conferencetodelete);
                await _context.SaveChangesAsync();

                // eliminamos versión
                foreach (var event_ in instance.Events.ToList())
                {
                    instance.Events.Remove(event_);
                    await event_.DeleteSelf(_context);
                }
                _context.ConcreteConferences.Attach(instance);
                _context.ConcreteConferences.Remove(instance);
                await _context.SaveChangesAsync();
            }

            _context.Conferences.Attach(conferencetodelete);
            _context.Conferences.Remove(conferencetodelete);
            var saveResult = await _context.SaveChangesAsync();
            return saveResult == 1;
        }

        public async Task<bool> CheckUser(Guid id, string UserId)
        {
            var concreteconference = await _context.Conferences.FirstOrDefaultAsync(x => x.id == id);
            return (concreteconference.UserId == UserId);
        }

        public async Task<double> DiscussionQuality(Guid ConferenceId)
        {
            var feedbacks = await _context.ConferenceFeedbacks.Where(e => e.ConferenceId == ConferenceId).ToArrayAsync();
            int Quality = 0;
            foreach (ConferenceFeedback feedback in feedbacks)
            {
                if (feedback.DiscussionQuality != 0)
                {
                    Quality += feedback.DiscussionQuality;
                }
            }

            var BadFeedbacks = feedbacks.Where(e => e.DiscussionQuality != 0).ToArray();

            if (feedbacks.Length - BadFeedbacks.Length == 0)
            {
                return 0;
            }

            return Quality / (feedbacks.Length - BadFeedbacks.Length);
        }

        public async Task<double> FoodQuality(Guid ConferenceId)
        {
            var feedbacks = await _context.ConferenceFeedbacks.Where(e => e.ConferenceId == ConferenceId).ToArrayAsync();
            int Quality = 0;
            foreach (ConferenceFeedback feedback in feedbacks)
            {
                if (feedback.FoodQuality != 0)
                {
                    Quality += feedback.FoodQuality;
                }
            }

            var BadFeedbacks = feedbacks.Where(e => e.FoodQuality != 0).ToArray();

            if (feedbacks.Length - BadFeedbacks.Length == 0)
            {
                return 0;
            }

            return Quality / (feedbacks.Length - BadFeedbacks.Length);
        }

        public async Task<double> PlaceQuality(Guid ConferenceId)
        {
            var feedbacks = await _context.ConferenceFeedbacks.Where(e => e.ConferenceId == ConferenceId).ToArrayAsync();
            int Quality = 0;
            foreach (ConferenceFeedback feedback in feedbacks)
            {
                if (feedback.PlaceQuality != 0)
                {
                    Quality += feedback.PlaceQuality;
                }
            }

            var BadFeedbacks = feedbacks.Where(e => e.PlaceQuality != 0).ToArray();

            if (feedbacks.Length - BadFeedbacks.Length == 0)
            {
                return 0;
            }

            return Quality / (feedbacks.Length - BadFeedbacks.Length);
        }

        public async Task<double> ExpositorQuality(Guid ConferenceId)
        {
            var feedbacks = await _context.ConferenceFeedbacks.Where(e => e.ConferenceId == ConferenceId).ToArrayAsync();
            int Quality = 0;
            foreach (ConferenceFeedback feedback in feedbacks)
            {
                if (feedback.ExpositorQuality != 0)
                {
                    Quality += feedback.ExpositorQuality;
                }

            }

            var BadFeedbacks = feedbacks.Where(e => e.ExpositorQuality != 0).ToArray();

            if (feedbacks.Length - BadFeedbacks.Length == 0)
            {
                return 0;
            }

            return Quality / (feedbacks.Length - BadFeedbacks.Length);
        }

        public async Task<double> MaterialQuality(Guid ConferenceId)
        {
            var feedbacks = await _context.ConferenceFeedbacks.Where(e => e.ConferenceId == ConferenceId).ToArrayAsync();
            int Quality = 0;
            foreach (ConferenceFeedback feedback in feedbacks)
            {
                if (feedback.MaterialQuality != 0)
                {
                    Quality += feedback.MaterialQuality;
                }
            }

            var BadFeedbacks = feedbacks.Where(e => e.MaterialQuality != 0).ToArray();

            if (feedbacks.Length - BadFeedbacks.Length == 0)
            {
                return 0;
            }

            return Quality / (feedbacks.Length - BadFeedbacks.Length);
        }

        public async Task<double> MusicQuality(Guid ConferenceId)
        {
            var feedbacks = await _context.ConferenceFeedbacks.Where(e => e.ConferenceId == ConferenceId).ToArrayAsync();
            int Quality = 0;
            foreach (ConferenceFeedback feedback in feedbacks)
            {
                if (feedback.MusicQuality != 0)
                {
                    Quality += feedback.MusicQuality;
                }
            }

            var BadFeedbacks = feedbacks.Where(e => e.MusicQuality != 0).ToArray();

            if (feedbacks.Length - BadFeedbacks.Length == 0)
            {
                return 0;
            }

            return Quality / (feedbacks.Length - BadFeedbacks.Length);
        }

        public async Task<List<string>> Comments(Guid ConferenceId)
        {
            var feedbacks = await _context.ConferenceFeedbacks.Where(e => e.ConferenceId == ConferenceId).ToArrayAsync();
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

    }
}
