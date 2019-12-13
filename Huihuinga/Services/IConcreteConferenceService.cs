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

        Task<bool> Edit(Guid id, string name, DateTime starttime, DateTime endtime, int maxAssistants);

        Task<bool> Delete(Guid id);

        Task<Conference> FindConference(Guid abstractConferenceId);

        Task<bool> AddUser(ApplicationUser user, Guid conferenceId);
        
        Task<bool> CheckUser(string userId, Guid conferenceId);

        Task<bool> CheckLimitUsers(ConcreteConference conference);
        Task<Event[]> ShowEvents(Guid id);
        Task<EventCenter[]> GetEventCenters();

        Task<bool> CheckOwner(Guid id, string UserId);

        Task<Guid> ObtainConference(Guid ConcreteConferenceId);

        Task<bool> CreateConferenceFeedback(ConferenceFeedback feedback);
    }
}
