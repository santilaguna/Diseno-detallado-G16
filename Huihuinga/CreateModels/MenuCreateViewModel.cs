using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Huihuinga.Models
{
    public class MenuCreateViewModel
    {
        [Required]
        public Guid EventId { get; set; }
        [Required]
        public string name { get; set; }
        [Required]
        public string menu { get; set; }
        [Required]
        public IFormFile file { get; set; }
    }
}
