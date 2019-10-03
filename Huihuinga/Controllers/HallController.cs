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
    public class HallController : Controller
    {
        // GET: /<controller>/
        private readonly IHallService _HallService;
        public HallController(IHallService hallService)
        {
            _HallService = hallService;
        }


        // GET: /<controller>/
        public async Task<IActionResult> Index(Guid id)
        {
            var halls = await _HallService.GetHallsAsync(id);
            ViewData["eventcenterid"] = id;
            var model = new HallViewModel()
            {
                Halls = halls
            };
            return View(model);
        }
        public IActionResult New(Guid id)
        {
            ViewData["centerid"] = id;

            return View();
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var model = await _HallService.Details(id);
            return View(model);
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Hall newHall)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("New", new { id = newHall.EventCenterid });
            }
            var successful = await _HallService.Create(newHall);
            if (!successful)
            {
                return BadRequest("Could not add item.");
            }
            return RedirectToAction("Index", new { id = newHall.EventCenterid });
        }


    }
}
