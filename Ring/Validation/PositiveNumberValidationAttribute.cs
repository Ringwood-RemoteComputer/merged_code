using System;
using System.ComponentModel.DataAnnotations;

namespace Ring.Validation
{
    /// <summary>
    /// Custom validation attribute to ensure numbers are positive
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class PositiveNumberValidationAttribute : ValidationAttribute
    {
        private readonly bool _allowZero;

        /// <summary>
        /// Initializes a new instance of the PositiveNumberValidationAttribute
        /// </summary>
        /// <param name="allowZero">Whether zero is allowed (default: false)</param>
        public PositiveNumberValidationAttribute(bool allowZero = false)
        {
            _allowZero = allowZero;
        }

        /// <summary>
        /// Validates that the number is positive
        /// </summary>
        /// <param name="value">The value to validate</param>
        /// <param name="validationContext">The validation context</param>
        /// <returns>ValidationResult indicating success or failure</returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success; // Let Required attribute handle null values
            }

            // Handle different numeric types
            decimal numericValue;
            if (value is int intValue)
            {
                numericValue = intValue;
            }
            else if (value is decimal decimalValue)
            {
                numericValue = decimalValue;
            }
            else if (value is double doubleValue)
            {
                numericValue = (decimal)doubleValue;
            }
            else if (value is float floatValue)
            {
                numericValue = (decimal)floatValue;
            }
            else
            {
                return new ValidationResult("Value must be a numeric type.");
            }

            if (_allowZero && numericValue < 0)
            {
                return new ValidationResult($"{validationContext.DisplayName} must be zero or positive.");
            }
            else if (!_allowZero && numericValue <= 0)
            {
                return new ValidationResult($"{validationContext.DisplayName} must be positive.");
            }

            return ValidationResult.Success;
        }
    }
}
