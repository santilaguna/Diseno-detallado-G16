using Huihuinga.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Huihuinga.Services
{
    public interface IChatService
    {
        Task<Chat[]> GetChatsAsync();

        Task<bool> Create(Chat newchat);

        Task<Chat> Details(Guid id);

        Task<Hall[]> GetHalls();

    }
}
