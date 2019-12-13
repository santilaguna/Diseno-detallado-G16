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
    public class TalkController : Controller, ITopicalController
    {
        // GET: /<controller>/
        private readonly ITalkService _TalkService;
        public IHostingEnvironment HostingEnvironment { get; }
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEventService _eventService;
        public TalkController(ITalkService talkservice, IHostingEnvironment hostingEnvironment,
                              UserManager<ApplicationUser> userManager, IEventService eventService)
        {
            _TalkService = talkservice;
            HostingEnvironment = hostingEnvironment;
            _userManager = userManager;
            _eventService = eventService;
        }


        // GET: /<controller>/
        public async Task<IActionResult> Index()
        {
            var talks = await _TalkService.GetTalksAsync();
            var users = await _eventService.GetAllUsers();
            var model = new TalkViewModel()
            {
                Talks = talks
            };
            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> New(Guid? id)
        {
            ViewData["concreteConferenceId"] = id;
            var halls = await _TalkService.GetHalls(id);
            var users = await _eventService.GetAllUsers();
            var model = new TalkCreateViewModel()
            {
                Users = users,
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
            var authorized = await _TalkService.CheckUser(id, UserId);
            var materials = await _TalkService.GetMaterial(id);
            ViewData["materials"] = materials;
            ViewData["owner"] = authorized;
    
            var model = await _TalkService.Details(id);
            var eventLimit = await _eventService.CheckLimitUsers(model);
            var maxAssistants = await _eventService.GetMaxAssistants(model.Hallid);
            ViewData["maxAssistants"] = maxAssistants;
            var actualUsers = await _eventService.GetActualUsers(model);
            ViewData["availableSpace"] = maxAssistants - actualUsers;

            ViewData["can_feedback"] = false;
            if (model.concreteConferenceId != null)
            {
                ViewData["can_feedback"] = await _TalkService.CanFeedback(currentUser.Id, id);
            }

            ViewData["finished"] = false;
            if (model.endtime < DateTime.Now)
            {
                ViewData["finished"] = true;
            }

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
        public async Task<IActionResult> Create(TalkCreateViewModel model)
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
            Talk newtalk = new Talk();
            newtalk.name = model.name;
            newtalk.starttime = model.starttime;
            newtalk.endtime = model.endtime;
            newtalk.PhotoPath = uniqueFileName;
            newtalk.Hallid = model.Hallid;
            newtalk.description = model.description;
            newtalk.concreteConferenceId = model.concreteConferenceId;
            newtalk.UserId = currentUser.Id;
            newtalk.ExpositorId = model.ExpositorId;
            newtalk.feedbacks = new List<Feedback> { };

            var successful = await _TalkService.Create(newtalk);
            if (!successful)
            {
                return BadRequest("Could not add item.");
            }
            return RedirectToAction("Details", new { newtalk.id } );
        }

        [Authorize]
        public async Task<IActionResult> Edit(Guid id)
        {
            var model = await _TalkService.Details(id);
            return View(model);
        }

        public async Task<IActionResult> Update(Talk talk)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Edit", new { id = talk.id });
            }

            if (talk.starttime >= talk.endtime)
            {
                return RedirectToAction("Edit", new { id = talk.id });
            }

            var successful = await _TalkService.Edit(talk.id, talk.name, talk.starttime, talk.endtime, talk.Hallid, talk.description);
            if (!successful)
            {
                return BadRequest("Could not edit item.");
            }
            return RedirectToAction("Index");
        }

        [Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            var model = await _TalkService.Details(id);
            var concreteConferenceId = model.concreteConferenceId;
            var successful = await _TalkService.Delete(id);
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
            var topics = await _TalkService.NewTopic(id);
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
            var successful = await _TalkService.AddNewTopic(id, topic);
            if (!successful)
            {
                return BadRequest("Could not add item.");
            }
            return RedirectToAction("Details", new { id });
        }

        public async Task<IActionResult> AddTopic(Guid id, Guid topicId)
        {
            var successful = await _TalkService.AddTopic(id, topicId);
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
            var successful = await _TalkService.CreateMaterial(newmaterial);
            if (!successful)
            {
                return BadRequest("Could not add item.");
            }
            return RedirectToAction("Details", new { id = newmaterial.EventId });

        }

        public async Task<IActionResult> DeleteMaterial(Guid MaterialId, Guid EventId)
        {
            var successful = await _TalkService.DeleteMaterial(MaterialId);
            if (!successful)
            {
                return BadRequest("Could not delete item.");
            }
            return RedirectToAction("Details", new { id = EventId });
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

        public async Task<IActionResult> PendingFeedbacks()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var talks = await _TalkService.GetTalksWithPendingFeedbacks(currentUser.Id);
            var model = new TalkViewModel()
            {
                Talks = talks
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
            var successful = await _TalkService.CreateFeedback(feedback, feedback.EventId);
            if (!successful)
            {
                return BadRequest("Could not add item.");
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> FinishedTalks()
        {
            var talks = await _TalkService.GetFinishedTalks();
            var model = new TalkViewModel()
            {
                Talks = talks
            };
            return View(model);
        }

        public async Task<IActionResult> ViewFeedbacks(Guid eventId)
        {
            ViewData["MaterialQuality"] = await _TalkService.MaterialQuality(eventId);
            ViewData["PlaceQuality"] = await _TalkService.PlaceQuality(eventId);
            ViewData["ExpositorQuality"] = await _TalkService.ExpositorQuality(eventId);
            ViewData["Comments"] = await _TalkService.Comments(eventId);
            ViewData["event_id"] = eventId;
            return View();
        }

        public async Task<IActionResult> NewConferenceFeedback(Guid eventid, Guid ConcreteConferenceId)
        {
            ViewData["event_id"] = eventid;
            ViewData["ConferenceId"] = await _eventService.ObtainConference(ConcreteConferenceId);
            ViewData["ConcreteConferenceId"] = ConcreteConferenceId;
            return View();

        }

        public async Task<IActionResult> CreateConferenceFeedback(ConferenceFeedback feedback)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("NewConferenceFeedback", new
                {
                    eventid = feedback.EventId,
                    ConcreteConferenceId = feedback.ConcreteConferenceId
                });
            }
            var currentUser = await _userManager.GetUserAsync(User);
            feedback.UserId = currentUser.Id;
            feedback.dateTime = DateTime.Now;
            var successful = await _eventService.CreateConferenceFeedback(feedback);
            if (!successful)
            {
                return BadRequest("Could not add item.");
            }
            return RedirectToAction("Details", new { id = feedback.EventId });
        }
    }
}
