using Huihuinga.Models;
using System;
using System.Threading.Tasks;

namespace Huihuinga.Services
{
    public interface ITopicService
    {
        Task<Topic[]> GetTopicsAsync();

        Task<bool> Create(Topic newTopic);

        Task<Topic> Details(Guid id);

        Task<bool> Edit(Guid id, string name, string description);

        Task<bool> Delete(Guid id);

        Task<bool> VerifyNewTopic(string topicName);
    }
}
