using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Huihuinga.Models;
using Huihuinga.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Huihuinga.Controllers
{
    public class PartyController : Controller
    {
        // GET: /<controller>/
        private readonly IPartyService _PartyService;
        public IHostingEnvironment HostingEnvironment { get; }
        private readonly UserManager<ApplicationUser> _userManager;
        public PartyController(IPartyService partyservice, IHostingEnvironment hostingEnvironment,
                               UserManager<ApplicationUser> userManager)
        {
            _PartyService = partyservice;
            HostingEnvironment = hostingEnvironment;
            _userManager = userManager;
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

        [Authorize]
        public async Task<IActionResult> New(Guid? id)
        {
            ViewData["concreteConferenceId"] = id;
            var halls = await _PartyService.GetHalls();
            var model = new PartyCreateViewModel()
            {
                Halls = halls
            };

            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> Edit(Guid id)
        {
            var model = await _PartyService.Details(id);
            return View(model);
        }

        public async Task<IActionResult> Update(Party party)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Edit", new { id = party.id });
            }

            if (party.starttime >= party.endtime)
            {
                return RedirectToAction("Edit", new { id = party.id });
            }

            var successful = await _PartyService.Edit(party.id, party.name, party.starttime, party.endtime, party.Hallid, party.description);
            if (!successful)
            {
                return BadRequest("Could not edit item.");
            }
            return RedirectToAction("Index");
        }

        [Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            var successful = await _PartyService.Delete(id);
            if (!successful)
            {
                return BadRequest("Could not delete item.");
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            string UserId = "";
            if (currentUser != null)
            {
                UserId = currentUser.Id;
            }
            var authorized = await _PartyService.CheckUser(id, UserId);
            ViewData["owner"] = authorized;
            var model = await _PartyService.Details(id);
            return View(model);
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PartyCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("New");
            }

            if (model.starttime >= model.endtime)
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
            var currentUser = await _userManager.GetUserAsync(User);
            Party newparty = new Party();
            newparty.name = model.name;
            newparty.starttime = model.starttime;
            newparty.endtime = model.endtime;
            newparty.PhotoPath = uniqueFileName;
            newparty.Hallid = model.Hallid;
            newparty.description = model.description;
            newparty.concreteConferenceId = model.concreteConferenceId;
            newparty.UserId = currentUser.Id;

            var successful = await _PartyService.Create(newparty);
            if (!successful)
            {
                return BadRequest("Could not add item.");
            }
            return RedirectToAction("Details", new { newparty.id });
        }
    }
}
