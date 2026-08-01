using Microsoft.EntityFrameworkCore;
using SPMS_API.Models;
using System.Security;
namespace SPMS_API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<ProjectAllocation> ProjectAllocation => Set<ProjectAllocation>();

        public DbSet<ProjectMaster> ProjectMaster => Set<ProjectMaster>();

        public DbSet<Role> Role => Set<Role>();

        public DbSet<SPMS_API.Models.Task> Task => Set<SPMS_API.Models.Task>();

        public DbSet<TaskPriority> TaskPriority => Set<TaskPriority>();

        public DbSet<SPMS_API.Models.TaskStatus> TaskStatus => Set<SPMS_API.Models.TaskStatus>();

        public DbSet<User> User => Set<User>();

        public DbSet<UserRole> UserRole => Set<UserRole>();

        public DbSet<UserType> UserType => Set<UserType>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProjectAllocation>()
                .ToTable("ProjectAllocation");

            modelBuilder.Entity<ProjectAllocation>()
                .HasOne(pa => pa.ProjectMaster)
                .WithMany()
                .HasForeignKey(pa => pa.ProjectID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProjectAllocation>()
                .HasOne(pa => pa.Student)
                .WithMany()
                .HasForeignKey(pa => pa.StudentID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProjectAllocation>()
                .HasOne(pa => pa.Faculty)
                .WithMany()
                .HasForeignKey(pa => pa.FacultyID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProjectMaster>()
                .ToTable("ProjectMaster")
                .HasIndex(p => p.ProjectTitle)
                .IsUnique();

            modelBuilder.Entity<Role>()
                .HasIndex(r => r.RoleName)
                .IsUnique();

            modelBuilder.Entity<SPMS_API.Models.Task>()
                .HasOne(pa => pa.ProjectAllocation)
                .WithMany()
                .HasForeignKey(pa => pa.ProjectAllocationID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SPMS_API.Models.Task>()
               .HasOne(pa => pa.TaskStatus)
               .WithMany()
               .HasForeignKey(pa => pa.TaskStatusID)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SPMS_API.Models.Task>()
               .HasOne(pa => pa.TaskPriority)
               .WithMany()
               .HasForeignKey(pa => pa.TaskPriorityID)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TaskPriority>()
                .HasIndex(tp => tp.TaskPriorityName)
                .IsUnique();

            modelBuilder.Entity<SPMS_API.Models.TaskStatus>()
                .HasIndex(ts => ts.TaskStatusName)
                .IsUnique();

            modelBuilder.Entity<User>()
               .HasIndex(u => u.Email)
               .IsUnique();

            modelBuilder.Entity<UserRole>()
                .HasIndex(ur => new { ur.RoleId, ur.UserId })
                .IsUnique();

            modelBuilder.Entity<UserType>()
                .HasIndex(p => p.UserTypeName)
                .IsUnique();



        }
    }
}
