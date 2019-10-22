using Huihuinga.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Huihuinga.Services
{
    public interface IConcreteConferenceService
    {
        Task<ConcreteConference[]> GetConcreteConferencesAsync();

        Task<bool> Create(ConcreteConference newConcreteConference);

        Task<ConcreteConference> Details(Guid id);

        Task<bool> AddUser(ApplicationUser user, Guid conferenceId);
    }
}
