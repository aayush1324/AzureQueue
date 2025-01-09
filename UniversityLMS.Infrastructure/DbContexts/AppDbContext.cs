using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniversityLMS.Domain.Entities;

namespace UniversityLMS.Infrastructure.DbContexts
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Assignment> Assignments { get; set; }

        public DbSet<Submission> Submissions { get; set; }

        public DbSet<TestCase> TestCases { get; set; }

        public DbSet<ExceptionLog> ExceptionLogs { get; set; }

        public DbSet<OutboxMessage> OutboxMessages { get; set; }

        public DbSet<SubmitAssignment> SubmitAssignments { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuring entities with keys
            modelBuilder.Entity<Submission>().ToTable("Submissions");
            modelBuilder.Entity<Assignment>().ToTable("Assignments");
            modelBuilder.Entity<TestCase>().ToTable("TestCases");
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<ExceptionLog>().ToTable("ExceptionLogs");


            modelBuilder.Entity<Submission>()
                .HasOne(s => s.Assignment)
                .WithMany(a => a.Submissions)
                .HasForeignKey(s => s.AssignmentId);

            modelBuilder.Entity<Submission>()
                .HasOne(s => s.User)
                .WithMany(st => st.Submissions)
                .HasForeignKey(s => s.UserId);

            modelBuilder.Entity<TestCase>()
                .HasOne(tc => tc.Assignment)
                .WithMany(a => a.TestCases)
                .HasForeignKey(tc => tc.AssignmentId);

            // Seed data for Users
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = Guid.Parse("c7bdf8c1-847a-4ea2-b9e1-f77b43d52baf"),
                    FirstName = "John",
                    LastName = "Doe",
                    UniversityID = "U001",
                    Email = "john.doe@example.com",
                   
                    Phone = "1234567890",
                    Role = "Student"
                },
                new User
                {
                    Id = Guid.Parse("dd7e5871-0cb2-452a-b1a8-1b7d0aebed5d"),
                    FirstName = "Jane",
                    LastName = "Smith",
                    UniversityID = "U002",
                    Email = "jane.smith@example.com",
                  
                    Phone = "0987654321",
                    Role = "Admin"
                },
                new User
                {
                    Id = Guid.Parse("3f0c4d62-0d56-4e4c-8424-d2d575bf08e5"),
                    FirstName = "Alice",
                    LastName = "Johnson",
                    UniversityID = "U003",
                    Email = "alice.johnson@example.com",
                  
                    Phone = "1122334455",
                    Role = "Teacher"
                }
            );


            modelBuilder.Entity<ExceptionLog>(entity =>
            {
                entity.HasKey(e => e.Id); // Define primary key

                entity.Property(e => e.ExceptionMessage)
                      .IsRequired()
                      .HasColumnType("NVARCHAR(MAX)");

                entity.Property(e => e.StackTrace)
                      .HasColumnType("NVARCHAR(MAX)");

                entity.Property(e => e.Source)
                      .HasMaxLength(255);

                entity.Property(e => e.LoggedAt)
                      .HasDefaultValueSql("GETDATE()");
            });


            base.OnModelCreating(modelBuilder);
        

    }
    }
}
