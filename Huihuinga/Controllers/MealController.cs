using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Huihuinga.Models;
using Huihuinga.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Huihuinga.Controllers
{
    public class MealController : Controller
    {
        private readonly IMealService _MealService;
        public IHostingEnvironment HostingEnvironment { get; }
        public MealController(IMealService mealService, IHostingEnvironment hostingEnvironment)
        {
            _MealService = mealService;
            HostingEnvironment = hostingEnvironment;
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
        public async Task<IActionResult> New(Guid? id)
        {
            ViewData["concreteConferenceId"] = id;
            var halls = await _MealService.GetHalls();
            var model = new MealCreateViewModel()
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
        public async Task<IActionResult> Create(MealCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("New");
            }

            string uniqueFileName = null;
            if (model.Photo != null)
            {
                string uploadsFolder = Path.Combine(HostingEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                model.Photo.CopyTo(new FileStream(filePath, FileMode.Create));
            }
            Meal newmeal = new Meal();
            newmeal.name = model.name;
            newmeal.starttime = model.starttime;
            newmeal.endtime = model.endtime;
            newmeal.PhotoPath = uniqueFileName;
            newmeal.Hallid = model.Hallid;
            newmeal.concreteConferenceId = model.concreteConferenceId;

            var successful = await _MealService.Create(newmeal);
            if (!successful)
            {
                return BadRequest("Could not add item.");
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var model = await _MealService.Details(id);
            return View(model);
        }

        public async Task<IActionResult> Update(Meal meal)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Edit", new { id = meal.id });
            }

            var successful = await _MealService.Edit(meal.id, meal.name, meal.starttime, meal.endtime, meal.Hallid);
            if (!successful)
            {
                return BadRequest("Could not edit item.");
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var successful = await _MealService.Delete(id);
            if (!successful)
            {
                return BadRequest("Could not delete item.");
            }
            return RedirectToAction("Index");
        }
    }
}