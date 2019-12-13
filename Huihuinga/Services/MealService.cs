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
            var meals = await _context.Meals.Where(e => e.concreteConferenceId == null && e.endtime > DateTime.Now).ToArrayAsync();
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

        public async Task<bool> CreateMenu(Menu menu)
        {
            menu.Id = Guid.NewGuid();
            _context.Menus.Add(menu);
            var saveResult = await _context.SaveChangesAsync();
            return saveResult == 1;
        }

        public async Task<Menu[]> GetMenu(Guid id)
        {
            var Menus = await _context.Menus.Where(x => x.EventId == id).ToArrayAsync();
            return Menus;
        }

        public async Task<bool> DeleteMenu(Guid MenuId)
        {
            var menutodelete = await _context.Menus.FirstOrDefaultAsync(s => s.Id == MenuId);
            _context.Menus.Attach(menutodelete);
            _context.Menus.Remove(menutodelete);
            var saveResult = await _context.SaveChangesAsync();
            return saveResult == 1;
        }

        public async Task<Menu> ShowMenu(Guid MenuId)
        {
            var menus = await _context.Menus.Where(x => x.Id == MenuId).ToArrayAsync();
            return menus[0];
        }

        public async Task<Meal[]> GetMealsWithPendingFeedbacks(string UserId)
        {
            var UsersEvent = await _context.UserEvents.Where(e => e.UserId == UserId).ToArrayAsync();
            var EventsId = new List<Guid> { };
            foreach (ApplicationUserEvent userevent in UsersEvent)
            {
                EventsId.Add(userevent.EventId);
            }
            var feedbacks = await _context.Feedbacks.Where(e => e.UserId == UserId).ToArrayAsync();
            var EventsWithFeedbackId = new List<Guid> { };
            foreach (Feedback feedback in feedbacks)
            {
                EventsWithFeedbackId.Add(feedback.EventId);
            }
            var meals = await _context.Meals.Where(e => EventsId.Contains(e.id) && !EventsWithFeedbackId.Contains(e.id)
                            && e.concreteConferenceId == null && e.endtime < DateTime.Now).ToArrayAsync();

            return meals;
        }

        public async Task<bool> CreateFeedback(Feedback feedback, Guid event_id)
        {
            var meal = await _context.Meals.FirstOrDefaultAsync(e => e.id == event_id);
            feedback.id = Guid.NewGuid();
            _context.Feedbacks.Add(feedback);
            meal.feedbacks.Add(feedback);
            var saveResult = await _context.SaveChangesAsync();
            return saveResult == 1;
        }

        public async Task<Meal[]> GetFinishedMeals()
        {
            var meals = await _context.Meals.Where(e => e.concreteConferenceId == null && e.endtime < DateTime.Now).ToArrayAsync();
            return meals;
        }

        public async Task<double> FoodQuality(Guid eventId)
        {
            var feedbacks = await _context.Feedbacks.Where(e => e.EventId == eventId).ToArrayAsync();
            int Quality = 0;
            foreach (Feedback feedback in feedbacks)
            {
                Quality += feedback.FoodQuality;
            }

            return Quality / feedbacks.Length;
        }

        public async Task<List<string>> Comments(Guid eventId)
        {
            var feedbacks = await _context.Feedbacks.Where(e => e.EventId == eventId).ToArrayAsync();
            var comments = new List<string> { };
            foreach (Feedback feedback in feedbacks)
            {
                comments.Add(feedback.comment);
            }
            return comments;
        }

    }
}
