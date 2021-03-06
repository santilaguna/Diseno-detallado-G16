﻿// <auto-generated />
using System;
using Huihuinga.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Huihuinga.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20191103022410_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("Huihuinga.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("Huihuinga.Models.ApplicationUserConcreteConference", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<Guid>("ConferenceId");

                    b.HasKey("UserId", "ConferenceId");

                    b.HasIndex("ConferenceId");

                    b.ToTable("UserConferences");
                });

            modelBuilder.Entity("Huihuinga.Models.ConcreteConference", b =>
                {
                    b.Property<Guid>("id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Maxassistants");

                    b.Property<DateTime>("endtime");

                    b.Property<string>("name")
                        .IsRequired();

                    b.Property<DateTime>("starttime");

                    b.HasKey("id");

                    b.ToTable("ConcreteConferences");
                });

            modelBuilder.Entity("Huihuinga.Models.Conference", b =>
                {
                    b.Property<Guid>("id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid?>("Instanceid");

                    b.Property<int>("calendarRepetition");

                    b.Property<string>("description");

                    b.Property<string>("name")
                        .IsRequired();

                    b.HasKey("id");

                    b.HasIndex("Instanceid");

                    b.ToTable("Conferences");
                });

            modelBuilder.Entity("Huihuinga.Models.Event", b =>
                {
                    b.Property<Guid>("id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid?>("ConcreteConferenceid");

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<Guid>("Hallid");

                    b.Property<string>("PhotoPath");

                    b.Property<DateTime>("endtime");

                    b.Property<string>("name")
                        .IsRequired();

                    b.Property<DateTime>("starttime");

                    b.HasKey("id");

                    b.HasIndex("ConcreteConferenceid");

                    b.ToTable("Events");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Event");
                });

            modelBuilder.Entity("Huihuinga.Models.EventCenter", b =>
                {
                    b.Property<Guid>("id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("PhotoPath");

                    b.Property<string>("address")
                        .IsRequired();

                    b.Property<string>("name")
                        .IsRequired();

                    b.HasKey("id");

                    b.ToTable("EventCenters");
                });

            modelBuilder.Entity("Huihuinga.Models.Hall", b =>
                {
                    b.Property<Guid>("id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("EventCenterid");

                    b.Property<string>("PhotoPath");

                    b.Property<int>("capacity");

                    b.Property<int>("computers");

                    b.Property<string>("location")
                        .IsRequired();

                    b.Property<string>("name")
                        .IsRequired();

                    b.Property<int>("plugs");

                    b.Property<bool>("projector");

                    b.HasKey("id");

                    b.HasIndex("EventCenterid");

                    b.ToTable("Halls");
                });

            modelBuilder.Entity("Huihuinga.Models.Sponsor", b =>
                {
                    b.Property<Guid>("id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid?>("ConcreteConferenceid");

                    b.Property<string>("name")
                        .IsRequired();

                    b.HasKey("id");

                    b.HasIndex("ConcreteConferenceid");

                    b.ToTable("Sponsor");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("Huihuinga.Models.Chat", b =>
                {
                    b.HasBaseType("Huihuinga.Models.Event");

                    b.HasDiscriminator().HasValue("Chat");
                });

            modelBuilder.Entity("Huihuinga.Models.Meal", b =>
                {
                    b.HasBaseType("Huihuinga.Models.Event");

                    b.HasDiscriminator().HasValue("Meal");
                });

            modelBuilder.Entity("Huihuinga.Models.Party", b =>
                {
                    b.HasBaseType("Huihuinga.Models.Event");

                    b.Property<string>("description")
                        .IsRequired();

                    b.HasDiscriminator().HasValue("Party");
                });

            modelBuilder.Entity("Huihuinga.Models.PracticalSession", b =>
                {
                    b.HasBaseType("Huihuinga.Models.Event");

                    b.HasDiscriminator().HasValue("PracticalSession");
                });

            modelBuilder.Entity("Huihuinga.Models.Talk", b =>
                {
                    b.HasBaseType("Huihuinga.Models.Event");

                    b.Property<string>("description")
                        .HasColumnName("Talk_description");

                    b.HasDiscriminator().HasValue("Talk");
                });

            modelBuilder.Entity("Huihuinga.Models.ApplicationUserConcreteConference", b =>
                {
                    b.HasOne("Huihuinga.Models.ConcreteConference", "Conference")
                        .WithMany("UsersConferences")
                        .HasForeignKey("ConferenceId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Huihuinga.Models.ApplicationUser", "User")
                        .WithMany("UsersConferences")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Huihuinga.Models.Conference", b =>
                {
                    b.HasOne("Huihuinga.Models.ConcreteConference", "Instance")
                        .WithMany()
                        .HasForeignKey("Instanceid");
                });

            modelBuilder.Entity("Huihuinga.Models.Event", b =>
                {
                    b.HasOne("Huihuinga.Models.ConcreteConference")
                        .WithMany("Events")
                        .HasForeignKey("ConcreteConferenceid");
                });

            modelBuilder.Entity("Huihuinga.Models.Hall", b =>
                {
                    b.HasOne("Huihuinga.Models.EventCenter", "EventCenter")
                        .WithMany("Halls")
                        .HasForeignKey("EventCenterid")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Huihuinga.Models.Sponsor", b =>
                {
                    b.HasOne("Huihuinga.Models.ConcreteConference")
                        .WithMany("Sponsors")
                        .HasForeignKey("ConcreteConferenceid");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Huihuinga.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Huihuinga.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Huihuinga.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Huihuinga.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
