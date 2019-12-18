using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Huihuinga.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Huihuinga.Controllers
{
    public class EventController : Controller
    {

        private readonly IEventService _eventService;
        public EventController(IEventService eventService)
        {
            _eventService = eventService;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult NewEvent(Guid id, string eventType)
        {
            return RedirectToAction("New", eventType, new { id });
        }

        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> VerifyNewEvent(string name, Guid? concreteConferenceId)
        {
            bool isNew = await _eventService.VerifyNewEvent(name, concreteConferenceId);
            if (!isNew)
            {
                return Json($"El Evento {name} ya existe.");
            }
            return Json(true);
        }


        [AcceptVerbs("Get", "Post")]
        public IActionResult VerifyStartTime(DateTime starttime)
        {
            bool isNew = starttime > DateTime.Now;
            if (!isNew)
            {
                return Json($"El evento debe empezar en una fecha posterior");
            }
            return Json(true);
        }

    }
}
