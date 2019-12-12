using System;
using System.ComponentModel.DataAnnotations;

namespace Huihuinga.Models
{
    public class Conference
    {
        public Guid id { get; set; }
        [Required(ErrorMessage = "Name is required")]
        public string name { get; set; }
        public ConcreteConference Instance { get; set; }
        public string description { get; set; }
        public CalendarRepetition calendarRepetition { get; set; }
        public string PhotoPath { get; set; }

        public string UserId { get; set; }
    }
}
