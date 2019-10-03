using System;
using System.Collections.Generic;
using System.Text;
using Huihuinga.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Huihuinga.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<EventCenter> EventCenters { get; set; }
        public DbSet<Hall> Halls { get; set; }
        public DbSet<ConcreteConference> ConcreteConferences { get; set; }
    }
}
