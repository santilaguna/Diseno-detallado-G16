using Huihuinga.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Huihuinga.Services
{
    public interface IChatService : ITopicalService
    {
        Task<Chat[]> GetChatsAsync();

        Task<bool> Create(Chat newchat);

        Task<Chat> Details(Guid id);

        Task<Hall[]> GetHalls(Guid? id);

        Task<bool> Edit(Guid id, string name, DateTime starttime, DateTime endtime, Guid Hallid);

        Task<bool> Delete(Guid id);

        Task<bool> CheckUser(Guid id, string UserId);

        Task<bool> AddExpositor(string expositorid, Guid eventid);
        Task<bool> DeleteExpositor(string expositormail, Guid eventid);
        Task<List<string>> GetExpositors(List<string> expositorsid);
    }
}
