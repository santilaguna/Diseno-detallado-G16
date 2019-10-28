﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Huihuinga.Models;
using Huihuinga.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Huihuinga.Controllers
{
    public class EventCenterController : Controller
    {

        private readonly IEventCenterService _eventCenterService;

        public IHostingEnvironment HostingEnvironment { get; }

        public EventCenterController(IEventCenterService eventCenterService, IHostingEnvironment hostingEnvironment)
        {
            _eventCenterService = eventCenterService;
            HostingEnvironment = hostingEnvironment;
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

        public async Task<IActionResult> Edit(Guid id)
        {
            var model = await _eventCenterService.Details(id);
            return View(model);
        }

        public async Task<IActionResult> Update(EventCenter eventCenter)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Edit", new { id = eventCenter.id });
            }

            var successful = await _eventCenterService.Edit(eventCenter.id, eventCenter.name, eventCenter.address);
            if (!successful)
            {
                return BadRequest("Could not edit item.");
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var successful = await _eventCenterService.Delete(id);
            if (!successful)
            {
                return BadRequest("Could not delete item.");
            }
            return RedirectToAction("Index");
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EventCenterCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("New");
            }

            string uniqueFileName = null;
            if(model.Photo != null)
            {
                string uploadsFolder = Path.Combine(HostingEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                model.Photo.CopyTo(new FileStream(filePath, FileMode.Create));
            }
            EventCenter newEventCenter = new EventCenter();
            newEventCenter.name = model.name;
            newEventCenter.address = model.address;
            newEventCenter.PhotoPath = uniqueFileName;

            var successful = await _eventCenterService.Create(newEventCenter);
            if (!successful)
            {
                return BadRequest("Could not add item.");
            }
            return RedirectToAction("Index");
        }

    }
}
