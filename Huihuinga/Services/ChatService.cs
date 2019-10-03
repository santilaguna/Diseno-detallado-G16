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
    }
}
