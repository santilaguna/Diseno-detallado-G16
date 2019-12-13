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
    public class ChatController : Controller, ITopicalController
    {
        // GET: /<controller>/
        // GET: /<controller>/
        private readonly IChatService _ChatService;
        public IHostingEnvironment HostingEnvironment { get; }
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEventService _eventService;
        public ChatController(IChatService chatservice, IHostingEnvironment hostingEnvironment,
                              UserManager<ApplicationUser> userManager, IEventService eventService)
        {
            _ChatService = chatservice;
            HostingEnvironment = hostingEnvironment;
            _userManager = userManager;
            _eventService = eventService;
        }


        // GET: /<controller>/
        public async Task<IActionResult> Index()
        {
            var chats = await _ChatService.GetChatsAsync();
            var model = new ChatViewModel()
            {
                Chats = chats
            };
            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> New(Guid? id)
        {
            ViewData["concreteConferenceId"] = id;
            var halls = await _ChatService.GetHalls(id);
            var users = await _eventService.GetAllUsers();
            var model = new ChatCreateViewModel()
            {
                Users = users,
                Halls = halls
            };

            return View(model);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            string UserId = "";
            ViewData["currentUser"] = false;
            if (currentUser != null)
            {
                UserId = currentUser.Id;
                ViewData["currentUser"] = true;
            }
            var authorized = await _ChatService.CheckUser(id, UserId);
            ViewData["owner"] = authorized;
            var model = await _ChatService.Details(id);
            
            var eventLimit = await _eventService.CheckLimitUsers(model);
            var maxAssistants = await _eventService.GetMaxAssistants(model.Hallid);
            ViewData["maxAssistants"] = maxAssistants;
            var actualUsers = await _eventService.GetActualUsers(model);
            ViewData["availableSpace"] = maxAssistants - actualUsers;

            var expositors = await _ChatService.GetExpositors(model.ExpositorsId);
            ViewData["expositors"] = expositors;

            ViewData["can_feedback"] = false;
            if (model.concreteConferenceId != null)
            {
                ViewData["can_feedback"] = await _ChatService.CanFeedback(currentUser.Id, id);
            }

            ViewData["finished"] = false;
            if (model.endtime < DateTime.Now)
            {
                ViewData["finished"] = true;
            }


            var moderator = await _eventService.GetUserName(model.ModeratorId);
            ViewData["moderator"] = moderator;
            ViewData["moderator permission"] = false;
            if (currentUser != null && currentUser.Id == model.ModeratorId)
            {
                ViewData["moderator permission"] = true;
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
        public async Task<IActionResult> Create(ChatCreateViewModel model)
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
            Chat newchat = new Chat();
            newchat.name = model.name;
            newchat.starttime = model.starttime;
            newchat.endtime = model.endtime;
            newchat.PhotoPath = uniqueFileName;
            newchat.Hallid = model.Hallid;
            newchat.concreteConferenceId = model.concreteConferenceId;
            newchat.UserId = currentUser.Id;
            newchat.ModeratorId = model.ModeratorId;
            newchat.ExpositorsId = new List<string> { };
            newchat.feedbacks = new List<Feedback> { };

            var successful = await _ChatService.Create(newchat);
            if (!successful)
            {
                return BadRequest("Could not add item.");
            }
            return RedirectToAction("Details", new { newchat.id });
        }

        [Authorize]
        public async Task<IActionResult> Edit(Guid id)
        {
            var model = await _ChatService.Details(id);
            return View(model);
        }

        public async Task<IActionResult> Update(Chat chat)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Edit", new { chat.id });
            }

            if (chat.starttime >= chat.endtime)
            {
                return RedirectToAction("Edit", new { id = chat.id });
            }

            var successful = await _ChatService.Edit(chat.id, chat.name, chat.starttime, chat.endtime, chat.Hallid);
            if (!successful)
            {
                return BadRequest("Could not edit item.");
            }
            return RedirectToAction("Index");
        }

        [Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            var model = await _ChatService.Details(id);
            var concreteConferenceId = model.concreteConferenceId;
            var successful = await _ChatService.Delete(id);
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
            var topics = await _ChatService.NewTopic(id);
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
            var successful = await _ChatService.AddNewTopic(id, topic);
            if (!successful)
            {
                return BadRequest("Could not add item.");
            }
            return RedirectToAction("Details", new { id });
        }

        public async Task<IActionResult> AddTopic(Guid id, Guid topicId)
        {
            var successful = await _ChatService.AddTopic(id, topicId);
            if (!successful)
            {
                return BadRequest("Could not add item.");
            }
            return RedirectToAction("Details", new { id });
        }
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
        public async Task<IActionResult> NewExpositor(Guid id)
        {
            var users = await _eventService.GetAllUsers();
            var model = new ExpositorToChatCreateViewModel()
            {
                event_id = id,
                Users = users
            };

            return View(model);
        }

        public async Task<IActionResult> AddExpositor(ExpositorToChatCreateViewModel expositor)
        {
            var successful = await _ChatService.AddExpositor(expositor.ExpositorId, expositor.event_id);
            if (!successful)
            {
                return BadRequest("Could not add item.");
            }
            return RedirectToAction("Details", new { id = expositor.event_id });

        }

        public async Task<IActionResult> DeleteExpositor(string expositormail, Guid eventid)
        {
            var successful = await _ChatService.DeleteExpositor(expositormail, eventid);
            if (!successful)
            {
                return BadRequest("Could not delete item.");
            }
            return RedirectToAction("Details", new { id = eventid });
        }

        [Authorize]
        public async Task<IActionResult> PendingFeedbacks()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var chats = await _ChatService.GetChatsWithPendingFeedbacks(currentUser.Id);
            var model = new ChatViewModel()
            {
                Chats = chats
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
            var successful = await _ChatService.CreateFeedback(feedback, feedback.EventId);
            if (!successful)
            {
                return BadRequest("Could not add item.");
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> FinishedChats()
        {
            var chats = await _ChatService.GetFinishedChats();
            var model = new ChatViewModel()
            {
                Chats = chats
            };
            return View(model);
        }

        public async Task<IActionResult> ViewFeedbacks(Guid eventId)
        {
            ViewData["PlaceQuality"] = await _ChatService.PlaceQuality(eventId);
            ViewData["DiscussionQuality"] = await _ChatService.DiscussionQuality(eventId);
            ViewData["Comments"] = await _ChatService.Comments(eventId);
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
                return RedirectToAction("NewConferenceFeedback", new { eventid = feedback.EventId ,
                                                                       ConcreteConferenceId = feedback.ConcreteConferenceId});
            }
            var currentUser = await _userManager.GetUserAsync(User);
            feedback.UserId = currentUser.Id;
            feedback.dateTime = DateTime.Now;
            var successful = await _eventService.CreateConferenceFeedback(feedback);
            if (!successful)
            {
                return BadRequest("Could not add item.");
            }
            return RedirectToAction("Details", new {id = feedback.EventId });
        }

    }
}
