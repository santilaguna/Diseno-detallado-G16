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
    public class PracticalSessionController : Controller, ITopicalController
    {
        // GET: /<controller>/
        private readonly IPracticalSessionService _PracticalService;
        public IHostingEnvironment HostingEnvironment { get; }
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEventService _eventService;
        public PracticalSessionController(IPracticalSessionService practicalservice, IHostingEnvironment hostingEnvironment,
                                          UserManager<ApplicationUser> userManager, IEventService eventService)
        {
            _PracticalService = practicalservice;
            HostingEnvironment = hostingEnvironment;
            _userManager = userManager;
            _eventService = eventService;
        }


        // GET: /<controller>/
        public async Task<IActionResult> Index()
        {
            var practical = await _PracticalService.GetSessionsAsync();
            var model = new PracticalSessionViewModel()
            {
                PracticalSessions = practical
            };
            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> New(Guid? id)
        {
            ViewData["concreteConferenceId"] = id;
            var halls = await _PracticalService.GetHalls(id);
            var users = await _eventService.GetAllUsers();
            var model = new PracticalSessionCreateViewModel()
            {
                Halls = halls,
                Users = users
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
            var authorized = await _PracticalService.CheckUser(id, UserId);
            var materials = await _PracticalService.GetMaterial(id);
            ViewData["materials"] = materials;
            ViewData["owner"] = authorized;
            
            var model = await _PracticalService.Details(id);
            var eventLimit = await _eventService.CheckLimitUsers(model);
            var maxAssistants = await _eventService.GetMaxAssistants(model.Hallid);
            ViewData["maxAssistants"] = maxAssistants;
            var actualUsers = await _eventService.GetActualUsers(model);
            ViewData["availableSpace"] = maxAssistants - actualUsers;
            var expositor = await _eventService.GetUserName(model.ExpositorId);
            ViewData["expositor"] = expositor;
            ViewData["expositor permission"] = false;
            if (currentUser != null && currentUser.Id == model.ExpositorId)
            {
                ViewData["expositor permission"] = true;
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
        public async Task<IActionResult> Create(PracticalSessionCreateViewModel model)
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
            PracticalSession newsession = new PracticalSession();
            newsession.name = model.name;
            newsession.starttime = model.starttime;
            newsession.endtime = model.endtime;
            newsession.PhotoPath = uniqueFileName;
            newsession.Hallid = model.Hallid;
            newsession.concreteConferenceId = model.concreteConferenceId;
            newsession.UserId = currentUser.Id;
            newsession.ExpositorId = model.ExpositorId;

            var successful = await _PracticalService.Create(newsession);
            if (!successful)
            {
                return BadRequest("Could not add item.");
            }
            return RedirectToAction("Details", new { newsession.id });
        }

        [Authorize]
        public async Task<IActionResult> Edit(Guid id)
        {
            var model = await _PracticalService.Details(id);
            return View(model);
        }

        public async Task<IActionResult> Update(PracticalSession session)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Edit", new { id = session.id });
            }

            if (session.starttime >= session.endtime)
            {
                return RedirectToAction("Edit", new { id = session.id });
            }

            var successful = await _PracticalService.Edit(session.id, session.name, session.starttime, session.endtime, session.Hallid);
            if (!successful)
            {
                return BadRequest("Could not edit item.");
            }
            return RedirectToAction("Index");
        }

        [Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            var model = await _PracticalService.Details(id);
            var concreteConferenceId = model.concreteConferenceId;
            var successful = await _PracticalService.Delete(id);
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

        public async Task<IActionResult> NewTopic(Guid id)
        {
            ViewData["event_id"] = id;
            var topics = await _PracticalService.NewTopic(id);
            var model = new TopicViewModel()
            {
                Topics = topics
            };
            return View(model);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddNewTopic(Guid id, Topic topic)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("NewTopic", new { id });
            }
            var successful = await _PracticalService.AddNewTopic(id, topic);
            if (!successful)
            {
                return BadRequest("Could not add item.");
            }
            return RedirectToAction("Details", new { id });
        }

        public async Task<IActionResult> AddTopic(Guid id, Guid topicId)
        {
            var successful = await _PracticalService.AddTopic(id, topicId);
            if (!successful)
            {
                return BadRequest("Could not add item.");
            }
            return RedirectToAction("Details", new { id });
        }

        [Authorize]
        public IActionResult NewMaterial(Guid id)
        {
            ViewData["event_id"] = id;
            return View();
        }

        public async Task<IActionResult> CreateMaterial(MaterialCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("NewMaterial");
            }

            string uploadsFolder = Path.Combine(HostingEnvironment.WebRootPath, "files");
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.file.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);
            model.file.CopyTo(new FileStream(filePath, FileMode.Create));

            Material newmaterial = new Material();
            newmaterial.name = model.name;
            newmaterial.filename = uniqueFileName;
            newmaterial.EventId = model.EventId;
            var successful = await _PracticalService.CreateMaterial(newmaterial);
            if (!successful)
            {
                return BadRequest("Could not add item.");
            }
            return RedirectToAction("Details", new { id = newmaterial.EventId });

        }

        public async Task<IActionResult> DeleteMaterial(Guid MaterialId, Guid EventId)
        {
            var successful = await _PracticalService.DeleteMaterial(MaterialId);
            if (!successful)
            {
                return BadRequest("Could not delete item.");
            }
            return RedirectToAction("Details", new { id =  EventId});
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
