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
    public class ChatController : Controller
    {
        // GET: /<controller>/
        // GET: /<controller>/
        private readonly IChatService _ChatService;
        public ChatController(IChatService chatservice)
        {
            _ChatService = chatservice;
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
            var model = new HallViewModel()
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
        public async Task<IActionResult> Create(Chat newchat)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("New");
            }
            var successful = await _ChatService.Create(newchat);
            if (!successful)
            {
                return BadRequest("Could not add item.");
            }
            return RedirectToAction("Index");
        }
    }
}
