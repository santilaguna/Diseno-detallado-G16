using System;

namespace Huihuinga.Models
{
    public class ApplicationUserEvent
    {
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public Guid EventId { get; set; }
        public Event Event { get; set; }
    }
}