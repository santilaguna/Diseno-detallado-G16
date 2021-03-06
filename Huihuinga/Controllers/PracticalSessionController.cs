﻿using System;
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
        private readonly INotificationService _notificationService;
        public PracticalSessionController(IPracticalSessionService practicalservice, IHostingEnvironment hostingEnvironment,
                                          UserManager<ApplicationUser> userManager, IEventService eventService,
                                          INotificationService notificationService)
        {
            _PracticalService = practicalservice;
            HostingEnvironment = hostingEnvironment;
            _userManager = userManager;
            _eventService = eventService;
            _notificationService = notificationService;
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

            ViewData["can_feedback"] = false;
            if (currentUser != null && model.concreteConferenceId != null)
            {
                ViewData["can_feedback"] = await _PracticalService.CanFeedback(currentUser.Id, id);
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
            newsession.feedbacks = new List<Feedback> { };

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

        [Authorize]
        public async Task<IActionResult> PendingFeedbacks()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var sessions = await _PracticalService.GetSessionsWithPendingFeedbacks(currentUser.Id);
            var model = new PracticalSessionViewModel()
            {
                PracticalSessions = sessions
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
            var successful = await _PracticalService.CreateFeedback(feedback, feedback.EventId);
            if (!successful)
            {
                return BadRequest("Could not add item.");
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> FinishedSessions()
        {
            var sessions = await _PracticalService.GetFinishedSessions();
            var model = new PracticalSessionViewModel()
            {
                PracticalSessions = sessions
            };
            return View(model);
        }

        public async Task<IActionResult> ViewFeedbacks(Guid eventId)
        {
            ViewData["MaterialQuality"] = await _PracticalService.MaterialQuality(eventId);
            ViewData["PlaceQuality"] = await _PracticalService.PlaceQuality(eventId);
            ViewData["ExpositorQuality"] = await _PracticalService.ExpositorQuality(eventId);
            ViewData["Comments"] = await _PracticalService.Comments(eventId);
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

        [Authorize]
        public async Task<IActionResult> SendNotification(Guid id, string mailBodyMessage)
        {
            var users = await _eventService.GetUsersAsync(id);
            await _notificationService.SendEventNotification(users, mailBodyMessage);
            return RedirectToAction("Details", new { id });
        }
    }
}
