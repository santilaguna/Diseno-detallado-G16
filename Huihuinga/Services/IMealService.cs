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

        Task<Hall[]> GetHalls();

        Task<bool> Edit(Guid id, string name, DateTime starttime, DateTime endtime, Guid Hallid);

        Task<bool> Delete(Guid id);

        Task<bool> CheckUser(Guid id, string UserId);

        Task<Meal[]> GetAllMealsAsync();
    }
}
