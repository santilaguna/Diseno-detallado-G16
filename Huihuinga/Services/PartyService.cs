﻿using Huihuinga.Data;
using Huihuinga.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Huihuinga.Services
{
    public class PartyService:IPartyService
    {
        private readonly ApplicationDbContext _context;
        public PartyService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Party[]> GetPartiesAsync()
        {
            var parties = await _context.Parties.Where(e => e.concreteConferenceId == null).ToArrayAsync();
            return parties;
        }

        public async Task<bool> Create(Party newparty)
        {
            newparty.id = Guid.NewGuid();
            _context.Parties.Add(newparty);
            var saveResult = await _context.SaveChangesAsync(); return saveResult == 1;

        }

        public async Task<Party> Details(Guid id)
        {
            var parties = await _context.Parties.Where(x => x.id == id).ToArrayAsync();
            return parties[0];
        }

        public async Task<Hall[]> GetHalls()
        {
            var halls = await _context.Halls.ToArrayAsync();
            return halls;
        }

        public async Task<bool> Edit(Guid id, string name, DateTime starttime, DateTime endtime, Guid Hallid, string description)
        {
            var partytoupdate = await _context.Parties.FirstOrDefaultAsync(s => s.id == id);
            partytoupdate.name = name;
            partytoupdate.starttime = starttime;
            partytoupdate.endtime = endtime;
            partytoupdate.Hallid = Hallid;
            partytoupdate.description = description;
            _context.Update(partytoupdate);
            var saveResult = await _context.SaveChangesAsync(); return saveResult == 1;
        }

        public async Task<bool> Delete(Guid id)
        {
            // TODO: delete from conference if is not null
            var partytodelete = await _context.Parties.FirstOrDefaultAsync(s => s.id == id);
            _context.Parties.Attach(partytodelete);
            _context.Parties.Remove(partytodelete);
            var saveResult = await _context.SaveChangesAsync();
            return saveResult == 1;
        }
    }
}
