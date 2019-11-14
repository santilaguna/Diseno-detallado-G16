using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Huihuinga.Models;
using Huihuinga.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Huihuinga.Controllers
{
    public class HallController : Controller
    {
        // GET: /<controller>/
        private readonly IHallService _HallService;
        public IHostingEnvironment HostingEnvironment { get; }
        public HallController(IHallService hallService, IHostingEnvironment hostingEnvironment)
        {
            _HallService = hallService;
            HostingEnvironment = hostingEnvironment;
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

        [Authorize]
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

        [Authorize]
        public async Task<IActionResult> Edit(Guid id)
        {
            var model = await _HallService.Details(id);
            return View(model);
        }

        public async Task<IActionResult> Update(Hall hall)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Edit", new { id = hall.id });
            }

            var successful = await _HallService.Edit(hall.id, hall.name, hall.capacity, hall.location, hall.projector, hall.plugs, hall.computers);
            if (!successful)
            {
                return BadRequest("Could not edit item.");
            }
            return RedirectToAction("Index", new { id = hall.EventCenterid });
        }

        [Authorize]
        public async Task<IActionResult> Delete(Guid id, Guid centerid)
        {
            var successful = await _HallService.Delete(id);
            if (!successful)
            {
                return BadRequest("Could not delete item.");
            }
            return RedirectToAction("Index", new { id = centerid });
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HallCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("New", new { id = model.EventCenterid });
            }

            string uniqueFileName = null;
            if (model.Photo != null)
            {
                string uploadsFolder = Path.Combine(HostingEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                model.Photo.CopyTo(new FileStream(filePath, FileMode.Create));
            }

            Hall newHall = new Hall();
            newHall.name = model.name;
            newHall.EventCenterid = model.EventCenterid;
            newHall.capacity = model.capacity;
            newHall.projector = model.projector;
            newHall.location = model.location;
            newHall.projector = model.projector;
            newHall.computers = model.computers;
            newHall.PhotoPath = uniqueFileName;

            var successful = await _HallService.Create(newHall);
            if (!successful)
            {
                return BadRequest("Could not add item.");
            }
            return RedirectToAction("Index", new { id = newHall.EventCenterid });
        }


    }
}
