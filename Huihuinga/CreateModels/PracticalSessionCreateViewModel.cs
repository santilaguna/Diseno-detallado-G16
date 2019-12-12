using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Huihuinga.Models
{
    public class PracticalSessionCreateViewModel: EventCreateViewModel
    {
        public Hall[] Halls { get; set; }

        public ApplicationUser[] Users { get; set; }

        [Required]
        public string ExpositorId { get; set; }

    }
}
