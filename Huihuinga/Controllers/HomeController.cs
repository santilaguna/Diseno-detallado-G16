﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Huihuinga.Models;
using Huihuinga.Services;
using Newtonsoft.Json;

namespace Huihuinga.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEventService _eventService;
        private readonly ITopicService _TopicService;
        public IHostingEnvironment HostingEnvironment { get; }

        public HomeController(
            IEventService eventservice,
            ITopicService topicService,
            IHostingEnvironment hostingEnvironment)
        {
            _eventService = eventservice;
            _TopicService = topicService;
            HostingEnvironment = hostingEnvironment;
        }


        public async Task<IActionResult> Index(string searchString, string eventTopic, string eventType)
        {
            var events = await _eventService.GetAllEvents();

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

        [HttpGet]
        public async Task<IActionResult> GetCalendarEvents()
        {
            // calendar test
            var events_models = await _eventService.GetAllEventsHome();
            var events_ = new List<Dictionary<string, string>>();
            foreach (var e in events_models)
            {
                events_.Add(
                    new Dictionary<string, string>()
                    {
                        { "title", e.name},
                        { "url", $"{e.GetType().Name}/Details/{e.id.ToString()}" },
                        { "start", e.starttime.ToString("yyyy-MM-ddTHH:mm:ss") },
                        { "end", e.endtime.ToString("yyyy-MM-ddTHH:mm:ss") }
                    }
                    );
            }
            var rows = events_.ToArray();
            //var json = new JsonResult(rows);
            //ViewData["home_events"] = json;
            //ViewData["home_events"] = JsonConvert.SerializeObject(rows, Formatting.Indented, new JsonSerializerSettings()
            //{ ReferenceLoopHandling = ReferenceLoopHandling.Ignore }
            //);
            return Json(rows);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
