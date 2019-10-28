using System;

namespace Huihuinga.Models
{
    public class ApplicationUserConcreteConference
    {
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public Guid ConferenceId { get; set; }
        public ConcreteConference Conference { get; set; }
    }
}