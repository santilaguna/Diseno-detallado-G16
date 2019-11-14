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
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace Huihuinga.Controllers
{
    public class ConcreteConferenceController : Controller
    {

        private readonly IConcreteConferenceService _concreteConferenceService;
        private readonly UserManager<ApplicationUser> _userManager;
        public IHostingEnvironment HostingEnvironment { get; }
        public ConcreteConferenceController(IConcreteConferenceService concreteConferenceService, 
            UserManager<ApplicationUser> userManager, IHostingEnvironment hostingEnvironment)
        {
            _concreteConferenceService = concreteConferenceService;
            _userManager = userManager;
            HostingEnvironment = hostingEnvironment;
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
        public async Task<IActionResult> New(Guid id)
        {
            ViewData["abstractConferenceId"] = id;
            var centers = await _concreteConferenceService.GetEventCenters();
            var model = new ConcreteConferenceCreateViewModel()
            {
                EventCenters = centers
            };

            return View(model);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var model = await _concreteConferenceService.Details(id);
            var currentUser = await _userManager.GetUserAsync(User);
            var conferenceLimit = await _concreteConferenceService.CheckLimitUsers(model);
            
            if (currentUser != null && conferenceLimit)
            {
                var userSubscribed = await _concreteConferenceService.CheckUser(currentUser.Id, id);
                if (userSubscribed)
                {
                    ViewData["userSubscribed"] = true;
                }
                else
                {
                    ViewData["userSubscribed"] = false;
                }
            }
            else
            {
                ViewData["userSubscribed"] = true;
            }
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ConcreteConferenceCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("New", new { id = model.abstractConferenceId });
            }

            string uniqueFileName = null;
            if (model.Photo != null)
            {
                string uploadsFolder = Path.Combine(HostingEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                model.Photo.CopyTo(new FileStream(filePath, FileMode.Create));
            }

            ConcreteConference newConcreteConference = new ConcreteConference();
            newConcreteConference.name = model.name;
            newConcreteConference.abstractConferenceId= model.abstractConferenceId;
            newConcreteConference.endtime = model.endtime;
            newConcreteConference.starttime = model.starttime;
            newConcreteConference.Maxassistants = model.Maxassistants;
            newConcreteConference.PhotoPath = uniqueFileName;
            newConcreteConference.centerId = model.centerId;
            newConcreteConference.Events = new List<Event> { };

            var successful = await _concreteConferenceService.Create(newConcreteConference);
            if (!successful)
            {
                return BadRequest("Could not add item.");
            }
            return RedirectToAction("Details", new { newConcreteConference.id});
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

        public async Task<IActionResult> Edit(Guid id)
        {
            var model = await _concreteConferenceService.Details(id);
            return View(model);
        }

        public async Task<IActionResult> Update(ConcreteConference concreteConference)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Edit", new { concreteConference.id });
            }

            var successful = await _concreteConferenceService.Edit(concreteConference.id, concreteConference.name,
                concreteConference.starttime, concreteConference.endtime, concreteConference.Maxassistants);

            if (!successful)
            {
                return BadRequest("Could not edit item.");
            }
            return RedirectToAction("Details", new { concreteConference.id });
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var model = await _concreteConferenceService.Details(id);
            var abstractConferenceId = model.abstractConferenceId;
            var successful = await _concreteConferenceService.Delete(id);
            if (!successful)
            {
                return BadRequest("Could not delete item.");
            }
            return RedirectToAction("Details", "Conference", new { id = abstractConferenceId});
        }

        public async Task<IActionResult> ShowEvents(Guid id)
        {
            var events = await _concreteConferenceService.ShowEvents(id);
            ViewData["concreteConferenceId"] = id;
            var model = new EventViewModel()
            {
                Events = events
            };
            return View(model);
        }
    }
}
