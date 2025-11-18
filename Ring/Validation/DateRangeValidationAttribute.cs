using System;
using System.ComponentModel.DataAnnotations;

namespace Ring.Validation
{
    /// <summary>
    /// Custom validation attribute to ensure end date is after start date
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class DateRangeValidationAttribute : ValidationAttribute
    {
        private readonly string _startDatePropertyName;

        /// <summary>
        /// Initializes a new instance of the DateRangeValidationAttribute
        /// </summary>
        /// <param name="startDatePropertyName">Name of the start date property</param>
        public DateRangeValidationAttribute(string startDatePropertyName)
        {
            _startDatePropertyName = startDatePropertyName ?? throw new ArgumentNullException(nameof(startDatePropertyName));
        }

        /// <summary>
        /// Validates the end date against the start date
        /// </summary>
        /// <param name="value">The end date value to validate</param>
        /// <param name="validationContext">The validation context</param>
        /// <returns>ValidationResult indicating success or failure</returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success; // Let Required attribute handle null values
            }

            if (!(value is DateTime endDate))
            {
                return new ValidationResult("End date must be a valid DateTime value.");
            }

            // Get the start date property
            var startDateProperty = validationContext.ObjectType.GetProperty(_startDatePropertyName);
            if (startDateProperty == null)
            {
                return new ValidationResult($"Start date property '{_startDatePropertyName}' not found.");
            }

            var startDateValue = startDateProperty.GetValue(validationContext.ObjectInstance);
            if (startDateValue == null)
            {
                return ValidationResult.Success; // Let Required attribute handle null values
            }

            if (!(startDateValue is DateTime startDate))
            {
                return new ValidationResult("Start date must be a valid DateTime value.");
            }

            // Validate date range
            if (endDate <= startDate)
            {
                return new ValidationResult($"End date must be after start date ({startDate:yyyy-MM-dd}).");
            }

            // Validate that dates are not too far in the future
            var maxFutureDate = DateTime.Today.AddYears(1);
            if (endDate > maxFutureDate)
            {
                return new ValidationResult($"End date cannot be more than 1 year in the future.");
            }

            // Validate that dates are not too far in the past
            var minPastDate = DateTime.Today.AddYears(-10);
            if (startDate < minPastDate)
            {
                return new ValidationResult($"Start date cannot be more than 10 years in the past.");
            }

            return ValidationResult.Success;
        }
    }
}
