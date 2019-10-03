﻿using Huihuinga.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Huihuinga.Services
{
    public interface IEventCenterService
    {
        Task<EventCenter[]> GetEventCentersAsync();

        Task<bool> Create(EventCenter newEventCenter);

        Task<EventCenter> Details(Guid id);
    }
}