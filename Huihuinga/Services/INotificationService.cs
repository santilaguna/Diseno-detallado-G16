using Huihuinga.Models;
using System;
using System.Threading.Tasks;

namespace Huihuinga.Services
{
    public interface INotificationService
    {
        Task<bool> SendConferenceNotification(ApplicationUserConcreteConference[] userConferences, string mailBodyMessage);
        Task<bool> SendEventNotification(ApplicationUserEvent[] userEvents, string mailBodyMessage);
    }
}
