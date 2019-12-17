using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Huihuinga.Models
{
    public class ConferenceCreateViewModel
    {
        [Required(ErrorMessage = "Name is required")]
        [Remote(action: "VerifyNewConference", controller: "Conference", ErrorMessage = "Esta Conferencia ya existe")]
        public string name { get; set; }
        public string description { get; set; }
        public CalendarRepetition calendarRepetition { get; set; }
        public IFormFile Photo { get; set; }
    }
}
