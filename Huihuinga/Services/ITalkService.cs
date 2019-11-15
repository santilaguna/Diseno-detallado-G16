using Huihuinga.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Huihuinga.Services
{
    public interface ITalkService : ITopicalService
    {
        Task<Talk[]> GetTalksAsync();

        Task<bool> Create(Talk newchat);

        Task<Talk> Details(Guid id);

        Task<Hall[]> GetHalls();

        Task<bool> Edit(Guid id, string name, DateTime starttime, DateTime endtime, Guid Hallid, string description);

        Task<bool> Delete(Guid id);

        Task<bool> CheckUser(Guid id, string UserId);

        Task<Talk[]> GetAllTalksAsync();
    }
}
