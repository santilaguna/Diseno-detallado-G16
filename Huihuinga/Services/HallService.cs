﻿using Huihuinga.Data;
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
    }
}