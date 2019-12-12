using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Huihuinga.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<ApplicationUserConcreteConference> UsersConferences { get; set; }
        public ICollection<ApplicationUserEvent> UsersEvents { get; set; }
    }
}