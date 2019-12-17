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

        private async Task<ConcreteConference[]> GetVersions(Guid id)
        {
            var versions = await _context.ConcreteConferences.Where(c => c.abstractConferenceId == id).ToArrayAsync();
            return versions;
        }

        public async Task<List<Dictionary<string, object>>> GetChartRows(Guid ConferenceId)
        {
            var versions = await GetVersions(ConferenceId);
            var ret = new List<Dictionary<string, object>>();
            var all_feedbacks = await _context.ConferenceFeedbacks.Where(e => e.ConferenceId == ConferenceId).ToArrayAsync();

            foreach (ConcreteConference version in versions) {
                
                var feedbacks = all_feedbacks.Where(f => f.ConcreteConferenceId == version.id).ToArray();
                var version_dict = new Dictionary<string, object> ();
                version_dict.Add("fecha", version.endtime);

                double Quality = 0;
                foreach (ConferenceFeedback feedback in feedbacks)
                {
                        Quality += feedback.DiscussionQuality;
                }
                var BadFeedbacks = feedbacks.Where(e => e.DiscussionQuality == 0).ToArray().Length;
                double divisor = Math.Max(1, feedbacks.Length - BadFeedbacks);
                version_dict.Add("Discussion", Quality/divisor);

                Quality = 0;
                foreach (ConferenceFeedback feedback in feedbacks)
                {
                    Quality += feedback.ExpositorQuality;
                }
                BadFeedbacks = feedbacks.Where(e => e.ExpositorQuality == 0).ToArray().Length;
                divisor = Math.Max(1, feedbacks.Length - BadFeedbacks);
                version_dict.Add("Expositor", Quality / divisor);

                Quality = 0;
                foreach (ConferenceFeedback feedback in feedbacks)
                {
                    Quality += feedback.FoodQuality;
                }
                BadFeedbacks = feedbacks.Where(e => e.FoodQuality == 0).ToArray().Length;
                divisor = Math.Max(1, feedbacks.Length - BadFeedbacks);
                version_dict.Add("Food", Quality / divisor);

                Quality = 0;
                foreach (ConferenceFeedback feedback in feedbacks)
                {
                    Quality += feedback.MusicQuality;
                }
                BadFeedbacks = feedbacks.Where(e => e.MusicQuality == 0).ToArray().Length;
                divisor = Math.Max(1, feedbacks.Length - BadFeedbacks);
                version_dict.Add("Music", Quality / divisor);

                Quality = 0;
                foreach (ConferenceFeedback feedback in feedbacks)
                {
                    Quality += feedback.MaterialQuality;
                }
                BadFeedbacks = feedbacks.Where(e => e.MaterialQuality == 0).ToArray().Length;
                divisor = Math.Max(1, feedbacks.Length - BadFeedbacks);
                version_dict.Add("Material", Quality / divisor);

                Quality = 0;
                foreach (ConferenceFeedback feedback in feedbacks)
                {
                    Quality += feedback.PlaceQuality;
                }
                BadFeedbacks = feedbacks.Where(e => e.PlaceQuality == 0).ToArray().Length;
                divisor = Math.Max(1, feedbacks.Length - BadFeedbacks);
                version_dict.Add("Place", Quality / divisor);
                ret.Add(version_dict);
            }

            return ret;
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

        public async Task<ConferenceAttendanceList> GetConferenceAttendance(Guid ConferenceId)
        {
            var concreteConferences = await GetVersions(ConferenceId);
            int session_attendance = 0;
            int meal_attendance = 0;
            int chat_attendance = 0;
            int talk_attendance = 0;
            int party_attendance = 0;
            int session_quantity = 0;
            int meal_quantity = 0;
            int chat_quantity = 0;
            int talk_quantity = 0;
            int party_quantity = 0;
            var Ratings = new ConferenceAttendanceList { };


            foreach (var conference in concreteConferences)
            {
                var Sessions = await _context.PracticalSessions.Where(e => e.concreteConferenceId == conference.id).ToArrayAsync();
                session_quantity += Sessions.Length;
                foreach (var session in Sessions)
                {
                    var attendees = await _context.UserEvents.Where(e => e.EventId == session.id).ToArrayAsync();
                    session_attendance += attendees.Length;

                }

                var Meals = await _context.Meals.Where(e => e.concreteConferenceId == conference.id).ToArrayAsync();
                meal_quantity += Meals.Length;
                foreach (var meal in Meals)
                {         
                    var attendees = await _context.UserEvents.Where(e => e.EventId == meal.id).ToArrayAsync();
                    meal_attendance += attendees.Length;

                }

                var Chats = await _context.Chats.Where(e => e.concreteConferenceId == conference.id).ToArrayAsync();
                chat_quantity += Chats.Length;
                foreach (var chat in Chats)
                {
                    var attendees = await _context.UserEvents.Where(e => e.EventId == chat.id).ToArrayAsync();
                    chat_attendance += attendees.Length;

                }

                var Talks = await _context.Talks.Where(e => e.concreteConferenceId == conference.id).ToArrayAsync();
                talk_quantity += Talks.Length;
                foreach (var talk in Talks)
                {
                    var attendees = await _context.UserEvents.Where(e => e.EventId == talk.id).ToArrayAsync();
                    talk_attendance += attendees.Length;

                }

                var Parties = await _context.Parties.Where(e => e.concreteConferenceId == conference.id).ToArrayAsync();
                party_quantity += Parties.Length;
                foreach (var party in Parties)
                {
                    var attendees = await _context.UserEvents.Where(e => e.EventId == party.id).ToArrayAsync();
                    party_attendance += attendees.Length;

                }

            }
            double divisor = Math.Max(session_quantity, 1);

            var sessions_attendance = new ConferenceAttendance
            {
                Attendance = session_attendance / divisor,
                EventType = "Sesion"
            };

            Ratings.list.Add(sessions_attendance);
    


            divisor = Math.Max(meal_quantity, 1);

            var meals_attendance = new ConferenceAttendance
            {
                Attendance = meal_attendance / divisor,
                EventType = "Comida"
            };

            Ratings.list.Add(meals_attendance);

            
            

            divisor = Math.Max(chat_quantity, 1);

            var chats_attendance = new ConferenceAttendance
            {
                Attendance = chat_attendance / divisor,
                EventType = "Chat"
            };

            Ratings.list.Add(chats_attendance);

                


            divisor = Math.Max(talk_quantity, 1);

            var talks_attendance = new ConferenceAttendance
            {
                Attendance = talk_attendance / divisor,
                EventType = "Charla"
            };

            Ratings.list.Add(talks_attendance);




            divisor = Math.Max(party_attendance, 1);

            var parties_attendance = new ConferenceAttendance
            {
                Attendance = party_attendance / divisor,
                EventType = "Fiesta"
            };

            Ratings.list.Add(parties_attendance);

            return Ratings;
        }

    }
}
