using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using Ring.Models;
using Ring.Services;

namespace Ring.Services
{
    /// <summary>
    /// Service for handling validation operations
    /// </summary>
    public class ValidationService
    {
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the ValidationService
        /// </summary>
        /// <param name="logger">Logger instance for validation operations</param>
        public ValidationService(ILogger logger = null)
        {
            _logger = logger ?? new Logger();
        }

        /// <summary>
        /// Validates a model object using Data Annotations
        /// </summary>
        /// <typeparam name="T">Type of the model to validate</typeparam>
        /// <param name="model">The model to validate</param>
        /// <returns>Validation result with errors if any</returns>
        public ValidationResult ValidateModel<T>(T model)
        {
            try
            {
                _logger.LogDebug($"Starting validation for {typeof(T).Name}");

                var validationContext = new ValidationContext(model);
                var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
                var isValid = Validator.TryValidateObject(model, validationContext, validationResults, true);

                var result = new ValidationResult
                {
                    IsValid = isValid,
                    Errors = validationResults.Select(vr => vr.ErrorMessage).ToList()
                };

                if (!isValid)
                {
                    _logger.LogWarning($"Validation failed for {typeof(T).Name}: {string.Join(", ", result.Errors)}");
                }
                else
                {
                    _logger.LogDebug($"Validation successful for {typeof(T).Name}");
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during validation of {typeof(T).Name}", ex);
                return new ValidationResult
                {
                    IsValid = false,
                    Errors = new List<string> { $"Validation error: {ex.Message}" }
                };
            }
        }

        /// <summary>
        /// Validates a property of a model object
        /// </summary>
        /// <typeparam name="T">Type of the model</typeparam>
        /// <param name="model">The model to validate</param>
        /// <param name="propertyName">Name of the property to validate</param>
        /// <returns>Validation result with errors if any</returns>
        public ValidationResult ValidateProperty<T>(T model, string propertyName)
        {
            try
            {
                _logger.LogDebug($"Starting property validation for {typeof(T).Name}.{propertyName}");

                var property = typeof(T).GetProperty(propertyName);
                if (property == null)
                {
                    return new ValidationResult
                    {
                        IsValid = false,
                        Errors = new List<string> { $"Property '{propertyName}' not found on {typeof(T).Name}" }
                    };
                }

                var value = property.GetValue(model);
                var validationContext = new ValidationContext(model) { MemberName = propertyName };
                var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
                var isValid = Validator.TryValidateProperty(value, validationContext, validationResults);

                var result = new ValidationResult
                {
                    IsValid = isValid,
                    Errors = validationResults.Select(vr => vr.ErrorMessage).ToList()
                };

                if (!isValid)
                {
                    _logger.LogWarning($"Property validation failed for {typeof(T).Name}.{propertyName}: {string.Join(", ", result.Errors)}");
                }
                else
                {
                    _logger.LogDebug($"Property validation successful for {typeof(T).Name}.{propertyName}");
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during property validation of {typeof(T).Name}.{propertyName}", ex);
                return new ValidationResult
                {
                    IsValid = false,
                    Errors = new List<string> { $"Property validation error: {ex.Message}" }
                };
            }
        }

        /// <summary>
        /// Validates a BatchQuery object with additional business rules
        /// </summary>
        /// <param name="batchQuery">The batch query to validate</param>
        /// <returns>Validation result with errors if any</returns>
        public ValidationResult ValidateBatchQuery(BatchQuery batchQuery)
        {
            var result = ValidateModel(batchQuery);

            // Additional business rule validations
            if (result.IsValid)
            {
                var businessRuleErrors = new List<string>();

                // Check if end weight is greater than start weight (if both are set)
                if (batchQuery.EndWeight > 0 && batchQuery.StartWeight > 0 && batchQuery.EndWeight < batchQuery.StartWeight)
                {
                    businessRuleErrors.Add("End weight cannot be less than start weight.");
                }

                // Check if batch duration is reasonable (not more than 24 hours)
                var duration = batchQuery.Duration;
                if (duration.TotalHours > 24)
                {
                    businessRuleErrors.Add("Batch duration cannot exceed 24 hours.");
                }

                // Check if batch duration is positive
                if (duration.TotalSeconds <= 0)
                {
                    businessRuleErrors.Add("Batch duration must be positive.");
                }

                if (businessRuleErrors.Any())
                {
                    result.IsValid = false;
                    result.Errors.AddRange(businessRuleErrors);
                    _logger.LogWarning($"Business rule validation failed for BatchQuery {batchQuery.Id}: {string.Join(", ", businessRuleErrors)}");
                }
            }

            return result;
        }

        /// <summary>
        /// Validates a BatchQueryFilter object
        /// </summary>
        /// <param name="filter">The filter to validate</param>
        /// <returns>Validation result with errors if any</returns>
        public ValidationResult ValidateBatchQueryFilter(BatchQueryFilter filter)
        {
            var result = ValidateModel(filter);

            // Additional filter validations
            if (result.IsValid)
            {
                var businessRuleErrors = new List<string>();

                // Check if date range is reasonable (not more than 1 year)
                if (filter.StartDate.HasValue && filter.EndDate.HasValue)
                {
                    var dateRange = filter.EndDate.Value - filter.StartDate.Value;
                    if (dateRange.TotalDays > 365)
                    {
                        businessRuleErrors.Add("Date range cannot exceed 1 year.");
                    }
                }

                if (businessRuleErrors.Any())
                {
                    result.IsValid = false;
                    result.Errors.AddRange(businessRuleErrors);
                    _logger.LogWarning($"Filter validation failed: {string.Join(", ", businessRuleErrors)}");
                }
            }

            return result;
        }

        /// <summary>
        /// Shows validation errors to the user
        /// </summary>
        /// <param name="validationResult">The validation result containing errors</param>
        /// <param name="title">Title for the error dialog</param>
        public void ShowValidationErrors(ValidationResult validationResult, string title = "Validation Errors")
        {
            if (!validationResult.IsValid && validationResult.Errors.Any())
            {
                var errorMessage = string.Join("\n• ", validationResult.Errors);
                var fullMessage = $"Please correct the following errors:\n\n• {errorMessage}";

                _logger.LogWarning($"Showing validation errors to user: {errorMessage}");

                MessageBox.Show(fullMessage, title, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// Shows a single validation error to the user
        /// </summary>
        /// <param name="errorMessage">The error message to show</param>
        /// <param name="title">Title for the error dialog</param>
        public void ShowValidationError(string errorMessage, string title = "Validation Error")
        {
            _logger.LogWarning($"Showing validation error to user: {errorMessage}");
            MessageBox.Show(errorMessage, title, MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    /// <summary>
    /// Represents the result of a validation operation
    /// </summary>
    public class ValidationResult
    {
        /// <summary>
        /// Gets or sets whether the validation was successful
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// Gets or sets the list of validation errors
        /// </summary>
        public List<string> Errors { get; set; } = new List<string>();

        /// <summary>
        /// Gets the combined error message
        /// </summary>
        public string ErrorMessage => string.Join("; ", Errors);
    }
}
