using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Huihuinga.Models;
using Huihuinga.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Huihuinga.Controllers
{
    public class MealController : Controller
    {
        private readonly IMealService _MealService;
        public MealController(IMealService mealService)
        {
            _MealService = mealService;
        }


        // GET: /<controller>/
        public async Task<IActionResult> Index()
        {
            var meals = await _MealService.GetMealsAsync();
            var model = new MealViewModel()
            {
                Meals = meals
            };
            return View(model);
        }
        public async Task<IActionResult> New()
        {
            var halls = await _MealService.GetHalls();
            var model = new HallViewModel()
            {
                Halls = halls
            };

            return View(model);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var model = await _MealService.Details(id);
            return View(model);
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Meal newmeal)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("New");
            }
            var successful = await _MealService.Create(newmeal);
            if (!successful)
            {
                return BadRequest("Could not add item.");
            }
            return RedirectToAction("Index");
        }
    }
}