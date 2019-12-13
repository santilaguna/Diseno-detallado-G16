using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Huihuinga.Models;
using Huihuinga.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Huihuinga.Controllers
{
    public class ConferenceController : Controller
    {

        private readonly IConferenceService _conferenceService;
        public IHostingEnvironment HostingEnvironment { get; }
        private readonly UserManager<ApplicationUser> _userManager;
        public ConferenceController(IConferenceService conferenceService, IHostingEnvironment hostingEnvironment,
                                    UserManager<ApplicationUser> userManager)
        {
            _conferenceService = conferenceService;
            HostingEnvironment = hostingEnvironment;
            _userManager = userManager;
        }


        // GET: /<controller>/
        public async Task<IActionResult> Index()
        {
            var conferences = await _conferenceService.GetConferencesAsync();
            var model = new ConferenceViewModel()
            {
                Conferences = conferences
            };
            return View(model);
        }
        
        [Authorize]
        public IActionResult New()
        {
            return View();
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
            var authorized = await _conferenceService.CheckUser(id, UserId);
            var model = await _conferenceService.Details(id);
            ViewData["owner"] = authorized;
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ConferenceCreateViewModel model)
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

            var currentUser = await _userManager.GetUserAsync(User);
            Conference newConference = new Conference();
            newConference.calendarRepetition = model.calendarRepetition;
            newConference.name = model.name;
            newConference.PhotoPath = uniqueFileName;
            newConference.description = model.description;
            newConference.UserId = currentUser.Id;

            var successful = await _conferenceService.Create(newConference);
            if (!successful)
            {
                return BadRequest("Could not add item.");
            }
            return RedirectToAction("Index");
        }

        [Authorize]
        public async Task<IActionResult> Edit(Guid id)
        {
            var model = await _conferenceService.Details(id);
            return View(model);
        }

        public async Task<IActionResult> Update(Conference conference)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Edit", new { id = conference.id });
            }

            var successful = await _conferenceService.Edit(conference.id, conference.name, conference.description, 
                conference.calendarRepetition);
            if (!successful)
            {
                return BadRequest("Could not edit item.");
            }
            return RedirectToAction("Index");
        }

        [Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            var successful = await _conferenceService.Delete(id);
            if (!successful)
            {
                return BadRequest("Could not delete item.");
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> ViewFeedbacks(Guid ConferenceId)
        {
            ViewData["FoodQuality"] = await _conferenceService.FoodQuality(ConferenceId);
            ViewData["MusicQuality"] = await _conferenceService.MusicQuality(ConferenceId);
            ViewData["PlaceQuality"] = await _conferenceService.PlaceQuality(ConferenceId);
            ViewData["DiscussionQuality"] = await _conferenceService.DiscussionQuality(ConferenceId);
            ViewData["MaterialQuality"] = await _conferenceService.MaterialQuality(ConferenceId);
            ViewData["ExpositorQuality"] = await _conferenceService.ExpositorQuality(ConferenceId);
            ViewData["Comments"] = await _conferenceService.Comments(ConferenceId);
            ViewData["conference_id"] = ConferenceId;
            return View();
        }


    }
}
