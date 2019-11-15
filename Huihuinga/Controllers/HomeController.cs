using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Huihuinga.Models;
using Huihuinga.Services;

namespace Huihuinga.Controllers
{
    public class HomeController : Controller
    {
        private readonly IChatService _ChatService;
        private readonly IMealService _MealService;
        private readonly IPartyService _PartyService;
        private readonly IPracticalSessionService _PracticalService;
        private readonly ITalkService _TalkService;
        private readonly ITopicService _TopicService;
        public IHostingEnvironment HostingEnvironment { get; }

        public HomeController(
            IChatService chatservice,
            IMealService mealService,
            IPartyService partyservice,
            IPracticalSessionService practicalservice,
            ITalkService talkservice,
            ITopicService topicService,
            IHostingEnvironment hostingEnvironment)
        {
            _ChatService = chatservice;
            _MealService = mealService;
            _PartyService = partyservice;
            _PracticalService = practicalservice;
            _TalkService = talkservice;
            _TopicService = topicService;
            HostingEnvironment = hostingEnvironment;
        }


        public async Task<IActionResult> Index(string searchString, string eventTopic)
        {
            ViewData["CurrentFilter"] = searchString;

            IEnumerable<Chat> chats = await _ChatService.GetAllChatsAsync();
            IEnumerable<Meal> meals = await _MealService.GetAllMealsAsync();
            IEnumerable<Party> parties = await _PartyService.GetAllPartiesAsync();
            IEnumerable<PracticalSession> practical = await _PracticalService.GetAllSessionsAsync();
            IEnumerable<Talk> talks = await _TalkService.GetTalksAsync();
            IEnumerable<Topic> topicsEnumerable = await _TopicService.GetTopicsAsync();

            var topicsList = new SelectList(topicsEnumerable, "name", "name");

            if (!String.IsNullOrEmpty(searchString))
            {
                chats = chats.Where(item => item.name.ToLower().Contains(searchString.ToLower()));
                meals = meals.Where(item => item.name.ToLower().Contains(searchString.ToLower()));
                parties = parties.Where(item => item.name.ToLower().Contains(searchString.ToLower()));
                practical = practical.Where(item => item.name.ToLower().Contains(searchString.ToLower()));
                talks = talks.Where(item => item.name.ToLower().Contains(searchString.ToLower()));
            }
            if (!String.IsNullOrEmpty(eventTopic))
            {

                IEnumerable<Chat> topicChats = Enumerable.Empty<Chat>();
                IEnumerable<PracticalSession> topicPractical = Enumerable.Empty<PracticalSession>();
                IEnumerable<Talk> topicTalks = Enumerable.Empty<Talk>();

                foreach (var chat in chats)
                {
                    var topicsChat = chat.Topics;
                    if (topicsChat != null && topicsChat.Any())
                    {
                        foreach (var topic in topicsChat)
                        {
                            if (topic.name == eventTopic)
                            {
                                topicChats = topicChats.Append(chat);
                            }
                        }
                    }
                }
                foreach (var session in practical)
                {
                    var topicsPractical = session.Topics;
                    if (topicsPractical != null && topicsPractical.Any())
                    {
                        foreach (var topic in topicsPractical)
                        {
                            if (topic.name == eventTopic)
                            {
                                topicPractical = topicPractical.Append(session);
                            }
                        }
                    }
                }
                foreach (var talk in talks)
                {
                    var topicsTalk = talk.Topics;
                    if (topicsTalk != null && topicsTalk.Any())
                    {
                        foreach (var topic in topicsTalk)
                        {
                            if (topic.name == eventTopic)
                            {
                                topicTalks = topicTalks.Append(talk);
                            }
                        }
                    }
                }

                chats = topicChats;
                meals = null;
                parties = null;
                practical = topicPractical;
                talks = topicTalks;
            }

            bool show_chats;
            if (chats == null || !chats.Any())
            {
                show_chats = false;
            }
            else
            {
                show_chats = true;
            }
            bool show_meals;
            if (meals == null || !meals.Any())
            {
                show_meals = false;
            }
            else
            {
                show_meals = true;
            }
            bool show_parties;
            if (parties == null || !parties.Any())
            {
                show_parties = false;
            }
            else
            {
                show_parties = true;
            }
            bool show_practical;
            if (practical == null || !practical.Any())
            {
                show_practical = false;
            }
            else
            {
                show_practical = true;
            }
            bool show_talks;
            if (talks == null || !talks.Any())
            {
                show_talks = false;
            }
            else
            {
                show_talks = true;
            }

            var model = new AllEventsViewModel()
            {
                Chats = chats,
                Meals = meals,
                Parties = parties,
                PracticalSessions = practical,
                Talks = talks,
                Show_chats = show_chats,
                Show_meals = show_meals,
                Show_parties = show_parties,
                Show_practical = show_practical,
                Show_talks = show_talks,
                TopicsList = topicsList,
            };
            return View(model);
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
