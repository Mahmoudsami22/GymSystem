using GymSystem.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace GymSystem.DAL.Contexts
{
    public class GymDbContext : DbContext
    {
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer("Server=.;Database=GymDb;Trusted_Connection=True;TrustServerCertificate=True;");
        //}
        public GymDbContext(DbContextOptions<GymDbContext> options) : base(options)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
        public DbSet<Plan> Plans { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Trainer> Trainers { get; set; }
        public DbSet<MemberShip> MemberShips { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<HealthRecord> HealthRecords { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Category> Categorys { get; set; }
        
        
        

    }
}
