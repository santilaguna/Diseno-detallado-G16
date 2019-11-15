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
    public class TalkService : ITalkService
    {
        private readonly ApplicationDbContext _context;
        public TalkService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Talk[]> GetTalksAsync()
        {
            var talks = await _context.Talks.Where(e => e.concreteConferenceId == null).ToArrayAsync();
            return talks;
        }

        public async Task<bool> Create(Talk newTalk)
        {
            newTalk.id = Guid.NewGuid();
            _context.Talks.Add(newTalk);
            var saveResult = await _context.SaveChangesAsync(); return saveResult == 1;

        }

        public async Task<Talk> Details(Guid id)
        {
            var talks = await _context.Talks.Where(x => x.id == id).Include(e => e.Topics).ToArrayAsync();
            return talks[0];
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
            var talktoupdate = await _context.Talks.Include(e => e.Topics).FirstOrDefaultAsync(s => s.id == id);
            talktoupdate.name = name;
            talktoupdate.starttime = starttime;
            talktoupdate.endtime = endtime;
            talktoupdate.Hallid = Hallid;
            talktoupdate.description = description;
            _context.Update(talktoupdate);
            var saveResult = await _context.SaveChangesAsync(); return saveResult == 1;
        }

        public async Task<bool> Delete(Guid id)
        {
            var talktodelete = await _context.Talks.Include(e => e.Topics).FirstOrDefaultAsync(s => s.id == id);
            talktodelete.Topics.Clear();
            if (talktodelete.concreteConferenceId != null) { 
                var conference = await _context.ConcreteConferences.Where(x => x.id == talktodelete.concreteConferenceId).FirstAsync();
                conference.Events.Remove(talktodelete);
            }
            _context.Talks.Attach(talktodelete);
            _context.Talks.Remove(talktodelete);
            var saveResult = await _context.SaveChangesAsync();
            return saveResult == 1;
        }

        public async Task<Topic[]> NewTopic(Guid id)
        {
            var topics = await _context.Topics.ToArrayAsync();
            var talks = await _context.Talks.Where(x => x.id == id).Include(e => e.Topics).ToArrayAsync();
            var talk = talks[0];
            topics = topics.Where(topic => !talk.Topics.Contains(topic)).ToArray();
            return topics;
        }
        [ValidateAntiForgeryToken]
        public async Task<bool> AddNewTopic(Guid id, Topic newTopic)
        {
            var talks = await _context.Talks.Where(x => x.id == id).Include(e => e.Topics).ToArrayAsync();
            var talk = talks[0];
            var topics = await _context.Topics.Where(x => x.name == newTopic.name).ToArrayAsync();
            if (topics.Any())
            {
                return false;
            }
            newTopic.id = Guid.NewGuid();
            _context.Topics.Add(newTopic);
            var saveResult = await _context.SaveChangesAsync();
            talk.Topics.Add(newTopic);
            var saveResult2 = await _context.SaveChangesAsync();
            return saveResult == 1 && saveResult2 == 1;
        }

        public async Task<bool> AddTopic(Guid id, Guid topicId)
        {
            var talks = await _context.Talks.Where(x => x.id == id).Include(e => e.Topics).ToArrayAsync();
            var talk = talks[0];
            var topics = await _context.Topics.Where(x => x.id == topicId).ToArrayAsync();
            var topic = topics[0];
            talk.Topics.Add(topic);
            var saveResult = await _context.SaveChangesAsync();
            return saveResult == 1;
        }

        public async Task<bool> CheckUser(Guid id, string UserId)
        {
            var talk = await _context.Talks.FirstOrDefaultAsync(x => x.id == id);
            return (talk.UserId == UserId);
        }
    }
}
