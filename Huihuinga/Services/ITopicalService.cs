using Huihuinga.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Huihuinga.Services
{
    public interface ITopicalService
    {
        Task<Topic[]> NewTopic(Guid id);
        Task<bool> AddNewTopic(Guid id, Topic newTopic);
        Task<bool> AddTopic(Guid id, Guid topicId);
    }
}
