using Huihuinga.Data;
using Huihuinga.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Huihuinga.Services
{
    public class TopicService : ITopicService
    {
        private readonly ApplicationDbContext _context;
        public TopicService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<bool> Create(Topic newTopic)
        {
            newTopic.id = Guid.NewGuid();
            _context.Topics.Add(newTopic);
            var saveResult = await _context.SaveChangesAsync(); return saveResult == 1;

        }

        public async Task<Topic> Details(Guid id)
        {
            var topics = await _context.Topics.Where(x => x.id == id).ToArrayAsync();
            return topics[0];
        }

        public async Task<bool> Edit(Guid id, string name, string description)
        {
            var topictoupdate = await _context.Topics.FirstOrDefaultAsync(s => s.id == id);
            topictoupdate.name = name;
            topictoupdate.description = description;
            _context.Update(topictoupdate);
            var saveResult = await _context.SaveChangesAsync(); return saveResult == 1;
        }

        public async Task<bool> Delete(Guid id)
        {
            // TODO: eliminar de los eventos donde pueda permanecer
            var topictodelete = await _context.Topics.FirstOrDefaultAsync(s => s.id == id);
            _context.Topics.Attach(topictodelete);
            _context.Topics.Remove(topictodelete);
            var saveResult = await _context.SaveChangesAsync();
            return saveResult == 1;
        }

        public async Task<Topic[]> GetTopicsAsync()
        {
            var topics = await _context.Topics.ToArrayAsync();
            return topics;
        }

        public async Task<bool> VerifyNewTopic(string topicName)
        {
            var topics = await _context.Topics.Where(t => t.name == topicName).ToArrayAsync();
            if (topics.Any()) return false;
            return true;
        }
    }
}
