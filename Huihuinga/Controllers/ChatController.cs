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
    public class ChatController : Controller
    {
        // GET: /<controller>/
        // GET: /<controller>/
        private readonly IChatService _ChatService;
        public IHostingEnvironment HostingEnvironment { get; }
        public ChatController(IChatService chatservice, IHostingEnvironment hostingEnvironment)
        {
            _ChatService = chatservice;
            HostingEnvironment = hostingEnvironment;
        }


        // GET: /<controller>/
        public async Task<IActionResult> Index()
        {
            var chats = await _ChatService.GetChatsAsync();
            var model = new ChatViewModel()
            {
                Chats = chats
            };
            return View(model);
        }
        public async Task<IActionResult> New()
        {
            var halls = await _ChatService.GetHalls();
            var model = new ChatCreateViewModel()
            {
                Halls = halls
            };

            return View(model);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var model = await _ChatService.Details(id);
            return View(model);
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ChatCreateViewModel model)
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
            Chat newchat = new Chat();
            newchat.name = model.name;
            newchat.starttime = model.starttime;
            newchat.endtime = model.endtime;
            newchat.PhotoPath = uniqueFileName;
            newchat.Hallid = model.Hallid;

            var successful = await _ChatService.Create(newchat);
            if (!successful)
            {
                return BadRequest("Could not add item.");
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var model = await _ChatService.Details(id);
            return View(model);
        }

        public async Task<IActionResult> Update(Meal meal)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Edit", new { id = meal.id });
            }

            var successful = await _ChatService.Edit(meal.id, meal.name, meal.starttime, meal.endtime, meal.Hallid);
            if (!successful)
            {
                return BadRequest("Could not edit item.");
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var successful = await _ChatService.Delete(id);
            if (!successful)
            {
                return BadRequest("Could not delete item.");
            }
            return RedirectToAction("Index");
        }
    }
}
