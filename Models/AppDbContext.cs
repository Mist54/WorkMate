using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace WorkMate.Models
{
    public class AppDbContext : IdentityDbContext<AppUsers, AppRole, int, UserLogin, UserRole, UserClaim>
    {
        public AppDbContext() : base("WorkMateConnection") { }

        public static AppDbContext Create()
        {
            return new AppDbContext();
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
