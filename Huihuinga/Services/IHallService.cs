using Huihuinga.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Huihuinga.Services
{
    public interface IHallService
    {
        Task<Hall[]> GetHallsAsync();

        Task<bool> Create(Hall newhall);

        Task<Hall> Details(Guid id);
    }
}
