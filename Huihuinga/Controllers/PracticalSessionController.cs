using System;
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
    public class PracticalSessionController : Controller, ITopicalController
    {
        // GET: /<controller>/
        private readonly IPracticalSessionService _PracticalService;
        public IHostingEnvironment HostingEnvironment { get; }
        public PracticalSessionController(IPracticalSessionService practicalservice, IHostingEnvironment hostingEnvironment)
        {
            _PracticalService = practicalservice;
            HostingEnvironment = hostingEnvironment;
        }


        // GET: /<controller>/
        public async Task<IActionResult> Index()
        {
            var practical = await _PracticalService.GetSessionsAsync();
            var model = new PracticalSessionViewModel()
            {
                PracticalSessions = practical
            };
            return View(model);
        }
        public async Task<IActionResult> New(Guid? id)
        {
            ViewData["concreteConferenceId"] = id;
            var halls = await _PracticalService.GetHalls();
            var model = new PracticalSessionCreateViewModel()
            {
                Halls = halls
            };

            return View(model);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var model = await _PracticalService.Details(id);
            return View(model);
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PracticalSessionCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("New");
            }

            string uniqueFileName = null;
            if (model.Photo != null)
            {
                string uploadsFolder = Path.Combine(HostingEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                model.Photo.CopyTo(new FileStream(filePath, FileMode.Create));
            }
            PracticalSession newsession = new PracticalSession();
            newsession.name = model.name;
            newsession.starttime = model.starttime;
            newsession.endtime = model.endtime;
            newsession.PhotoPath = uniqueFileName;
            newsession.Hallid = model.Hallid;
            newsession.concreteConferenceId = model.concreteConferenceId;

            var successful = await _PracticalService.Create(newsession);
            if (!successful)
            {
                return BadRequest("Could not add item.");
            }
            return RedirectToAction("Details", new { newsession.id });
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var model = await _PracticalService.Details(id);
            return View(model);
        }

        public async Task<IActionResult> Update(PracticalSession session)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Edit", new { id = session.id });
            }

            var successful = await _PracticalService.Edit(session.id, session.name, session.starttime, session.endtime, session.Hallid);
            if (!successful)
            {
                return BadRequest("Could not edit item.");
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var successful = await _PracticalService.Delete(id);
            if (!successful)
            {
                return BadRequest("Could not delete item.");
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> NewTopic(Guid id)
        {
            ViewData["event_id"] = id;
            var topics = await _PracticalService.NewTopic(id);
            var model = new TopicViewModel()
            {
                Topics = topics
            };
            return View(model);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddNewTopic(Guid id, Topic topic)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("NewTopic", new { id });
            }
            var successful = await _PracticalService.AddNewTopic(id, topic);
            if (!successful)
            {
                return BadRequest("Could not add item.");
            }
            return RedirectToAction("Details", new { id });
        }

        public async Task<IActionResult> AddTopic(Guid id, Guid topicId)
        {
            var successful = await _PracticalService.AddTopic(id, topicId);
            if (!successful)
            {
                return BadRequest("Could not add item.");
            }
            return RedirectToAction("Details", new { id });
        }
    }
}
