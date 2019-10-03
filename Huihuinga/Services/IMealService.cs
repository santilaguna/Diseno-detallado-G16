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

    }
}
