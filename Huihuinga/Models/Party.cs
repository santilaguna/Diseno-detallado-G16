using Huihuinga.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Huihuinga.Models
{
    public class Party:Event
    {
        [Required]
        public string description { get; set; }
        public override async Task DeleteSelf(ApplicationDbContext _context)
        {
            _context.Parties.Attach(this);
            _context.Parties.Remove(this);
            await _context.SaveChangesAsync();
        }

    }
}
