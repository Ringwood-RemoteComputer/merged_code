using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ring.Models;
using Ring.Services;

namespace Ring.Services
{
    /// <summary>
    /// Service class for handling batch data queries and operations using in-memory data storage
    /// </summary>
    public class BatchQueryService
    {
        private readonly List<BatchQuery> _batchData;
        private readonly object _lockObject = new object();
        private readonly ILogger _logger;
        private readonly ValidationService _validationService;

        /// <summary>
        /// Initializes a new instance of the BatchQueryService
        /// </summary>
        /// <param name="logger">Logger instance for service operations</param>
        /// <param name="validationService">Validation service for data validation</param>
        public BatchQueryService(ILogger logger = null, ValidationService validationService = null)
        {
            _logger = logger ?? new Logger();
            _validationService = validationService ?? new ValidationService(_logger);
            _batchData = new List<BatchQuery>();
            
            _logger.LogInfo("Initializing BatchQueryService");
            InitializeSampleData();
            _logger.LogInfo($"BatchQueryService initialized with {_batchData.Count} sample records");
        }


        /// <summary>
        /// Gets batches based on the provided filter criteria
        /// </summary>
        /// <param name="filter">Filter criteria for batch queries</param>
        /// <returns>List of batches matching the filter criteria</returns>
        public async Task<List<BatchQuery>> GetBatchesAsync(BatchQueryFilter filter)
        {
            try
            {
                _logger.LogInfo($"Getting batches with filter: {filter}");

                if (filter == null)
                {
                    throw new ArgumentNullException(nameof(filter), "Filter cannot be null");
                }

                if (!filter.IsValid())
                {
                    throw new ArgumentException("Invalid filter criteria", nameof(filter));
                }

                // Simulate async operation
                await Task.Delay(50);

                lock (_lockObject)
                {
                    var batches = _batchData.AsQueryable();

                    // Apply filters
                    if (filter.StartDate.HasValue)
                        batches = batches.Where(b => b.StartDate.Date >= filter.StartDate.Value.Date);
                    if (filter.EndDate.HasValue)
                        batches = batches.Where(b => b.StartDate.Date <= filter.EndDate.Value.Date);
                    if (filter.FormulaNumber.HasValue)
                        batches = batches.Where(b => b.FormulaNumber == filter.FormulaNumber.Value);
                    if (filter.StorageTankNumber.HasValue)
                        batches = batches.Where(b => b.StorageTankNumber == filter.StorageTankNumber.Value);
                    if (filter.Status.HasValue)
                        batches = batches.Where(b => b.Status == filter.Status.Value);
                    if (!string.IsNullOrEmpty(filter.SearchText))
                        batches = batches.Where(b => 
                            b.FormulaName.ToLower().Contains(filter.SearchText.ToLower()) ||
                            b.StorageTankName.ToLower().Contains(filter.SearchText.ToLower()) ||
                            b.StatusText.ToLower().Contains(filter.SearchText.ToLower()));

                    var result = batches.OrderByDescending(b => b.StartDate)
                                       .ThenByDescending(b => b.StartTime)
                                       .ToList();

                    LogInfo($"Retrieved {result.Count} batches");
                    return result;
                }
            }
            catch (Exception ex)
            {
                LogError($"Error in GetBatchesAsync: {ex.Message}", ex);
                throw new InvalidOperationException("An error occurred while retrieving batches", ex);
            }
        }

        /// <summary>
        /// Gets a specific batch by its ID
        /// </summary>
        /// <param name="id">The batch ID</param>
        /// <returns>The batch with the specified ID, or null if not found</returns>
        public async Task<BatchQuery> GetBatchByIdAsync(int id)
        {
            try
            {
                LogInfo($"Getting batch by ID: {id}");

                if (id <= 0)
                {
                    throw new ArgumentException("Batch ID must be greater than 0", nameof(id));
                }

                // Simulate async operation
                await Task.Delay(25);

                lock (_lockObject)
                {
                    var batch = _batchData.FirstOrDefault(b => b.Id == id);
                    if (batch != null)
                    {
                        LogInfo($"Retrieved batch with ID: {id}");
                        return batch;
                    }
                }

                LogInfo($"Batch with ID {id} not found");
                return null;
            }
            catch (Exception ex)
            {
                LogError($"Error in GetBatchByIdAsync: {ex.Message}", ex);
                throw new InvalidOperationException($"An error occurred while retrieving batch with ID {id}", ex);
            }
        }

