using Huihuinga.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Huihuinga.Services
{
    public interface IEventService
    {
        Task<Event[]> GetAllEvents();
        Task<Event[]> GetAllEventsHome();
        Task<Event[]> GetAllEventsProfile(string id);
        Task<int> GetMaxAssistants(Guid hallId);
        Task<bool> CheckSubscribedUser(string userId, Guid eventId);
        Task<bool> CheckLimitUsers(Event Event);
        Task<bool> AddUser(ApplicationUser user, Guid eventId);
        Task<int> GetActualUsers(Event Event);
        Task<bool> DeleteUser(ApplicationUser user, Guid eventId);
        Task<ApplicationUser[]> GetAllUsers();
        Task<string> GetUserName(string userid);

        Task<Guid> ObtainConference(Guid ConcreteConferenceId);

        Task<bool> CreateConferenceFeedback(ConferenceFeedback feedback);

        Task<List<Event>> GetExpositorEvents(string UserId);
    }
}
