using System;
using System.ComponentModel.DataAnnotations;
using Ring.Validation;

namespace Ring.Models
{
    /// <summary>
    /// Represents filter criteria for batch queries
    /// </summary>
    public class BatchQueryFilter
    {
        /// <summary>
        /// Start date for filtering batches
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// End date for filtering batches
        /// </summary>
        [DateRangeValidation("StartDate", ErrorMessage = "End date must be after start date.")]
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Formula number for filtering batches
        /// </summary>
        [FormulaNumberValidation(ErrorMessage = "Formula number must be between 1 and 9999.")]
        public int? FormulaNumber { get; set; }

        /// <summary>
        /// Storage tank number for filtering batches
        /// </summary>
        [TankNumberValidation(ErrorMessage = "Storage tank number must be between 1 and 50.")]
        public int? StorageTankNumber { get; set; }

        /// <summary>
        /// Status code for filtering batches
        /// </summary>
        [StatusValidation(ErrorMessage = "Status must be a valid status code (0-5).")]
        public int? Status { get; set; }

        /// <summary>
        /// Search text for filtering batches by name or description
        /// </summary>
        [StringLength(100, ErrorMessage = "Search text cannot exceed 100 characters.")]
        public string SearchText { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public BatchQueryFilter()
        {
            SearchText = string.Empty;
        }

        /// <summary>
        /// Constructor with parameters
        /// </summary>
        /// <param name="startDate">Start date filter</param>
        /// <param name="endDate">End date filter</param>
        /// <param name="formulaNumber">Formula number filter</param>
        /// <param name="storageTankNumber">Storage tank number filter</param>
        /// <param name="status">Status filter</param>
        /// <param name="searchText">Search text filter</param>
        public BatchQueryFilter(DateTime? startDate = null, DateTime? endDate = null, 
                               int? formulaNumber = null, int? storageTankNumber = null,
                               int? status = null, string searchText = null)
        {
            StartDate = startDate;
            EndDate = endDate;
            FormulaNumber = formulaNumber;
            StorageTankNumber = storageTankNumber;
            Status = status;
            SearchText = searchText ?? string.Empty;
        }

        /// <summary>
        /// Checks if the filter has any criteria set
        /// </summary>
        public bool HasFilters
        {
            get
            {
                return StartDate.HasValue || EndDate.HasValue || FormulaNumber.HasValue || 
                       StorageTankNumber.HasValue || Status.HasValue || !string.IsNullOrEmpty(SearchText);
            }
        }

        /// <summary>
        /// Clears all filter criteria
        /// </summary>
        public void ClearFilters()
        {
            StartDate = null;
            EndDate = null;
            FormulaNumber = null;
            StorageTankNumber = null;
            Status = null;
            SearchText = string.Empty;
        }

        /// <summary>
        /// Sets a date range filter
        /// </summary>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        public void SetDateRange(DateTime startDate, DateTime endDate)
        {
            StartDate = startDate;
            EndDate = endDate;
        }

        /// <summary>
        /// Validates the filter criteria
        /// </summary>
        /// <returns>True if valid, false otherwise</returns>
        public bool IsValid()
        {
            // Check if start date is before end date
            if (StartDate.HasValue && EndDate.HasValue && StartDate.Value > EndDate.Value)
                return false;

            return true;
        }

        /// <summary>
        /// Returns a string representation of the BatchQueryFilter object
        /// </summary>
        /// <returns>String representation</returns>
        public override string ToString()
        {
            var filters = new System.Collections.Generic.List<string>();

            if (StartDate.HasValue)
                filters.Add($"Start Date: {StartDate.Value:yyyy-MM-dd}");
            if (EndDate.HasValue)
                filters.Add($"End Date: {EndDate.Value:yyyy-MM-dd}");
            if (FormulaNumber.HasValue)
                filters.Add($"Formula Number: {FormulaNumber.Value}");
            if (StorageTankNumber.HasValue)
                filters.Add($"Storage Tank Number: {StorageTankNumber.Value}");
            if (Status.HasValue)
                filters.Add($"Status: {Status.Value}");
            if (!string.IsNullOrEmpty(SearchText))
                filters.Add($"Search: {SearchText}");

            return filters.Count > 0 ? string.Join(", ", filters) : "No filters applied";
        }
    }
}
