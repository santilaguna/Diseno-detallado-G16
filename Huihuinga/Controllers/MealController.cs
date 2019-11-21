using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Huihuinga.Models;
using Huihuinga.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Huihuinga.Controllers
{
    public class MealController : Controller
    {
        private readonly IMealService _MealService;
        public IHostingEnvironment HostingEnvironment { get; }
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEventService _eventService;
        public MealController(IMealService mealService, IHostingEnvironment hostingEnvironment,
                              UserManager<ApplicationUser> userManager, IEventService eventService)
        {
            _MealService = mealService;
            HostingEnvironment = hostingEnvironment;
            _userManager = userManager;
            _eventService = eventService;
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

        [Authorize]
        public async Task<IActionResult> New(Guid? id)
        {
            ViewData["concreteConferenceId"] = id;
            var halls = await _MealService.GetHalls(id);
            var model = new MealCreateViewModel()
            {
                Halls = halls
            };

            return View(model);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var UserId = "";
            ViewData["currentUser"] = false;
            if (currentUser != null)
            {
                UserId = currentUser.Id;
                ViewData["currentUser"] = true;
            }
            var authorized = await _MealService.CheckUser(id, UserId);
            var menus = await _MealService.GetMenu(id);
            ViewData["menus"] = menus;
            ViewData["owner"] = authorized;
            
            var model = await _MealService.Details(id);
            var eventLimit = await _eventService.CheckLimitUsers(model);
            var maxAssistants = await _eventService.GetMaxAssistants(model.Hallid);
            ViewData["maxAssistants"] = maxAssistants;
            var actualUsers = await _eventService.GetActualUsers(model);
            ViewData["availableSpace"] = maxAssistants - actualUsers;
            
            if (currentUser != null && eventLimit)
            {
                ViewData["userSubscribed"] = await _eventService.CheckSubscribedUser(UserId, id);
            }
            else
            {
                ViewData["userSubscribed"] = true;
            }
            return View(model);
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MealCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("New", new { id = model.concreteConferenceId});
            }

            if (model.starttime >= model.endtime)
            {
                return RedirectToAction("New", new { id = model.concreteConferenceId });
            }

            string uniqueFileName = null;
            if (model.Photo != null)
            {
                string uploadsFolder = Path.Combine(HostingEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                model.Photo.CopyTo(new FileStream(filePath, FileMode.Create));
            }
            var currentUser = await _userManager.GetUserAsync(User);
            Meal newmeal = new Meal();
            newmeal.name = model.name;
            newmeal.starttime = model.starttime;
            newmeal.endtime = model.endtime;
            newmeal.PhotoPath = uniqueFileName;
            newmeal.Hallid = model.Hallid;
            newmeal.concreteConferenceId = model.concreteConferenceId;
            newmeal.UserId = currentUser.Id;

            var successful = await _MealService.Create(newmeal);
            if (!successful)
            {
                return BadRequest("Could not add item.");
            }
            return RedirectToAction("Details", new { newmeal.id });
        }

        [Authorize]
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

            if (meal.starttime >= meal.endtime)
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

        [Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            var model = await _MealService.Details(id);
            var concreteConferenceId = model.concreteConferenceId;
            var successful = await _MealService.Delete(id);
            if (!successful)
            {
                return BadRequest("Could not delete item.");
            }
            if (concreteConferenceId == null)
            {
                return RedirectToAction("Index");
            }
            return RedirectToAction("Details", "ConcreteConference", new { id = concreteConferenceId });
        }

        [Authorize]
        public IActionResult NewMenu(Guid id)
        {
            ViewData["event_id"] = id;
            return View();
        }

        public async Task<IActionResult> CreateMenu(MenuCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("NewMenu");
            }

            string uploadsFolder = Path.Combine(HostingEnvironment.WebRootPath, "images");
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.file.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);
            model.file.CopyTo(new FileStream(filePath, FileMode.Create));

            Menu newmenu = new Menu();
            newmenu.name = model.name;
            newmenu.filename = uniqueFileName;
            newmenu.EventId = model.EventId;
            newmenu.menu = model.menu;
            var successful = await _MealService.CreateMenu(newmenu);
            if (!successful)
            {
                return BadRequest("Could not add item.");
            }
            return RedirectToAction("Details", new { id = newmenu.EventId });

        }

        public async Task<IActionResult> DeleteMenu(Guid MenuId, Guid EventId)
        {
            var successful = await _MealService.DeleteMenu(MenuId);
            if (!successful)
            {
                return BadRequest("Could not delete item.");
            }
            return RedirectToAction("Details", new { id = EventId });
        }


        public async Task<IActionResult> ShowMenu(Guid MenuId)
        {
            var model = await _MealService.ShowMenu(MenuId);
            return View(model);
        }
        [Authorize]
        public async Task<IActionResult> Join(Guid eventId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Challenge();
            var successful = await _eventService.AddUser(currentUser, eventId);
            if (!successful)
            {
                return BadRequest("Could not add User.");
            }
            return RedirectToAction("Details", new { id = eventId });
        }
        [Authorize]
        public async Task<IActionResult> Disjoint(Guid eventId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Challenge();
            var successful = await _eventService.DeleteUser(currentUser, eventId);
            if (!successful)
            {
                return BadRequest("Could not remove User.");
            }
            return RedirectToAction("Details", new { id = eventId });
        }
    }
}