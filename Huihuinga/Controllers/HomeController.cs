using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
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
        public IHostingEnvironment HostingEnvironment { get; }

        public HomeController(
            IChatService chatservice,
            IMealService mealService,
            IPartyService partyservice,
            IPracticalSessionService practicalservice,
            ITalkService talkservice,
            IHostingEnvironment hostingEnvironment)
        {
            _ChatService = chatservice;
            _MealService = mealService;
            _PartyService = partyservice;
            _PracticalService = practicalservice;
            _TalkService = talkservice;
            HostingEnvironment = hostingEnvironment;
        }


        public async Task<IActionResult> Index(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;

            IEnumerable<Chat> chats = await _ChatService.GetChatsAsync();
            IEnumerable<Meal> meals = await _MealService.GetMealsAsync();
            IEnumerable<Party> parties = await _PartyService.GetPartiesAsync();
            IEnumerable<PracticalSession> practical = await _PracticalService.GetSessionsAsync();
            IEnumerable<Talk> talks = await _TalkService.GetTalksAsync();

            if (!String.IsNullOrEmpty(searchString))
            {
                chats = chats.Where(item => item.name.Contains(searchString));
                meals = meals.Where(item => item.name.Contains(searchString));
                parties = parties.Where(item => item.name.Contains(searchString));
                practical = practical.Where(item => item.name.Contains(searchString));
                talks = talks.Where(item => item.name.Contains(searchString));
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
