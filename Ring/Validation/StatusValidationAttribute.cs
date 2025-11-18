using System;
using System.ComponentModel.DataAnnotations;

namespace Ring.Validation
{
    /// <summary>
    /// Custom validation attribute for batch status with business rules
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class StatusValidationAttribute : ValidationAttribute
    {
        private readonly int[] _validStatuses;

        /// <summary>
        /// Initializes a new instance of the StatusValidationAttribute
        /// </summary>
        /// <param name="validStatuses">Array of valid status values (default: 0-5)</param>
        public StatusValidationAttribute(params int[] validStatuses)
        {
            _validStatuses = validStatuses ?? new int[] { 0, 1, 2, 3, 4, 5 }; // Default statuses: Pending, Running, Completed, Failed, Cancelled, Paused
        }

        /// <summary>
        /// Validates the status against business rules
        /// </summary>
        /// <param name="value">The status value to validate</param>
        /// <param name="validationContext">The validation context</param>
        /// <returns>ValidationResult indicating success or failure</returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success; // Let Required attribute handle null values
            }

            if (!(value is int status))
            {
                return new ValidationResult("Status must be an integer value.");
            }

            if (Array.IndexOf(_validStatuses, status) == -1)
            {
                var validStatusesString = string.Join(", ", _validStatuses);
                return new ValidationResult($"Status must be one of the following values: {validStatusesString}.");
            }

            return ValidationResult.Success;
        }
    }
}
