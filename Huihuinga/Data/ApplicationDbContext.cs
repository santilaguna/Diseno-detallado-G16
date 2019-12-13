using System;
using System.Collections.Generic;
using System.Text;
using Huihuinga.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Huihuinga.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<EventCenter> EventCenters { get; set; }
        public DbSet<Hall> Halls { get; set; }
        public DbSet<Meal> Meals { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Party> Parties { get; set; }
        public DbSet<Talk> Talks { get; set; }
        public DbSet<PracticalSession> PracticalSessions { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<ConcreteConference> ConcreteConferences { get; set; }
        public DbSet<Conference> Conferences { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<ApplicationUserConcreteConference> UserConferences { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<EventTopic> EventTopics { get; set; }
        public DbSet<Material> Materials { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<ApplicationUserEvent> UserEvents { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<ConferenceFeedback> ConferenceFeedbacks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ApplicationUserConcreteConference>()
                .HasKey(bc => new { bc.UserId, bc.ConferenceId });  
            modelBuilder.Entity<ApplicationUserConcreteConference>()
                .HasOne(bc => bc.Conference)
                .WithMany(b => b.UsersConferences)
                .HasForeignKey(bc => bc.ConferenceId);  
            modelBuilder.Entity<ApplicationUserConcreteConference>()
                .HasOne(bc => bc.User)
                .WithMany(c => c.UsersConferences)
                .HasForeignKey(bc => bc.UserId);
  
            modelBuilder.Entity<ApplicationUserEvent>()
                .HasKey(bc => new { bc.UserId, bc.EventId });  
            modelBuilder.Entity<ApplicationUserEvent>()
                .HasOne(bc => bc.Event)
                .WithMany(b => b.UsersEvents)
                .HasForeignKey(bc => bc.EventId);  
            modelBuilder.Entity<ApplicationUserEvent>()
                .HasOne(bc => bc.User)
                .WithMany(c => c.UsersEvents)
                .HasForeignKey(bc => bc.UserId);

            modelBuilder.Entity<EventTopic>()
                .HasKey(c => new { c.EventId, c.TopicId });

        }
    }
}
