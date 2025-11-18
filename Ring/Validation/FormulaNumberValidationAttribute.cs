using System;
using System.ComponentModel.DataAnnotations;

namespace Ring.Validation
{
    /// <summary>
    /// Custom validation attribute for formula numbers with business rules
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class FormulaNumberValidationAttribute : ValidationAttribute
    {
        private readonly int _minValue;
        private readonly int _maxValue;

        /// <summary>
        /// Initializes a new instance of the FormulaNumberValidationAttribute
        /// </summary>
        /// <param name="minValue">Minimum allowed formula number (default: 1)</param>
        /// <param name="maxValue">Maximum allowed formula number (default: 9999)</param>
        public FormulaNumberValidationAttribute(int minValue = 1, int maxValue = 9999)
        {
            _minValue = minValue;
            _maxValue = maxValue;
        }

        /// <summary>
        /// Validates the formula number against business rules
        /// </summary>
        /// <param name="value">The formula number value to validate</param>
        /// <param name="validationContext">The validation context</param>
        /// <returns>ValidationResult indicating success or failure</returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success; // Let Required attribute handle null values
            }

            if (!(value is int formulaNumber))
            {
                return new ValidationResult("Formula number must be an integer value.");
            }

            if (formulaNumber < _minValue)
            {
                return new ValidationResult($"Formula number must be at least {_minValue}.");
            }

            if (formulaNumber > _maxValue)
            {
                return new ValidationResult($"Formula number cannot exceed {_maxValue}.");
            }

            return ValidationResult.Success;
        }
    }
}
