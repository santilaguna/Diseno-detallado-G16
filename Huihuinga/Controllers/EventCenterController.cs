using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Huihuinga.Models;
using Huihuinga.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Huihuinga.Controllers
{
    public class EventCenterController : Controller
    {

        private readonly IEventCenterService _eventCenterService;
        public EventCenterController(IEventCenterService eventCenterService)
        {
            _eventCenterService = eventCenterService;
        }


        // GET: /<controller>/
        public async Task<IActionResult> Index()
        {
            var centers = await _eventCenterService.GetEventCentersAsync();
            var model = new EventCenterViewModel()
            {
                EventCenters = centers
            };
            return View(model);
        }

        public IActionResult New()
        {
            return View();
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var model = await _eventCenterService.Details(id);
            return View(model);
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EventCenter newEventCenter)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("New");
            }
            var successful = await _eventCenterService.Create(newEventCenter);
            if (!successful)
            {
                return BadRequest("Could not add item.");
            }
            return RedirectToAction("Index");
        }

    }
}
