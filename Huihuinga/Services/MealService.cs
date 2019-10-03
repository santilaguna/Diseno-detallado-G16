using Huihuinga.Data;
using Huihuinga.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Huihuinga.Services
{
    public class MealService:IMealService
    {
        private readonly ApplicationDbContext _context;
        public MealService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Meal[]> GetMealsAsync()
        {
            var meals = await _context.Meals.ToArrayAsync();
            return meals;
        }

        public async Task<bool> Create(Meal newMeal)
        {
            newMeal.id = Guid.NewGuid();
            _context.Meals.Add(newMeal);
            var saveResult = await _context.SaveChangesAsync(); return saveResult == 1;

        }

        public async Task<Meal> Details(Guid id)
        {
            var meals = await _context.Meals.Where(x => x.id == id).ToArrayAsync();
            return meals[0];
        }

        public async Task<Hall[]> GetHalls()
        {
            var halls = await _context.Halls.ToArrayAsync();
            return halls;
        }

    }
}
