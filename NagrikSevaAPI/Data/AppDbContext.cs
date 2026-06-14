using Microsoft.EntityFrameworkCore;
using NagrikSevaAPI.Models;

namespace NagrikSevaAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<ApplicationTracking> ApplicationTrackings { get; set; }
        public DbSet<Announcement> Announcements { get; set; }
        
        public DbSet<ApplicationDetails>
            ApplicationDetails { get; set; } //  NEW
        protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // Composite primary key for UserRole
    modelBuilder.Entity<UserRole>()
        .HasKey(ur => new { ur.UserId, ur.RoleId });

    // UserRole → User relationship
    modelBuilder.Entity<UserRole>()
        .HasOne(ur => ur.User)
        .WithMany(u => u.UserRoles)
        .HasForeignKey(ur => ur.UserId);

    // UserRole → Role relationship
    modelBuilder.Entity<UserRole>()
        .HasOne(ur => ur.Role)
        .WithMany(r => r.UserRoles)
        .HasForeignKey(ur => ur.RoleId);

    //  Fix cascade delete issue
    modelBuilder.Entity<ApplicationTracking>()
        .HasOne(at => at.Application)
        .WithMany(a => a.Trackings)
        .HasForeignKey(at => at.ApplicationId)
        .OnDelete(DeleteBehavior.Cascade);

    //  This line fixes the error!
    modelBuilder.Entity<ApplicationTracking>()
        .HasOne(at => at.Officer)
        .WithMany()
        .HasForeignKey(at => at.OfficerId)
        .OnDelete(DeleteBehavior.NoAction); //  NoAction instead of Cascade

       //  NEW: ApplicationDetails relationship
            modelBuilder.Entity<ApplicationDetails>()
                .HasOne(ad => ad.Application)
                .WithOne(a => a.Details)
                .HasForeignKey<ApplicationDetails>(
                    ad => ad.ApplicationId)
                .OnDelete(DeleteBehavior.Cascade); 

    // Seed default roles
    modelBuilder.Entity<Role>().HasData(
        new Role { Id = 1, Name = "Citizen" },
        new Role { Id = 2, Name = "Officer" },
        new Role { Id = 3, Name = "Admin" }
    );

    // Seed default departments
    modelBuilder.Entity<Department>().HasData(
        new Department { Id = 1, Name = "Revenue Department", Code = "REV" },
        new Department { Id = 2, Name = "Municipal Corporation", Code = "MUN" },
        new Department { Id = 3, Name = "Transport Department", Code = "TRN" },
        new Department { Id = 4, Name = "Health Department", Code = "HLT" }
    );

    // Seed default services
    modelBuilder.Entity<Service>().HasData(
        new Service { Id = 1, ServiceName = "Birth Certificate", Description = "Apply for birth certificate", DepartmentId = 1 },
        new Service { Id = 2, ServiceName = "Income Certificate", Description = "Apply for income certificate", DepartmentId = 1 },
        new Service { Id = 3, ServiceName = "Property Tax", Description = "Pay property tax online", DepartmentId = 2 },
        new Service { Id = 4, ServiceName = "Driving License", Description = "Apply for driving license", DepartmentId = 3 },
        new Service { Id = 5, ServiceName = "Health Card", Description = "Apply for health card", DepartmentId = 4 }
    );
}
    }
}