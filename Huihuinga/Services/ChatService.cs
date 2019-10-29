using Huihuinga.Data;
using Huihuinga.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Huihuinga.Services
{
    public class ChatService:IChatService
    {
        private readonly ApplicationDbContext _context;
        public ChatService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Chat[]> GetChatsAsync()
        {
            var chats = await _context.Chats.ToArrayAsync();
            return chats;
        }

        public async Task<bool> Create(Chat newchat)
        {
            newchat.id = Guid.NewGuid();
            _context.Chats.Add(newchat);
            var saveResult = await _context.SaveChangesAsync(); return saveResult == 1;

        }

        public async Task<Chat> Details(Guid id)
        {
            var chats = await _context.Chats.Where(x => x.id == id).ToArrayAsync();
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
            var chattodelete = await _context.Chats.FirstOrDefaultAsync(s => s.id == id);
            _context.Chats.Attach(chattodelete);
            _context.Chats.Remove(chattodelete);
            var saveResult = await _context.SaveChangesAsync();
            return saveResult == 1;
        }
    }
}
