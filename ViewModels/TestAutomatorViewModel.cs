using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WorkMate.Filters;
using WorkMate.Models;

namespace WorkMate.ViewModels
{
    public class TestAutomatorViewModel
    {
        [Required]
        [Display(Name = "Input field type")]
        public FieldType FieldType { get; set; }

        [RequiredIf("FieldType", FieldType.Number)]
        public Numerical NumericalConstraints { get; set; }

        [RequiredIf("FieldType", FieldType.AlphaNumeric)]
        public AlphaNumeric AlphaNumericConstraints { get; set; }

        [RequiredIf("FieldType", FieldType.Email)]
        public EmailConstraints EmailConstraints { get; set; }

        [RequiredIf("FieldType", FieldType.PhoneNumber)]
        public PhoneNumberConstraints PhoneNumberConstraints { get; set; }



    }
}