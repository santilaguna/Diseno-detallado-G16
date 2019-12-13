using Huihuinga.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Huihuinga.Services
{
    public interface IMealService
    {
        Task<Meal[]> GetMealsAsync();

        Task<bool> Create(Meal newmeal);

        Task<Meal> Details(Guid id);

        Task<Hall[]> GetHalls(Guid? id);

        Task<bool> Edit(Guid id, string name, DateTime starttime, DateTime endtime, Guid Hallid);

        Task<bool> Delete(Guid id);

        Task<bool> CheckUser(Guid id, string UserId);

        Task<bool> CreateMenu(Menu menu);

        Task<Menu[]> GetMenu(Guid id);

        Task<bool> DeleteMenu(Guid MenuId);

        Task<Menu> ShowMenu(Guid MenuId);

        Task<Meal[]> GetMealsWithPendingFeedbacks(string UserId);

        Task<bool> CreateFeedback(Feedback feedback, Guid event_id);

        Task<Meal[]> GetFinishedMeals();

        Task<double> FoodQuality(Guid eventId);
        Task<List<string>> Comments(Guid eventId);

        Task<bool> CanFeedback(string UserId, Guid EventId);
    }
}
