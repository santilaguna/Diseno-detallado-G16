﻿using Huihuinga.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Huihuinga.Services
{
    public interface ITalkService : ITopicalService
    {
        Task<Talk[]> GetTalksAsync();

        Task<bool> Create(Talk newchat);

        Task<Talk> Details(Guid id);

        Task<Hall[]> GetHalls(Guid? id);

        Task<bool> Edit(Guid id, string name, DateTime starttime, DateTime endtime, Guid Hallid, string description);

        Task<bool> Delete(Guid id);

        Task<bool> CheckUser(Guid id, string UserId);

        Task<bool> CreateMaterial(Material material);

        Task<Material[]> GetMaterial(Guid id);

        Task<bool> DeleteMaterial(Guid MaterialId);

        Task<Talk[]> GetTalksWithPendingFeedbacks(string UserId);

        Task<bool> CreateFeedback(Feedback feedback, Guid event_id);

        Task<Talk[]> GetFinishedTalks();

        Task<double> MaterialQuality(Guid eventId);

        Task<double> PlaceQuality(Guid eventId);
        Task<double> ExpositorQuality(Guid eventId);
        Task<List<string>> Comments(Guid eventId);

        Task<bool> CanFeedback(string UserId, Guid EventId);
    }
}
