using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WorkMate.Filters
{
    public class RequiredIfAttribute : ValidationAttribute
    {
        private readonly string _comparisonProperty;
        private readonly object _expectedValue;

        public RequiredIfAttribute(string comparisonProperty, object expectedValue)
        {
            _comparisonProperty = comparisonProperty;
            _expectedValue = expectedValue;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var property = validationContext.ObjectType.GetProperty(_comparisonProperty);
            var actualValue = property?.GetValue(validationContext.ObjectInstance, null);

            if (actualValue?.ToString() == _expectedValue.ToString())
            {
                if (value == null)
                    return new ValidationResult($"{validationContext.MemberName} is required.");
            }
            return ValidationResult.Success;
        }
    }
}