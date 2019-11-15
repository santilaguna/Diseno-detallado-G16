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
            var meals = await _context.Meals.Where(e => e.concreteConferenceId == null).ToArrayAsync();
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

        public async Task<Hall[]> GetHalls(Guid? conferenceId)
        {
            if (conferenceId == null)
            {
                var halls = await _context.Halls.ToArrayAsync();
                return halls;
            }
            else
            {
                var conference = await _context.ConcreteConferences.FirstAsync(x => x.id == conferenceId);
                var halls = await _context.Halls.Where(x => x.EventCenterid == conference.centerId).ToArrayAsync();
                return halls;
            }
        }

        public async Task<bool> Edit(Guid id, string name, DateTime starttime, DateTime endtime, Guid Hallid)
        {
            var mealtoupdate = await _context.Meals.FirstOrDefaultAsync(s => s.id == id);
            mealtoupdate.name = name;
            mealtoupdate.starttime = starttime;
            mealtoupdate.endtime = endtime;
            mealtoupdate.Hallid = Hallid;
            _context.Update(mealtoupdate);
            var saveResult = await _context.SaveChangesAsync(); return saveResult == 1;
        }

        public async Task<bool> Delete(Guid id)
        {
            var mealtodelete = await _context.Meals.FirstOrDefaultAsync(s => s.id == id);
            if (mealtodelete.concreteConferenceId != null)
            {
                var conference = await _context.ConcreteConferences.Where(x => x.id == mealtodelete.concreteConferenceId).FirstAsync();
                conference.Events.Remove(mealtodelete);
            }
            _context.Meals.Attach(mealtodelete);
            _context.Meals.Remove(mealtodelete);
            var saveResult = await _context.SaveChangesAsync();
            return saveResult == 1;
        }

        public async Task<bool> CheckUser(Guid id, string UserId)
        {
            var meal = await _context.Meals.FirstOrDefaultAsync(x => x.id == id);
            return (meal.UserId == UserId);
        }

    }
}
