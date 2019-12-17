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
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace Huihuinga.Controllers
{
    public class ConcreteConferenceController : Controller
    {

        private readonly IConcreteConferenceService _concreteConferenceService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITopicService _TopicService;
        private readonly INotificationService _notificationService;
        public IHostingEnvironment HostingEnvironment { get; }
        public ConcreteConferenceController(IConcreteConferenceService concreteConferenceService,
            UserManager<ApplicationUser> userManager, ITopicService topicService,
            INotificationService notificationService, IHostingEnvironment hostingEnvironment)
        {
            _concreteConferenceService = concreteConferenceService;
            _userManager = userManager;
            _TopicService = topicService;
            _notificationService = notificationService;
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

        [Authorize]
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
            var UserId = "";
            ViewData["currentUser"] = false;
            if (currentUser != null)
            {
                UserId = currentUser.Id;
                ViewData["currentUser"] = true;
            }
            var authorized = await _concreteConferenceService.CheckOwner(id, UserId);
            ViewData["owner"] = authorized;

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

            var currentUser = await _userManager.GetUserAsync(User);
            ConcreteConference newConcreteConference = new ConcreteConference();
            newConcreteConference.name = model.name;
            newConcreteConference.abstractConferenceId = model.abstractConferenceId;
            newConcreteConference.endtime = model.endtime;
            newConcreteConference.starttime = model.starttime;
            newConcreteConference.Maxassistants = model.Maxassistants;
            newConcreteConference.PhotoPath = uniqueFileName;
            newConcreteConference.centerId = model.centerId;
            newConcreteConference.Events = new List<Event> { };
            newConcreteConference.UserId = currentUser.Id;

            var successful = await _concreteConferenceService.Create(newConcreteConference);
            if (!successful)
            {
                return BadRequest("Could not add item.");
            }
            return RedirectToAction("Details", new { newConcreteConference.id });
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
            return RedirectToAction("Details", new { id = conferenceId });
        }

        [Authorize]
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

        [Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            var model = await _concreteConferenceService.Details(id);
            var abstractConferenceId = model.abstractConferenceId;
            var successful = await _concreteConferenceService.Delete(id);
            if (!successful)
            {
                return BadRequest("Could not delete item.");
            }
            return RedirectToAction("Details", "Conference", new { id = abstractConferenceId });
        }

        public async Task<IActionResult> ShowEvents(Guid id, string searchString, string eventTopic, string eventType)
        {
            var events = await _concreteConferenceService.ShowEvents(id);
            ViewData["concreteConferenceId"] = id;

            // Filtro por nombre

            ViewData["CurrentFilter"] = searchString;
            if (!String.IsNullOrEmpty(searchString))
            {
                events = (events.Where(item => item.name.ToLower().Contains(searchString.ToLower()))).Cast<Event>().ToArray();
            }

            // Filtro por tema

            IEnumerable<Topic> topicsEnumerable = await _TopicService.GetTopicsAsync();
            var topicsList = new SelectList(topicsEnumerable, "name", "name");
            if (!String.IsNullOrEmpty(eventTopic))
            {
                IEnumerable<Event> topicEvents = Enumerable.Empty<Event>();

                foreach (var actualEvent in events)
                {
                    switch (actualEvent.GetType().Name)
                    {
                        case "Chat":
                            Chat actualChat;
                            actualChat = (Chat)actualEvent;
                            var topicsChat = from et in actualChat.EventTopics select et.Topic; 
                            if (topicsChat != null && topicsChat.Any())
                            {
                                foreach (var topic in topicsChat)
                                {
                                    if (topic.name == eventTopic)
                                    {
                                        topicEvents = topicEvents.Append(actualEvent);
                                    }
                                }
                            }
                            break;

                        case "PracticalSession":
                            PracticalSession actualSession;
                            actualSession = (PracticalSession)actualEvent;
                            var topicsSession = from et in actualSession.EventTopics select et.Topic;
                            if (topicsSession != null && topicsSession.Any())
                            {
                                foreach (var topic in topicsSession)
                                {
                                    if (topic.name == eventTopic)
                                    {
                                        topicEvents = topicEvents.Append(actualEvent);
                                    }
                                }
                            }
                            break;

                        case "Talk":
                            Talk actualTalk;
                            actualTalk = (Talk)actualEvent;
                            var topicsTalk = from et in actualTalk.EventTopics select et.Topic;
                            if (topicsTalk != null && topicsTalk.Any())
                            {
                                foreach (var topic in topicsTalk)
                                {
                                    if (topic.name == eventTopic)
                                    {
                                        topicEvents = topicEvents.Append(actualEvent);
                                    }
                                }
                            }
                            break;
                    }
                }
                events = topicEvents.Cast<Event>().ToArray();
            }

            // Filtro por tipo de evento

            if (!String.IsNullOrEmpty(eventType))
            {
                IEnumerable<Event> typeEvents = Enumerable.Empty<Event>();

                foreach (var actualEvent in events)
                {
                    if (actualEvent.GetType().Name == eventType)
                    {
                        typeEvents = typeEvents.Append(actualEvent);
                    }
                }
                events = typeEvents.Cast<Event>().ToArray();
            }

            var typeTranslation = new Dictionary<String, String>
            {
                { "Chat", "Chat" },
                { "Talk", "Charla" },
                { "Party", "Fiesta" },
                { "PracticalSession", "Sesión Práctica" },
                { "Meal", "Comida" }
            };

            var model = new EventViewModel()
            {
                Events = events,
                TopicsList = topicsList,
                TypeTranslation = typeTranslation
            };
            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> NewConferenceFeedback(Guid ConcreteConferenceId)
        {
            ViewData["ConferenceId"] = await _concreteConferenceService.ObtainConference(ConcreteConferenceId);
            ViewData["ConcreteConferenceId"] = ConcreteConferenceId;
            return View();
        }

        [Authorize]
        public async Task<IActionResult> CreateConferenceFeedback(ConferenceFeedback feedback)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("NewConferenceFeedback", new
                {
                    ConcreteConferenceId = feedback.ConcreteConferenceId
                });
            }
            var currentUser = await _userManager.GetUserAsync(User);
            feedback.UserId = currentUser.Id;
            feedback.dateTime = DateTime.Now;
            var successful = await _concreteConferenceService.CreateConferenceFeedback(feedback);
            if (!successful)
            {
                return BadRequest("Could not add item.");
            }
            return RedirectToAction("Details", new { id = feedback.ConcreteConferenceId });
        }

        [Authorize]
        public async Task<IActionResult> SendNotification(Guid id, string mailBodyMessage)
        {
            var users = await _concreteConferenceService.GetUsersAsync(id);
            await _notificationService.SendConferenceNotification(users, mailBodyMessage);
            return RedirectToAction("Details", new { id });
        }
        
        public async Task<IActionResult> ViewFeedbacks(Guid id)
        {
            ViewData["Comments"] = await _concreteConferenceService.Comments(id);
            ViewData["concreteConference_id"] = id;
            return View();
        }

        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> VerifyNewConcreteConference(string name, Guid abstractConferenceId)
        {
            bool isNew = await _concreteConferenceService.VerifyNewConcreteConference(name, abstractConferenceId);
            if (!isNew)
            {
                return Json($"La instancia {name} ya existe.");
            }
            return Json(true);
        }

        [HttpGet]
        public async Task<JsonResult> GetQualities(Guid id)
        {
            var Qualities = new ConcreteFeedbackList { };
            var FoodQuality = new ConcreteFeedback
            {
                Quality = await _concreteConferenceService.FoodQuality(id),
                Label = "Comida"
            };
            var MusicQuality = new ConcreteFeedback
            {
                Quality = await _concreteConferenceService.MusicQuality(id),
                Label = "Musica"
            };
            var PlaceQuality = new ConcreteFeedback
            {
                Quality = await _concreteConferenceService.PlaceQuality(id),
                Label = "Lugar"
            };
            var DiscussionQuality = new ConcreteFeedback
            {
                Quality = await _concreteConferenceService.DiscussionQuality(id),
                Label = "Discusion"
            };
            var MaterialQuality = new ConcreteFeedback
            {
                Quality = await _concreteConferenceService.MaterialQuality(id),
                Label = "Material"
            };
            var ExpositorQuality = new ConcreteFeedback
            {
                Quality = await _concreteConferenceService.ExpositorQuality(id),
                Label = "Expositor"
            };
            Qualities.list.Add(FoodQuality);
            Qualities.list.Add(MusicQuality);
            Qualities.list.Add(PlaceQuality);
            Qualities.list.Add(DiscussionQuality);
            Qualities.list.Add(MaterialQuality);
            Qualities.list.Add(ExpositorQuality);

            return Json(Qualities);
        }
    }
}
