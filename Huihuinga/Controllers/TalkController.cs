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
    public class TalkController : Controller
    {
        // GET: /<controller>/
        private readonly ITalkService _TalkService;
        public IHostingEnvironment HostingEnvironment { get; }
        public TalkController(ITalkService talkservice, IHostingEnvironment hostingEnvironment)
        {
            _TalkService = talkservice;
            HostingEnvironment = hostingEnvironment;
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
            var model = new TalkCreateViewModel()
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
        public async Task<IActionResult> Create(TalkCreateViewModel model)
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
            Talk newtalk = new Talk();
            newtalk.name = model.name;
            newtalk.starttime = model.starttime;
            newtalk.endtime = model.endtime;
            newtalk.PhotoPath = uniqueFileName;
            newtalk.Hallid = model.Hallid;
            newtalk.description = model.description;

            var successful = await _TalkService.Create(newtalk);
            if (!successful)
            {
                return BadRequest("Could not add item.");
            }
            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Edit(Guid id)
        {
            var model = await _TalkService.Details(id);
            return View(model);
        }

        public async Task<IActionResult> Update(Talk talk)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Edit", new { id = talk.id });
            }

            var successful = await _TalkService.Edit(talk.id, talk.name, talk.starttime, talk.endtime, talk.Hallid, talk.description);
            if (!successful)
            {
                return BadRequest("Could not edit item.");
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var successful = await _TalkService.Delete(id);
            if (!successful)
            {
                return BadRequest("Could not delete item.");
            }
            return RedirectToAction("Index");
        }
    }
}
