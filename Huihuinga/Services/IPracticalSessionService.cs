using Huihuinga.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Huihuinga.Services
{
    public interface IPracticalSessionService
    {
        Task<PracticalSession[]> GetSessionsAsync();

        Task<bool> Create(PracticalSession newsession);

        Task<PracticalSession> Details(Guid id);

        Task<Hall[]> GetHalls();
    }
}
