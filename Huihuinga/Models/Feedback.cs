using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Huihuinga.Models
{
    public class Feedback
    {
        public Guid id { get; set; }
        public string UserId { get; set; }
        public Guid EventId { get; set; }
        public Event Event { get; set; }
        public DateTime dateTime { get; set; }
        public string comment { get; set; }
        public int FoodQuality { get; set; }
        public int MusicQuality { get; set; }
        public int DiscussionQuality {get; set; }
        public int MaterialQuality { get; set; }
        public int PlaceQuality { get; set; }
        public int ExpositorQuality { get; set; }

    }
}
