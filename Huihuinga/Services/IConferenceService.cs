using Huihuinga.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Huihuinga.Services
{
    public interface IConferenceService
    {
        Task<Conference[]> GetConferencesAsync();

        Task<bool> Create(Conference newConference);

        Task<Conference> Details(Guid id);

        Task<bool> Edit(Guid id, string name, string description, CalendarRepetition calendarRepetition);

        Task<bool> Delete(Guid id);

        Task<bool> CheckUser(Guid id, string UserId);

        Task<double> FoodQuality(Guid eventId);
        Task<List<string>> Comments(Guid eventId);
        Task<double> ExpositorQuality(Guid eventId);
        Task<double> PlaceQuality(Guid eventId);
        Task<double> MusicQuality(Guid eventId);
        Task<double> MaterialQuality(Guid eventId);
        Task<double> DiscussionQuality(Guid eventId);
    }
}
