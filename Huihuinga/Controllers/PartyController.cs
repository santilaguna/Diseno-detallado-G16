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
    public class PartyController : Controller
    {
        // GET: /<controller>/
        private readonly IPartyService _PartyService;
        public IHostingEnvironment HostingEnvironment { get; }
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEventService _eventService;
        public PartyController(IPartyService partyservice, IHostingEnvironment hostingEnvironment,
                               UserManager<ApplicationUser> userManager, IEventService eventService)
        {
            _PartyService = partyservice;
            HostingEnvironment = hostingEnvironment;
            _userManager = userManager;
            _eventService = eventService;
        }


        // GET: /<controller>/
        public async Task<IActionResult> Index()
        {
            var parties = await _PartyService.GetPartiesAsync();
            var model = new PartyViewModel()
            {
                Parties = parties
            };
            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> New(Guid? id)
        {
            ViewData["concreteConferenceId"] = id;
            var halls = await _PartyService.GetHalls(id);
            var model = new PartyCreateViewModel()
            {
                Halls = halls
            };

            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> Edit(Guid id)
        {
            var model = await _PartyService.Details(id);
            return View(model);
        }

        public async Task<IActionResult> Update(Party party)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Edit", new { id = party.id });
            }

            if (party.starttime >= party.endtime)
            {
                return RedirectToAction("Edit", new { id = party.id });
            }

            var successful = await _PartyService.Edit(party.id, party.name, party.starttime, party.endtime, party.Hallid, party.description);
            if (!successful)
            {
                return BadRequest("Could not edit item.");
            }
            return RedirectToAction("Index");
        }

        [Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            var model = await _PartyService.Details(id);
            var concreteConferenceId = model.concreteConferenceId;
            var successful = await _PartyService.Delete(id);
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
            var authorized = await _PartyService.CheckUser(id, UserId);
            ViewData["owner"] = authorized;
            
            var model = await _PartyService.Details(id);
            var eventLimit = await _eventService.CheckLimitUsers(model);
            var maxAssistants = await _eventService.GetMaxAssistants(model.Hallid);
            ViewData["maxAssistants"] = maxAssistants;
            var actualUsers = await _eventService.GetActualUsers(model);
            ViewData["availableSpace"] = maxAssistants - actualUsers;

            ViewData["finished"] = false;
            if (model.endtime < DateTime.Now)
            {
                ViewData["finished"] = true;
            }

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
        public async Task<IActionResult> Create(PartyCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("New", new { id = model.concreteConferenceId });
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
            Party newparty = new Party();
            newparty.name = model.name;
            newparty.starttime = model.starttime;
            newparty.endtime = model.endtime;
            newparty.PhotoPath = uniqueFileName;
            newparty.Hallid = model.Hallid;
            newparty.description = model.description;
            newparty.concreteConferenceId = model.concreteConferenceId;
            newparty.UserId = currentUser.Id;
            newparty.feedbacks = new List<Feedback> { };

            var successful = await _PartyService.Create(newparty);
            if (!successful)
            {
                return BadRequest("Could not add item.");
            }
            return RedirectToAction("Details", new { newparty.id });
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

        [Authorize]
        public async Task<IActionResult> PendingFeedbacks()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var parties = await _PartyService.GetPartiesWithPendingFeedbacks(currentUser.Id);
            var model = new PartyViewModel()
            {
                Parties = parties
            };
            return View(model);
        }

        [Authorize]
        public IActionResult NewFeedback(Guid eventid)
        {
            ViewData["event_id"] = eventid;
            return View();
        }

        [Authorize]
        public async Task<IActionResult> CreateFeedback(Feedback feedback)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("NewFeedback", new { id = feedback.EventId });
            }
            var currentUser = await _userManager.GetUserAsync(User);
            feedback.UserId = currentUser.Id;
            feedback.dateTime = DateTime.Now;
            var successful = await _PartyService.CreateFeedback(feedback, feedback.EventId);
            if (!successful)
            {
                return BadRequest("Could not add item.");
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> FinishedParties()
        {
            var parties = await _PartyService.GetFinishedParties();
            var model = new PartyViewModel()
            {
                Parties = parties
            };
            return View(model);
        }

        public async Task<IActionResult> ViewFeedbacks(Guid eventId)
        {
            ViewData["MusicQuality"] = await _PartyService.MusicQuality(eventId);
            ViewData["PlaceQuality"] = await _PartyService.PlaceQuality(eventId);
            ViewData["Comments"] = await _PartyService.Comments(eventId);
            ViewData["event_id"] = eventId;
            return View();
        }
    }
}
