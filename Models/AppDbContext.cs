using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using WorkMate.Helpers;

namespace WorkMate.Models
{
    public class AppDbContext : IdentityDbContext<AppUsers, AppRole, int, UserLogin, UserRole, UserClaim>
    {
        public AppDbContext() : base("WorkMateConnection") { }

        public static AppDbContext Create()
        {
            return new AppDbContext();
        }

        public override int SaveChanges()
        {
            OnBeforeSaving();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync()
        {
            OnBeforeSaving();
            return await base.SaveChangesAsync();
        }

        private void OnBeforeSaving()
        {
            var entries = ChangeTracker.Entries().Where(e => e.Entity is AuditableEntityModel && (e.State == EntityState.Added || e.State == EntityState.Modified));
            var currentUser = HttpContext.Current?.User?.Identity?.Name ?? HttpContextCurrentUserService.GetCurrentUser(); // Get current user or default to "System"
            if (string.IsNullOrWhiteSpace(currentUser))
            {
                throw new Exception("Current user is not available in the context.");
            }
            foreach (var entry in entries)
            {
                var auditableEntity = (AuditableEntityModel)entry.Entity;

                if (entry.State == EntityState.Added)
                {
                    auditableEntity.CreatedDate = DateTime.UtcNow;
                    auditableEntity.CreatedBy = currentUser;
                }
                else if (entry.State == EntityState.Modified)
                {
                    auditableEntity.UpdatedDate = DateTime.UtcNow;
                    auditableEntity.UpdatedBy = currentUser;
                }
            }
        }
        // Application tables
        public DbSet<TaskModel> Tasks { get; set; }
        public DbSet<TestCaseModel> TestCases { get; set; }
        public DbSet<TagModel> Tags { get; set; }
        public DbSet<TestCaseTagModel> TestCaseTags { get; set; }
        public DbSet<TimeTrackModel> TimeTracks { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Identity configurations
            modelBuilder.Entity<UserLogin>()
                .HasKey(l => new { l.LoginProvider, l.ProviderKey, l.UserId });

            modelBuilder.Entity<UserRole>()
                .HasKey(r => new { r.UserId, r.RoleId });

            modelBuilder.Entity<UserClaim>()
                .HasKey(c => c.Id);

            

            modelBuilder.Entity<AppUsers>().ToTable("AppUsers");
            modelBuilder.Entity<AppRole>().ToTable("Roles");
            modelBuilder.Entity<UserRole>().ToTable("UserRoles");
            modelBuilder.Entity<UserLogin>().ToTable("UserLogins");
            modelBuilder.Entity<UserClaim>().ToTable("UserClaims");

            // Application table configurations
            modelBuilder.Entity<TaskModel>().ToTable("Tasks");
            modelBuilder.Entity<TestCaseModel>().ToTable("TestCases");
            modelBuilder.Entity<TagModel>().ToTable("Tags");
            modelBuilder.Entity<TestCaseTagModel>().ToTable("TestCaseTags");
            modelBuilder.Entity<TimeTrackModel>().ToTable("TimeTracks");
        }
    }
}
