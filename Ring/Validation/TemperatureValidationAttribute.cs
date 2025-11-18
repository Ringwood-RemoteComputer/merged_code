using System;
using System.ComponentModel.DataAnnotations;

namespace Ring.Validation
{
    /// <summary>
    /// Custom validation attribute for temperature values with business rules
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class TemperatureValidationAttribute : ValidationAttribute
    {
        private readonly double _minTemperature;
        private readonly double _maxTemperature;

        /// <summary>
        /// Initializes a new instance of the TemperatureValidationAttribute
        /// </summary>
        /// <param name="minTemperature">Minimum allowed temperature (default: -50°C)</param>
        /// <param name="maxTemperature">Maximum allowed temperature (default: 200°C)</param>
        public TemperatureValidationAttribute(double minTemperature = -50, double maxTemperature = 200)
        {
            _minTemperature = minTemperature;
            _maxTemperature = maxTemperature;
        }

        /// <summary>
        /// Validates the temperature against business rules
        /// </summary>
        /// <param name="value">The temperature value to validate</param>
        /// <param name="validationContext">The validation context</param>
        /// <returns>ValidationResult indicating success or failure</returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success; // Let Required attribute handle null values
            }

            double temperature;
            if (value is decimal decimalValue)
            {
                temperature = (double)decimalValue;
            }
            else if (value is double doubleValue)
            {
                temperature = doubleValue;
            }
            else if (value is float floatValue)
            {
                temperature = floatValue;
            }
            else
            {
                return new ValidationResult("Temperature must be a numeric value.");
            }

            if (temperature < _minTemperature)
            {
                return new ValidationResult($"Temperature must be at least {_minTemperature}°C.");
            }

            if (temperature > _maxTemperature)
            {
                return new ValidationResult($"Temperature cannot exceed {_maxTemperature}°C.");
            }

            return ValidationResult.Success;
        }
    }
}
