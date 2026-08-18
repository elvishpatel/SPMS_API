using Microsoft.EntityFrameworkCore;
using SPMS_API.Models;
using Task = SPMS_API.Models.Task;
using TaskStatus = SPMS_API.Models.TaskStatus;

namespace SPMS_API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Role> Role => Set<Role>();
        public DbSet<UserType> UserType => Set<UserType>();
        public DbSet<User> User => Set<User>();
        public DbSet<UserRole> UserRole => Set<UserRole>();
        public DbSet<TaskStatus> TaskStatus => Set<TaskStatus>();
        public DbSet<TaskPriority> TaskPriority => Set<TaskPriority>();
        public DbSet<ProjectMaster> ProjectMaster => Set<ProjectMaster>();
        public DbSet<ProjectAllocation> ProjectAllocation => Set<ProjectAllocation>();
        public DbSet<Task> Task => Set<Task>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Role>()
                .HasIndex(r => r.RoleName)
                .IsUnique();

            modelBuilder.Entity<UserType>()
                .HasIndex(p => p.UserTypeName)
                .IsUnique();

            // Prevent duplicate user-role pairs
            modelBuilder.Entity<UserRole>()
                .HasIndex(ur => new { ur.RoleId, ur.UserId })
                .IsUnique();

            modelBuilder.Entity<ProjectMaster>()
                .HasIndex(p => p.ProjectTitle)
                .IsUnique();

            modelBuilder.Entity<TaskStatus>()
                .HasIndex(ts => ts.TaskStatusName)
                .IsUnique();

            modelBuilder.Entity<TaskPriority>()
                .HasIndex(tp => tp.TaskPriorityName)
                .IsUnique();

            // Configure relationships

            // UserType to User relationship
            modelBuilder.Entity<User>()
                .HasOne(u => u.UserType)
                .WithMany(ut => ut.Users)
                .HasForeignKey(u => u.UserTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // User -> UserRole (1:N)
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Role -> UserRole (1:N)
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Project -> ProjectAllocation
            modelBuilder.Entity<ProjectAllocation>()
                .HasOne(pa => pa.ProjectMaster)
                .WithMany(p => p.ProjectAllocations)
                .HasForeignKey(pa => pa.ProjectID)
                .OnDelete(DeleteBehavior.Restrict);

            // Student -> ProjectAllocation
            modelBuilder.Entity<ProjectAllocation>()
                .HasOne(pa => pa.Student)
                .WithMany(u => u.StudentProjectAllocations)
                .HasForeignKey(pa => pa.StudentID)
                .OnDelete(DeleteBehavior.Restrict);

            // Faculty -> ProjectAllocation
            modelBuilder.Entity<ProjectAllocation>()
                .HasOne(pa => pa.Faculty)
                .WithMany(u => u.FacultyProjectAllocations)
                .HasForeignKey(pa => pa.FacultyID)
                .OnDelete(DeleteBehavior.Restrict);

            // ProjectAllocation -> Task
            modelBuilder.Entity<Task>()
                .HasOne(t => t.ProjectAllocation)
                .WithMany(pa => pa.Tasks)
                .HasForeignKey(t => t.ProjectAllocationID)
                .OnDelete(DeleteBehavior.Restrict);

            // TaskStatus -> Task
            modelBuilder.Entity<Task>()
                .HasOne(t => t.TaskStatus)
                .WithMany(ts => ts.Tasks)
                .HasForeignKey(t => t.TaskStatusID)
                .OnDelete(DeleteBehavior.Restrict);

            // TaskPriority -> Task
            modelBuilder.Entity<Task>()
                .HasOne(t => t.TaskPriority)
                .WithMany(tp => tp.Tasks)
                .HasForeignKey(t => t.TaskPriorityID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
