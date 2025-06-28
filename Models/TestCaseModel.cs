using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkMate.Models
{
    public enum TestCaseType { Functional, Performance, Security, Regression, Integration, Other }
    public enum TestCasePriority { Low, Medium, High, Critical }
    public enum TestCaseStatus { Other,Passed, Failed}

    public class TestCaseModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TestCaseId { get; set; }

        [Required, MaxLength(200)]
        public string TestCaseName { get; set; }

        [Required]
        public DateTime TestCaseTimeStamp { get; set; } = DateTime.Now;

        public string Preconditions { get; set; }

        public string Steps { get; set; }

        public string ExpectedResult { get; set; }

        public string ActualResult { get; set; }

        public TestCaseType Type { get; set; }

        public TestCasePriority Priority { get; set; }

        public TestCaseStatus TestCaseStatus { get; set; }

        public string Notes { get; set; }

        [Required, MaxLength(100)]
        public string CreatedBy { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }= DateTime.Now;

        public DateTime? ModifiedDate { get; set; }

        [Required,MaxLength(100)]
        public string ModifiedBy { get; set; }

        [InverseProperty("TestCases")]
        public virtual TaskModel Task { get; set; }

        [Required]
        public int TaskId { get; set; }

        public virtual ICollection<TestCaseTagModel> TestCaseTags { get; set; } = new List<TestCaseTagModel>();

        public TestCaseModel()
        {
            Preconditions = string.Empty;
            Steps = string.Empty;
            ExpectedResult = string.Empty;
            ActualResult = string.Empty;
            Notes = string.Empty;


        }
    }

    public class TagModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TagId { get; set; }

        [Required, MaxLength(100)]
        public string TagName { get; set; }

        public virtual ICollection<TestCaseTagModel> TestCaseTags { get; set; } = new List<TestCaseTagModel>();
    }

    public class TestCaseTagModel
    {
        [Key, Column(Order = 0)]
        public int TagId { get; set; }

        [Key, Column(Order = 1)]
        public int TestCaseId { get; set; }

        public virtual TestCaseModel TestCase { get; set; }

        
        public virtual TagModel Tag { get; set; }
    }

}