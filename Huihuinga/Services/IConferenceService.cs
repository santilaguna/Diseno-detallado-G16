﻿using Huihuinga.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Huihuinga.Services
{
    public interface IConferenceService
    {
        Task<Conference[]> GetConferencesAsync();

        Task<bool> Create(Conference newConference);

        Task<Conference> Details(Guid id);

        Task<bool> Edit(Guid id, string name, string description, CalendarRepetition calendarRepetition);

        Task<bool> Delete(Guid id);
    }
}
