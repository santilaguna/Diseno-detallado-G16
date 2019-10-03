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
    public class TalkController : Controller
    {
        // GET: /<controller>/
        private readonly ITalkService _TalkService;
        public TalkController(ITalkService talkservice)
        {
            _TalkService = talkservice;
        }


        // GET: /<controller>/
        public async Task<IActionResult> Index()
        {
            var talks = await _TalkService.GetTalksAsync();
            var model = new TalkViewModel()
            {
                Talks = talks
            };
            return View(model);
        }
        public async Task<IActionResult> New()
        {
            var halls = await _TalkService.GetHalls();
            var model = new HallViewModel()
            {
                Halls = halls
            };

            return View(model);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var model = await _TalkService.Details(id);
            return View(model);
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Talk newtalk)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("New");
            }
            var successful = await _TalkService.Create(newtalk);
            if (!successful)
            {
                return BadRequest("Could not add item.");
            }
            return RedirectToAction("Index");
        }
    }
}
