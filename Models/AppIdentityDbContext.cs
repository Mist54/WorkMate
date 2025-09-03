using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace WorkMate.Models
{
    public class AppIdentityDbContext : IdentityDbContext<AppUsers, AppRole, int, UserLogin, UserRole, UserClaim>
    {
        public AppIdentityDbContext() : base("WorkMateConnection") { }

        public static AppIdentityDbContext Create()
        {
            return new AppIdentityDbContext();
        }

        /// <summary>
        /// Once the Modal class has been created or Inherited 
        /// the Connections should be set here 
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<UserLogin>()
                .HasKey(l => new { l.LoginProvider, l.ProviderKey, l.UserId });


            modelBuilder.Entity<UserRole>()
                .HasKey(r => new { r.UserId, r.RoleId });


            modelBuilder.Entity<UserClaim>()
                .HasKey(c => c.Id);

            // (Optional) custom table names — do it HERE (not in AppDbContext)
            modelBuilder.Entity<AppUsers>().ToTable("AppUsers");     
            modelBuilder.Entity<AppRole>().ToTable("Roles");         
            modelBuilder.Entity<UserRole>().ToTable("UserRoles"); 
            modelBuilder.Entity<UserLogin>().ToTable("UserLogins");
            modelBuilder.Entity<UserClaim>().ToTable("UserClaims");
        }
    }
}