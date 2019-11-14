﻿using System;
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
    public class ChatController : Controller, ITopicalController
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

        [Authorize]
        public async Task<IActionResult> New(Guid? id)
        {
            ViewData["concreteConferenceId"] = id;
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
            Chat newchat = new Chat();
            newchat.name = model.name;
            newchat.starttime = model.starttime;
            newchat.endtime = model.endtime;
            newchat.PhotoPath = uniqueFileName;
            newchat.Hallid = model.Hallid;
            newchat.concreteConferenceId = model.concreteConferenceId;

            var successful = await _ChatService.Create(newchat);
            if (!successful)
            {
                return BadRequest("Could not add item.");
            }
            return RedirectToAction("Details", new { newchat.id });
        }

        [Authorize]
        public async Task<IActionResult> Edit(Guid id)
        {
            var model = await _ChatService.Details(id);
            return View(model);
        }

        public async Task<IActionResult> Update(Chat chat)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Edit", new { chat.id });
            }

            if (chat.starttime >= chat.endtime)
            {
                return RedirectToAction("Edit", new { id = chat.id });
            }

            var successful = await _ChatService.Edit(chat.id, chat.name, chat.starttime, chat.endtime, chat.Hallid);
            if (!successful)
            {
                return BadRequest("Could not edit item.");
            }
            return RedirectToAction("Index");
        }

        [Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            var successful = await _ChatService.Delete(id);
            if (!successful)
            {
                return BadRequest("Could not delete item.");
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> NewTopic(Guid id)
        {
            ViewData["event_id"] = id;
            var topics = await _ChatService.NewTopic(id);
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
            var successful = await _ChatService.AddNewTopic(id, topic);
            if (!successful)
            {
                return BadRequest("Could not add item.");
            }
            return RedirectToAction("Details", new { id });
        }

        public async Task<IActionResult> AddTopic(Guid id, Guid topicId)
        {
            var successful = await _ChatService.AddTopic(id, topicId);
            if (!successful)
            {
                return BadRequest("Could not add item.");
            }
            return RedirectToAction("Details", new { id });
        }
    }
}
