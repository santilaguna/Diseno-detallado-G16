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
            var chats = await _context.Chats.Where(e => e.concreteConferenceId == null).ToArrayAsync();
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
            var chats = await _context.Chats.Where(x => x.id == id).Include(e => e.Topics).ToArrayAsync();
            return chats[0];
        }

        public async Task<Hall[]> GetHalls()
        {
            var halls = await _context.Halls.ToArrayAsync();
            return halls;
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
            var chattodelete = await _context.Chats.Include(e => e.Topics).FirstAsync(s => s.id == id);
            if (chattodelete.concreteConferenceId != null)
            {
                var conference = await _context.ConcreteConferences.Where(x => x.id == chattodelete.concreteConferenceId).FirstAsync();
                conference.Events.Remove(chattodelete);
            }
            chattodelete.Topics.Clear();
            _context.Chats.Attach(chattodelete);
            _context.Chats.Remove(chattodelete);
            var saveResult = await _context.SaveChangesAsync();
            return saveResult == 1;
        }

        public async Task<Topic[]> NewTopic(Guid id)
        {
            var topics = await _context.Topics.ToArrayAsync();
            var chats = await _context.Chats.Where(x => x.id == id).Include(e => e.Topics).ToArrayAsync();
            var chat = chats[0];
            topics = topics.Where(topic => !chat.Topics.Contains(topic)).ToArray();
            return topics;
        }
        [ValidateAntiForgeryToken]
        public async Task<bool> AddNewTopic(Guid id, Topic newTopic)
        {
            var chats = await _context.Chats.Where(x => x.id == id).Include(e => e.Topics).ToArrayAsync();
            var chat = chats[0];
            var topics = await _context.Topics.Where(x => x.name == newTopic.name).ToArrayAsync();
            if (topics.Any()) { 
                return false;
            }
            newTopic.id = Guid.NewGuid();
            _context.Topics.Add(newTopic);
            var saveResult = await _context.SaveChangesAsync();
            chat.Topics.Add(newTopic);
            var saveResult2 = await _context.SaveChangesAsync();
            return saveResult == 1 && saveResult2 == 1;
        }

        public async Task<bool> AddTopic(Guid id, Guid topicId)
        {
            var chats = await _context.Chats.Where(x => x.id == id).Include(e => e.Topics).ToArrayAsync();
            var chat = chats[0];
            var topics = await _context.Topics.Where(x => x.id == topicId).ToArrayAsync();
            var topic = topics[0];
            chat.Topics.Add(topic);
            var saveResult = await _context.SaveChangesAsync();
            return saveResult == 1;
        }

        public async Task<bool> CheckUser(Guid id, string UserId)
        {
            var chat = await _context.Chats.FirstOrDefaultAsync(x => x.id == id);
            return (chat.UserId == UserId);
        }
    }
}