        /// <summary>
        /// Gets batch history within the specified date range
        /// </summary>
        /// <param name="startDate">Start date for the history query</param>
        /// <param name="endDate">End date for the history query</param>
        /// <returns>List of batches within the specified date range</returns>
        public async Task<List<BatchQuery>> GetBatchHistoryAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                LogInfo($"Getting batch history from {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}");

                if (startDate > endDate)
                {
                    throw new ArgumentException("Start date cannot be greater than end date");
                }

                // Simulate async operation
                await Task.Delay(40);

                lock (_lockObject)
                {
                    var batches = _batchData.Where(b => 
                        b.StartDate.Date >= startDate.Date && 
                        b.StartDate.Date <= endDate.Date)
                        .OrderByDescending(b => b.StartDate)
                        .ThenByDescending(b => b.StartTime)
                        .ToList();

                    LogInfo($"Retrieved {batches.Count} batches from history");
                    return batches;
                }
            }
            catch (Exception ex)
            {
                LogError($"Error in GetBatchHistoryAsync: {ex.Message}", ex);
                throw new InvalidOperationException("An error occurred while retrieving batch history", ex);
            }
        }

        /// <summary>
        /// Gets batch statistics within the specified date range
        /// </summary>
        /// <param name="startDate">Start date for the statistics query</param>
        /// <param name="endDate">End date for the statistics query</param>
        /// <returns>Dictionary containing various batch statistics</returns>
        public async Task<Dictionary<string, object>> GetBatchStatisticsAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                LogInfo($"Getting batch statistics from {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}");

                if (startDate > endDate)
                {
                    throw new ArgumentException("Start date cannot be greater than end date");
                }

                // Simulate async operation
                await Task.Delay(60);

                lock (_lockObject)
                {
                    var batches = _batchData.Where(b => 
                        b.StartDate.Date >= startDate.Date && 
                        b.StartDate.Date <= endDate.Date).ToList();

                    var statistics = new Dictionary<string, object>
                    {
                        ["TotalBatches"] = batches.Count,
                        ["CompletedBatches"] = batches.Count(b => b.Status == 1),
                        ["InProgressBatches"] = batches.Count(b => b.Status == 2),
                        ["FailedBatches"] = batches.Count(b => b.Status == 3),
                        ["AverageVolume"] = batches.Any() ? batches.Average(b => b.Volume) : 0,
                        ["TotalVolume"] = batches.Sum(b => b.Volume),
                        ["AverageTemperature"] = batches.Any() ? batches.Average(b => b.Temperature) : 0,
                        ["MinTemperature"] = batches.Any() ? batches.Min(b => b.Temperature) : 0,
                        ["MaxTemperature"] = batches.Any() ? batches.Max(b => b.Temperature) : 0,
                        ["AverageWeightDifference"] = batches.Any() ? batches.Average(b => b.EndWeight - b.StartWeight) : 0,
                        ["UniqueFormulas"] = batches.Select(b => b.FormulaNumber).Distinct().Count(),
                        ["UniqueTanks"] = batches.Select(b => b.StorageTankNumber).Distinct().Count()
                    };

                    LogInfo($"Retrieved batch statistics: {statistics.Count} metrics");
                    return statistics;
                }
            }
            catch (Exception ex)
            {
                LogError($"Error in GetBatchStatisticsAsync: {ex.Message}", ex);
                throw new InvalidOperationException("An error occurred while retrieving batch statistics", ex);
            }
        }

        /// <summary>
        /// Gets list of available formula numbers from the database
        /// </summary>
        /// <returns>List of unique formula numbers</returns>
        public async Task<List<int>> GetAvailableFormulaNumbersAsync()
        {
            try
            {
                LogInfo("Getting available formula numbers");

                // Simulate async operation
                await Task.Delay(30);

                lock (_lockObject)
                {
                    var formulaNumbers = _batchData.Select(b => b.FormulaNumber)
                                                 .Distinct()
                                                 .OrderBy(n => n)
                                                 .ToList();

                    LogInfo($"Retrieved {formulaNumbers.Count} unique formula numbers");
                    return formulaNumbers;
                }
            }
            catch (Exception ex)
            {
                LogError($"Error in GetAvailableFormulaNumbersAsync: {ex.Message}", ex);
                throw new InvalidOperationException("An error occurred while retrieving formula numbers", ex);
            }
        }

        /// <summary>
        /// Gets list of available storage tank numbers from the database
        /// </summary>
        /// <returns>List of unique storage tank numbers</returns>
        public async Task<List<int>> GetAvailableStorageTankNumbersAsync()
        {
            try
            {
                LogInfo("Getting available storage tank numbers");

                // Simulate async operation
                await Task.Delay(30);

                lock (_lockObject)
                {
                    var tankNumbers = _batchData.Select(b => b.StorageTankNumber)
                                               .Distinct()
                                               .OrderBy(n => n)
                                               .ToList();

                    LogInfo($"Retrieved {tankNumbers.Count} unique storage tank numbers");
                    return tankNumbers;
                }
            }
            catch (Exception ex)
            {
                LogError($"Error in GetAvailableStorageTankNumbersAsync: {ex.Message}", ex);
                throw new InvalidOperationException("An error occurred while retrieving storage tank numbers", ex);
            }
        }

        /// <summary>
        /// Initializes sample data for testing purposes
        /// </summary>
        private void InitializeSampleData()
        {
            try
            {
                var sampleData = GetSampleBatchData();
                _batchData.AddRange(sampleData);
                LogInfo($"Initialized {sampleData.Count} sample batch records");
            }
            catch (Exception ex)
            {
                LogError($"Error initializing sample data: {ex.Message}", ex);
                throw;
            }
        }

        /// <summary>
        /// Generates sample batch data for testing purposes
        /// </summary>
        private List<BatchQuery> GetSampleBatchData()
        {
            var sampleBatches = new List<BatchQuery>
            {
                new BatchQuery(1, "Standard Make Ready", 1, 
                    new DateTime(2024, 1, 15), new TimeSpan(8, 0, 0),
                    new DateTime(2024, 1, 15), new TimeSpan(10, 30, 0),
                    "Storage Tank 1", 1, 1000.5m, 65.2m, 1, "Completed", 500.0m, 1500.5m),

                new BatchQuery(2, "High Temperature Mix", 2,
                    new DateTime(2024, 1, 15), new TimeSpan(11, 0, 0),
                    new DateTime(2024, 1, 15), new TimeSpan(13, 45, 0),
                    "Storage Tank 2", 2, 1200.0m, 75.8m, 1, "Completed", 600.0m, 1800.0m),

                new BatchQuery(3, "Low pH Formula", 3,
                    new DateTime(2024, 1, 16), new TimeSpan(9, 15, 0),
                    new DateTime(2024, 1, 16), new TimeSpan(11, 20, 0),
                    "Storage Tank 1", 1, 950.3m, 60.5m, 1, "Completed", 450.0m, 1400.3m),

                new BatchQuery(4, "Custom Blend", 4,
                    new DateTime(2024, 1, 16), new TimeSpan(14, 0, 0),
                    new DateTime(2024, 1, 16), new TimeSpan(16, 30, 0),
                    "Storage Tank 3", 3, 1100.7m, 70.1m, 2, "In Progress", 550.0m, 1650.7m),

                new BatchQuery(5, "Emergency Mix", 5,
                    new DateTime(2024, 1, 17), new TimeSpan(7, 30, 0),
                    new DateTime(2024, 1, 17), new TimeSpan(9, 45, 0),
                    "Storage Tank 2", 2, 800.2m, 68.9m, 1, "Completed", 400.0m, 1200.2m),

                new BatchQuery(6, "Standard Make Ready", 1,
                    new DateTime(2024, 1, 17), new TimeSpan(10, 0, 0),
                    new DateTime(2024, 1, 17), new TimeSpan(12, 15, 0),
                    "Storage Tank 1", 1, 1050.8m, 66.3m, 1, "Completed", 525.0m, 1575.8m),

                new BatchQuery(7, "High Temperature Mix", 2,
                    new DateTime(2024, 1, 18), new TimeSpan(8, 45, 0),
                    new DateTime(2024, 1, 18), new TimeSpan(11, 0, 0),
                    "Storage Tank 3", 3, 1150.4m, 77.2m, 1, "Completed", 575.0m, 1725.4m),

                new BatchQuery(8, "Low pH Formula", 3,
                    new DateTime(2024, 1, 18), new TimeSpan(13, 30, 0),
                    new DateTime(2024, 1, 18), new TimeSpan(15, 45, 0),
                    "Storage Tank 2", 2, 900.6m, 62.1m, 3, "Failed", 450.0m, 1350.6m),

                new BatchQuery(9, "Custom Blend", 4,
                    new DateTime(2024, 1, 19), new TimeSpan(9, 0, 0),
                    new DateTime(2024, 1, 19), new TimeSpan(11, 30, 0),
                    "Storage Tank 1", 1, 1080.9m, 71.5m, 1, "Completed", 540.0m, 1620.9m),

                new BatchQuery(10, "Emergency Mix", 5,
                    new DateTime(2024, 1, 19), new TimeSpan(15, 0, 0),
                    new DateTime(2024, 1, 19), new TimeSpan(17, 15, 0),
                    "Storage Tank 3", 3, 850.1m, 69.8m, 2, "In Progress", 425.0m, 1275.1m)
            };

            return sampleBatches;
        }

        /// <summary>
        /// Logs an informational message
        /// </summary>
        private void LogInfo(string message)
        {
            System.Diagnostics.Debug.WriteLine($"[INFO] BatchQueryService: {message}");
        }

        /// <summary>
        /// Logs an error message with exception details
        /// </summary>
        private void LogError(string message, Exception ex = null)
        {
            System.Diagnostics.Debug.WriteLine($"[ERROR] BatchQueryService: {message}");
            if (ex != null)
            {
                System.Diagnostics.Debug.WriteLine($"[ERROR] Exception: {ex}");
            }
        }

        #region SQL Query Building Methods

        /// <summary>
        /// Gets batches with advanced filtering, sorting, and pagination using SQL query building
        /// </summary>
        /// <param name="filter">Filter criteria for batch queries</param>
        /// <param name="sortOptions">Sorting options</param>
        /// <param name="pagination">Pagination options</param>
        /// <returns>List of batches matching the criteria</returns>
        public async Task<List<BatchQuery>> GetBatchesAdvancedAsync(BatchQueryFilter filter, 
            SortOptions sortOptions = null, PaginationOptions pagination = null)
        {
            try
            {
                LogInfo($"Getting batches with advanced filtering, sorting, and pagination");

                if (filter == null)
                {
                    throw new ArgumentNullException(nameof(filter), "Filter cannot be null");
                }

                if (!filter.IsValid())
                {
                    throw new ArgumentException("Invalid filter criteria", nameof(filter));
                }

                // Simulate async operation
                await Task.Delay(50);

                // Build SQL query using extension method
                var sqlQuery = this.BuildFilterQuery(filter, out var parameters, sortOptions, pagination);
                var countQuery = this.BuildCountQuery(filter, out var countParameters);

                LogInfo($"Generated SQL Query: {sqlQuery}");
                LogInfo($"Generated Count Query: {countQuery}");
                LogInfo($"Total Parameters: {parameters.Count}");

                // In a real implementation, you would execute these queries against the database
                // For now, we'll simulate the results using the existing in-memory data
                lock (_lockObject)
                {
                    var batches = _batchData.AsQueryable();

                    // Apply filters (simulating SQL WHERE clause)
                    if (filter.StartDate.HasValue)
                        batches = batches.Where(b => b.StartDate.Date >= filter.StartDate.Value.Date);
                    if (filter.EndDate.HasValue)
                        batches = batches.Where(b => b.StartDate.Date <= filter.EndDate.Value.Date);
                    if (filter.FormulaNumber.HasValue)
                        batches = batches.Where(b => b.FormulaNumber == filter.FormulaNumber.Value);
                    if (filter.StorageTankNumber.HasValue)
                        batches = batches.Where(b => b.StorageTankNumber == filter.StorageTankNumber.Value);
                    if (filter.Status.HasValue)
                        batches = batches.Where(b => b.Status == filter.Status.Value);
                    if (!string.IsNullOrEmpty(filter.SearchText))
                        batches = batches.Where(b => 
                            b.FormulaName.ToLower().Contains(filter.SearchText.ToLower()) ||
                            b.StorageTankName.ToLower().Contains(filter.SearchText.ToLower()) ||
                            b.StatusText.ToLower().Contains(filter.SearchText.ToLower()));

                    // Apply sorting (simulating SQL ORDER BY clause)
                    if (sortOptions != null && sortOptions.SortFields.Count > 0)
                    {
                        IOrderedQueryable<BatchQuery> orderedBatches = null;
                        bool isFirst = true;

                        foreach (var sortField in sortOptions.SortFields)
                        {
                            if (isFirst)
                            {
                                orderedBatches = ApplySorting(batches, sortField, true);
                                isFirst = false;
                            }
                            else
                            {
                                orderedBatches = ApplyThenBySorting(orderedBatches, sortField);
                            }
                        }

                        batches = orderedBatches ?? batches;
                    }
                    else
                    {
                        // Default sorting
                        batches = batches.OrderByDescending(b => b.StartDate).ThenByDescending(b => b.StartTime);
                    }

                    // Apply pagination (simulating SQL LIMIT and OFFSET)
                    if (pagination != null)
                    {
                        batches = batches.Skip(pagination.Offset).Take(pagination.PageSize);
                    }

                    var result = batches.ToList();

                    // Set pagination info
                    if (pagination != null)
                    {
                        // In a real implementation, you would get this from the count query
                        pagination.TotalRecords = _batchData.Count;
                    }

                    LogInfo($"Retrieved {result.Count} batches with advanced filtering");
                    return result;
                }
            }
            catch (Exception ex)
            {
                LogError($"Error in GetBatchesAdvancedAsync: {ex.Message}", ex);
                throw new InvalidOperationException("An error occurred while retrieving batches with advanced filtering", ex);
            }
        }

        /// <summary>
        /// Gets database index creation scripts for performance optimization
        /// </summary>
        /// <returns>List of index creation SQL scripts</returns>
        public List<string> GetDatabaseIndexScripts()
        {
            return BatchQueryServiceExtensions.GetDatabaseIndexScripts();
        }

        /// <summary>
        /// Applies initial sorting to a queryable collection
        /// </summary>
        /// <param name="batches">Queryable collection</param>
        /// <param name="sortField">Sort field option</param>
        /// <param name="isFirst">Whether this is the first sort field</param>
        /// <returns>Ordered queryable collection</returns>
        private IOrderedQueryable<BatchQuery> ApplySorting(IQueryable<BatchQuery> batches, SortFieldOption sortField, bool isFirst)
        {
            return sortField.Field switch
            {
                SortField.StartDate => sortField.Direction == SortDirection.Ascending 
                    ? batches.OrderBy(b => b.StartDate) 
                    : batches.OrderByDescending(b => b.StartDate),
                SortField.EndDate => sortField.Direction == SortDirection.Ascending 
                    ? batches.OrderBy(b => b.EndDate) 
                    : batches.OrderByDescending(b => b.EndDate),
                SortField.FormulaNumber => sortField.Direction == SortDirection.Ascending 
                    ? batches.OrderBy(b => b.FormulaNumber) 
                    : batches.OrderByDescending(b => b.FormulaNumber),
                SortField.FormulaName => sortField.Direction == SortDirection.Ascending 
                    ? batches.OrderBy(b => b.FormulaName) 
                    : batches.OrderByDescending(b => b.FormulaName),
                SortField.StorageTankNumber => sortField.Direction == SortDirection.Ascending 
                    ? batches.OrderBy(b => b.StorageTankNumber) 
                    : batches.OrderByDescending(b => b.StorageTankNumber),
                SortField.StorageTankName => sortField.Direction == SortDirection.Ascending 
                    ? batches.OrderBy(b => b.StorageTankName) 
                    : batches.OrderByDescending(b => b.StorageTankName),
                SortField.Volume => sortField.Direction == SortDirection.Ascending 
                    ? batches.OrderBy(b => b.Volume) 
                    : batches.OrderByDescending(b => b.Volume),
                SortField.Temperature => sortField.Direction == SortDirection.Ascending 
                    ? batches.OrderBy(b => b.Temperature) 
                    : batches.OrderByDescending(b => b.Temperature),
                SortField.Status => sortField.Direction == SortDirection.Ascending 
                    ? batches.OrderBy(b => b.Status) 
                    : batches.OrderByDescending(b => b.Status),
                SortField.StartWeight => sortField.Direction == SortDirection.Ascending 
                    ? batches.OrderBy(b => b.StartWeight) 
                    : batches.OrderByDescending(b => b.StartWeight),
                SortField.EndWeight => sortField.Direction == SortDirection.Ascending 
                    ? batches.OrderBy(b => b.EndWeight) 
                    : batches.OrderByDescending(b => b.EndWeight),
                _ => batches.OrderByDescending(b => b.StartDate)
            };
        }

        /// <summary>
        /// Applies additional sorting to an already ordered queryable collection
        /// </summary>
        /// <param name="batches">Ordered queryable collection</param>
        /// <param name="sortField">Sort field option</param>
        /// <returns>Ordered queryable collection</returns>
        private IOrderedQueryable<BatchQuery> ApplyThenBySorting(IOrderedQueryable<BatchQuery> batches, SortFieldOption sortField)
        {
            return sortField.Field switch
            {
                SortField.StartDate => sortField.Direction == SortDirection.Ascending 
                    ? batches.ThenBy(b => b.StartDate) 
                    : batches.ThenByDescending(b => b.StartDate),
                SortField.EndDate => sortField.Direction == SortDirection.Ascending 
                    ? batches.ThenBy(b => b.EndDate) 
                    : batches.ThenByDescending(b => b.EndDate),
                SortField.FormulaNumber => sortField.Direction == SortDirection.Ascending 
                    ? batches.ThenBy(b => b.FormulaNumber) 
                    : batches.ThenByDescending(b => b.FormulaNumber),
                SortField.FormulaName => sortField.Direction == SortDirection.Ascending 
                    ? batches.ThenBy(b => b.FormulaName) 
                    : batches.ThenByDescending(b => b.FormulaName),
                SortField.StorageTankNumber => sortField.Direction == SortDirection.Ascending 
                    ? batches.ThenBy(b => b.StorageTankNumber) 
                    : batches.ThenByDescending(b => b.StorageTankNumber),
                SortField.StorageTankName => sortField.Direction == SortDirection.Ascending 
                    ? batches.ThenBy(b => b.StorageTankName) 
                    : batches.ThenByDescending(b => b.StorageTankName),
                SortField.Volume => sortField.Direction == SortDirection.Ascending 
                    ? batches.ThenBy(b => b.Volume) 
                    : batches.ThenByDescending(b => b.Volume),
                SortField.Temperature => sortField.Direction == SortDirection.Ascending 
                    ? batches.ThenBy(b => b.Temperature) 
                    : batches.ThenByDescending(b => b.Temperature),
                SortField.Status => sortField.Direction == SortDirection.Ascending 
                    ? batches.ThenBy(b => b.Status) 
                    : batches.ThenByDescending(b => b.Status),
                SortField.StartWeight => sortField.Direction == SortDirection.Ascending 
                    ? batches.ThenBy(b => b.StartWeight) 
                    : batches.ThenByDescending(b => b.StartWeight),
                SortField.EndWeight => sortField.Direction == SortDirection.Ascending 
                    ? batches.ThenBy(b => b.EndWeight) 
                    : batches.ThenByDescending(b => b.EndWeight),
                _ => batches.ThenByDescending(b => b.StartDate)
            };
        }

        #endregion
    }
}