using Huihuinga.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Huihuinga.Services
{
    public interface IPartyService
    {
        Task<Party[]> GetPartiesAsync();

        Task<bool> Create(Party newparty);

        Task<Party> Details(Guid id);

        Task<Hall[]> GetHalls(Guid? id);

        Task<bool> Edit(Guid id, string name, DateTime starttime, DateTime endtime, Guid Hallid, string description);

        Task<bool> Delete(Guid id);

        Task<bool> CheckUser(Guid id, string UserId);
    }
}
