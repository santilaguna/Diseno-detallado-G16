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
    public class PartyController : Controller
    {
        // GET: /<controller>/
        private readonly IPartyService _PartyService;
        public PartyController(IPartyService partyservice)
        {
            _PartyService = partyservice;
        }


        // GET: /<controller>/
        public async Task<IActionResult> Index()
        {
            var parties = await _PartyService.GetPartiesAsync();
            var model = new PartyViewModel()
            {
                Parties = parties
            };
            return View(model);
        }
        public async Task<IActionResult> New()
        {
            var halls = await _PartyService.GetHalls();
            var model = new HallViewModel()
            {
                Halls = halls
            };

            return View(model);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var model = await _PartyService.Details(id);
            return View(model);
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Party newparty)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("New");
            }
            var successful = await _PartyService.Create(newparty);
            if (!successful)
            {
                return BadRequest("Could not add item.");
            }
            return RedirectToAction("Index");
        }
    }
}
