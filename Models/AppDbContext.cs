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
            //here we will define the names 
            //If explicity not defined it will take the names of the table as same as class name
            modelBuilder.Entity<TaskModel>().ToTable("Tasks");
            modelBuilder.Entity<TestCaseModel>().ToTable("TestCases");
            modelBuilder.Entity<TagModel>().ToTable("Tags");
            modelBuilder.Entity<TestCaseTagModel>().ToTable("TestCaseTags");

            modelBuilder.Entity<TestCaseTagModel>().HasKey(t => new { t.TagId, t.TestCaseId });
            base.OnModelCreating(modelBuilder);
        }

    }
}