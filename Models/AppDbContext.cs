using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace WorkMate.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() : base("WorkMateConnection") { }

        //Tables will be defined here 
        public DbSet<TaskModel> Tasks { get; set; }
        public DbSet<TestCaseModel> TestCases { get; set; }
        public DbSet<TagModel> Tags { get; set; }
        //Many to many relationship
        public DbSet<TestCaseTagModel> TestCaseTags { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TestCaseTagModel>().HasKey(t => new { t.TagId, t.TestCaseId });
            base.OnModelCreating(modelBuilder);
        }

    }
}