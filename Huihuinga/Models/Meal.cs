using Huihuinga.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Huihuinga.Models
{
    public class Meal: Event
    {
        public override async Task DeleteSelf(ApplicationDbContext _context)
        {
            _context.Meals.Attach(this);
            _context.Meals.Remove(this);
            await _context.SaveChangesAsync();
        }

        public List<Menu> Menus { get; set; }
    }
}
