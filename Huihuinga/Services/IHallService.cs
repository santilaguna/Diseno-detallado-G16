using Huihuinga.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Huihuinga.Services
{
    public interface IHallService
    {
        Task<Hall[]> GetHallsAsync(Guid centerid);

        Task<bool> Create(Hall newhall);

        Task<Hall> Details(Guid id);

        Task<EventCenter> FindCenter(Guid centerid);
    }
}
