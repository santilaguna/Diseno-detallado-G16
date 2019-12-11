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
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity;

namespace Huihuinga.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IEventService _eventService;
        private readonly UserManager<ApplicationUser> _userManager;
        public IHostingEnvironment HostingEnvironment { get; }

        public ProfileController(IEventService eventservice, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager; 
            _eventService = eventservice;
        }

        [HttpGet]
        public async Task<IActionResult> GetCalendarEvents()
        {
            var user = await _userManager.GetUserAsync(User);
            // calendar test
            var events_models = await _eventService.GetAllEventsProfile(user.Id);
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
    }
}
