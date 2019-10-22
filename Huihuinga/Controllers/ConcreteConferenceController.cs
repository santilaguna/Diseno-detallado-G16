using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Huihuinga.Models;
using Huihuinga.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Huihuinga.Controllers
{
    public class ConcreteConferenceController : Controller
    {

        private readonly IConcreteConferenceService _concreteConferenceService;
        private readonly UserManager<ApplicationUser> _userManager;
        public ConcreteConferenceController(IConcreteConferenceService concreteConferenceService, 
            UserManager<ApplicationUser> userManager)
        {
            _concreteConferenceService = concreteConferenceService;
            _userManager = userManager;
        }


        // GET: /<controller>/
        public async Task<IActionResult> Index()
        {
            var conferences = await _concreteConferenceService.GetConcreteConferencesAsync();
            var model = new ConcreteConferenceViewModel()
            {
                ConcreteConferences = conferences
            };
            return View(model);
        }
        
        // Add [Authorize]
        public IActionResult New()
        {
            return View();
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var model = await _concreteConferenceService.Details(id);
            return View(model);
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ConcreteConference newConcreteConference)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("New");
            }
            var successful = await _concreteConferenceService.Create(newConcreteConference);
            if (!successful)
            {
                return BadRequest("Could not add item.");
            }
            return RedirectToAction("Index");
        }
        
        [Authorize]
        public async Task<IActionResult> Join(Guid conferenceId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Challenge();
            var successful = await _concreteConferenceService.AddUser(currentUser, conferenceId);
            if (!successful)
            {
                return BadRequest("Could not add User.");
            }
            return RedirectToAction("Details", new {id = conferenceId});
        }
    }
}