using Huihuinga.Data;
using Huihuinga.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Huihuinga.Services
{
    public class HallService: IHallService
    {
        private readonly ApplicationDbContext _context;
        public HallService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Hall[]> GetHallsAsync(Guid centerid)
        {
            var halls = await _context.Halls.Where(x => x.EventCenterid == centerid).ToArrayAsync();
            return halls;
        }

        public async Task<bool> Create(Hall newHall)
        {
            newHall.id = Guid.NewGuid();
            _context.Halls.Add(newHall);
            var saveResult = await _context.SaveChangesAsync(); return saveResult == 1;

        }

        public async Task<Hall> Details(Guid id)
        {
            var halls = await _context.Halls.Where(x => x.id == id).ToArrayAsync();
            return halls[0];
        }

        public async Task<EventCenter> FindCenter(Guid centerid)
        {
            var centers = await _context.EventCenters.Where(x => x.id == centerid).ToArrayAsync();
            return centers[0];
        }


        public async Task<bool> Edit(Guid id, string name, int capacity, string location, bool projector, int plugs, int computers)
        {
            var halltoupdate = await _context.Halls.FirstOrDefaultAsync(s => s.id == id);
            halltoupdate.capacity = capacity;
            halltoupdate.location = location;
            halltoupdate.projector = projector;
            halltoupdate.plugs = plugs;
            halltoupdate.computers = computers;
            halltoupdate.name = name;
            _context.Update(halltoupdate);
            var saveResult = await _context.SaveChangesAsync(); return saveResult == 1;
        }

        public async Task<bool> Delete(Guid id)
        {
            var halltodelete = await _context.Halls.FirstOrDefaultAsync(s => s.id == id);
            _context.Halls.Attach(halltodelete);
            _context.Halls.Remove(halltodelete);
            var saveResult = await _context.SaveChangesAsync();
            return saveResult == 1;
        }
    }
}
