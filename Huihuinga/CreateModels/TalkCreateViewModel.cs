using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Huihuinga.Models
{
    public class TalkCreateViewModel: EventCreateViewModel
    {
        [Required]
        public string description { get; set; }
        public Hall[] Halls { get; set; }

    }
}
