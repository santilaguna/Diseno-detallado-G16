using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Huihuinga.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<ApplicationUserConcreteConference> UsersConferences { get; set; }
        public ICollection<ApplicationUserEvent> UsersEvents { get; set; }

        [PersonalData, Required, StringLength(20)]
        public string FirstName { get; set; }

        [PersonalData, Required, StringLength(20)]
        public string LastName { get; set; }

        public string FullName { get { return $"{FirstName} {LastName}"; } }
    }
}