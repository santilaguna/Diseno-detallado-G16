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
    public class ConcreteConferenceController : Controller
    {

        private readonly IConcreteConferenceService _concreteConferenceService;
        public ConcreteConferenceController(IConcreteConferenceService concreteConferenceService)
        {
            _concreteConferenceService = concreteConferenceService;
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

        public IActionResult New()
        {
            return View();
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var model = await _concreteConferenceService.Details(id);
            return View(model);
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ConcreteConference newConcreteConference)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("New");
            }
            var successful = await _concreteConferenceService.Create(newConcreteConference);
            if (!successful)
            {
                return BadRequest("Could not add item.");
            }
            return RedirectToAction("Index");
        }

    }
}