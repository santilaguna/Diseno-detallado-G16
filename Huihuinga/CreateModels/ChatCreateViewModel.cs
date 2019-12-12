using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Huihuinga.Models
{
    public class ChatCreateViewModel: EventCreateViewModel
    {
        public Hall[] Halls { get; set; }

        [Required]
        public string ModeratorId { get; set; }

        public ApplicationUser[] Users { get; set; }
    }
}
