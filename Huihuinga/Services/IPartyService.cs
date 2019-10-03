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

        Task<Hall[]> GetHalls();
    }
}
