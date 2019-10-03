using Huihuinga.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Huihuinga.Services
{
    public interface ITalkService
    {
        Task<Talk[]> GetTalksAsync();

        Task<bool> Create(Talk newchat);

        Task<Talk> Details(Guid id);

        Task<Hall[]> GetHalls();
    }
}
