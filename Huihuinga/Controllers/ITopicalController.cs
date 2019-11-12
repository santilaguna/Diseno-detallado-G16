using Huihuinga.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Huihuinga.Controllers
{
    public interface ITopicalController
    {
        Task<IActionResult> NewTopic(Guid id);

        Task<IActionResult> AddNewTopic(Guid id, Topic topic);

        Task<IActionResult> AddTopic(Guid id, Guid topicId);
    }
}
