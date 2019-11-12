using System;
using System.Linq;
using System.Threading.Tasks;
using Huihuinga.Models;
using Huihuinga.Services;
using Microsoft.AspNetCore.Mvc;

namespace Huihuinga.Controllers
{
    public class TopicController : Controller
    {

        private readonly ITopicService _topicService;
        public TopicController(ITopicService topicService)
        {
            _topicService = topicService;
        }


        // GET: /<controller>/
        public async Task<IActionResult> Index()
        {
            var topics = await _topicService.GetTopicsAsync();
            var model = new TopicViewModel()
            {
                Topics = topics
            };
            return View(model);
        }

        public IActionResult New()
        {
            return View();
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var model = await _topicService.Details(id);
            return View(model);
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var model = await _topicService.Details(id);
            return View(model);
        }

        public async Task<IActionResult> Update(Topic topic)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Edit", new { topic.id });
            }

            var successful = await _topicService.Edit(topic.id, topic.name, topic.description);
            if (!successful)
            {
                return BadRequest("Could not edit item.");
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var successful = await _topicService.Delete(id);
            if (!successful)
            {
                return BadRequest("Could not delete item.");
            }
            return RedirectToAction("Index");
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Topic newTopic)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("New");
            }
            var successful = await _topicService.Create(newTopic);
            if (!successful)
            {
                return BadRequest("Could not add item.");
            }
            return RedirectToAction("Index");
        }

        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> VerifyNewTopic(string name)
        {
            bool isNew = await _topicService.VerifyNewTopic(name);
            if (!isNew)
            {
                return Json($"El tema {name} ya existe.");
            }
            return Json(true);
        }
    }
}
