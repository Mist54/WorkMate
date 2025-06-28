using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkMate.Models
{
    public enum FieldType
    {
        Number,
        AlphaNumeric,
        Email,
        PhoneNumber
        
    }
    public class TestAutomatorModel
    {
        public FieldType FieldType { get; set; }

        public Numerical NumericalConstraints { get; set; }
        public AlphaNumeric AlphaNumericConstraints { get; set; }
        public EmailConstraints EmailConstraints { get; set; }
        public PhoneNumberConstraints PhoneNumberConstraints { get; set; }
    }

    public class Numerical
    {
        public bool AllowNegative { get; set; }
        public bool AllowZero { get; set; }
        public bool AllowDecimal { get; set; }
        public bool AllowImaginary { get; set; }

        public int? MinValue { get; set; }
        public int? MaxValue { get; set; }
    }


    public class AlphaNumeric
    {
        public bool AllowCaseSensitive { get; set; }
        public bool AllowSpecialCharacter { get; set; }
        public bool AllowSpace { get; set; }

        public int? MinLength { get; set; }
        public int? MaxLength { get; set; }
    }

    public class EmailConstraints
    {
        public bool AllowSubDomain { get; set; }              // test@sub.domain.com
        public bool AllowNumericUser { get; set; }            // 123@domain.com
        public bool AllowSpecialCharsInUser { get; set; }     // user.name+tag@domain.com
        public bool AllowInvalidDomain { get; set; }          // For edge case generation
    }

    public class PhoneNumberConstraints
    {
        public bool AllowCountryCode { get; set; }            // +91, +1, etc.
        public bool AllowExtension { get; set; }              // 123-456-7890 x123
        public bool AllowSeparators { get; set; }             // 123-456-7890 or (123) 456-7890
        public bool AllowInvalidLength { get; set; }          // e.g., 5-digit or 15-digit cases
    }
}