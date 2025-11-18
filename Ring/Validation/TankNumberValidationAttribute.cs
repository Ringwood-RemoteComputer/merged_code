using System;
using System.ComponentModel.DataAnnotations;

namespace Ring.Validation
{
    /// <summary>
    /// Custom validation attribute for storage tank numbers with business rules
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class TankNumberValidationAttribute : ValidationAttribute
    {
        private readonly int _minValue;
        private readonly int _maxValue;

        /// <summary>
        /// Initializes a new instance of the TankNumberValidationAttribute
        /// </summary>
        /// <param name="minValue">Minimum allowed tank number (default: 1)</param>
        /// <param name="maxValue">Maximum allowed tank number (default: 50)</param>
        public TankNumberValidationAttribute(int minValue = 1, int maxValue = 50)
        {
            _minValue = minValue;
            _maxValue = maxValue;
        }

        /// <summary>
        /// Validates the tank number against business rules
        /// </summary>
        /// <param name="value">The tank number value to validate</param>
        /// <param name="validationContext">The validation context</param>
        /// <returns>ValidationResult indicating success or failure</returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success; // Let Required attribute handle null values
            }

            if (!(value is int tankNumber))
            {
                return new ValidationResult("Tank number must be an integer value.");
            }

            if (tankNumber < _minValue)
            {
                return new ValidationResult($"Tank number must be at least {_minValue}.");
            }

            if (tankNumber > _maxValue)
            {
                return new ValidationResult($"Tank number cannot exceed {_maxValue}.");
            }

            return ValidationResult.Success;
        }
    }
}
