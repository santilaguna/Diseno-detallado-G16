using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Huihuinga.Models
{
    public class ExpositorToChatCreateViewModel
    {
        public string ExpositorId { get; set; }
        public ApplicationUser[] Users { get; set; }

        public Guid event_id { get; set; }
    }
}
