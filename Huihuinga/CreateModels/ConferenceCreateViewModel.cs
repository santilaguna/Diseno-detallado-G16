using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Huihuinga.Models
{
    public class ConferenceCreateViewModel
    {
        [Required(ErrorMessage = "Name is required")]
        public string name { get; set; }
        public string description { get; set; }
        public CalendarRepetition calendarRepetition { get; set; }
        public IFormFile Photo { get; set; }
    }
}
