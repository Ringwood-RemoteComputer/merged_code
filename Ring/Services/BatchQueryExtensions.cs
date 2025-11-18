using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Ring.Models;

namespace Ring.Services
{
    #region Supporting Classes

    /// <summary>
    /// Options for sorting query results
    /// </summary>
    public class SortOptions
    {
        /// <summary>
        /// List of sort fields with their directions
        /// </summary>
        public List<SortFieldOption> SortFields { get; set; } = new List<SortFieldOption>();

        /// <summary>
        /// Adds a sort field to the options
        /// </summary>
        /// <param name="field">Field to sort by</param>
        /// <param name="direction">Sort direction</param>
        public void AddSortField(SortField field, SortDirection direction = SortDirection.Descending)
        {
            SortFields.Add(new SortFieldOption { Field = field, Direction = direction });
        }

        /// <summary>
        /// Clears all sort fields
        /// </summary>
        public void ClearSortFields()
        {
            SortFields.Clear();
        }
    }

    /// <summary>
    /// Individual sort field option
    /// </summary>
    public class SortFieldOption
    {
        /// <summary>
        /// Field to sort by
        /// </summary>
        public SortField Field { get; set; }

        /// <summary>
        /// Sort direction
        /// </summary>
        public SortDirection Direction { get; set; }
    }

    /// <summary>
    /// Available sort fields
    /// </summary>
    public enum SortField
    {
        StartDate,
        EndDate,
        FormulaNumber,
        FormulaName,
        StorageTankNumber,
        StorageTankName,
        Volume,
        Temperature,
        Status,
        StartWeight,
        EndWeight
    }

    /// <summary>
    /// Sort direction options
    /// </summary>
    public enum SortDirection
    {
        Ascending,
        Descending
    }

    /// <summary>
    /// Options for pagination
    /// </summary>
    public class PaginationOptions
    {
        /// <summary>
        /// Page number (1-based)
        /// </summary>
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// Number of items per page
        /// </summary>
        public int PageSize { get; set; } = 50;

        /// <summary>
        /// Calculated offset for SQL queries
        /// </summary>
        public int Offset => (PageNumber - 1) * PageSize;

        /// <summary>
        /// Total number of records (set after query execution)
        /// </summary>
        public int TotalRecords { get; set; }

        /// <summary>
        /// Total number of pages (calculated)
        /// </summary>
        public int TotalPages => (int)Math.Ceiling((double)TotalRecords / PageSize);

        /// <summary>
        /// Whether there is a next page
        /// </summary>
        public bool HasNextPage => PageNumber < TotalPages;

        /// <summary>
        /// Whether there is a previous page
        /// </summary>
        public bool HasPreviousPage => PageNumber > 1;
    }

    /// <summary>
    /// Mock database parameter for demonstration purposes
    /// </summary>
    public class MockDbParameter : IDbDataParameter
    {
        public MockDbParameter(string parameterName, object value)
        {
            ParameterName = parameterName;
            Value = value;
        }

        public byte Precision { get; set; }
        public byte Scale { get; set; }
        public int Size { get; set; }
        public DbType DbType { get; set; }
        public ParameterDirection Direction { get; set; }
        public bool IsNullable { get; set; }
        public string ParameterName { get; set; }
        public string SourceColumn { get; set; }
        public DataRowVersion SourceVersion { get; set; }
        public object Value { get; set; }
    }

    #endregion

    #region SQL Query Builder Extension Methods

    /// <summary>
    /// Extension methods for BatchQueryService to provide SQL query building capabilities
    /// </summary>
    public static class BatchQueryServiceExtensions
    {
        /// <summary>
        /// Builds a dynamic SQL WHERE clause based on filter criteria with parameterized queries
        /// </summary>
        /// <param name="service">BatchQueryService instance</param>
        /// <param name="filter">Filter criteria for batch queries</param>
        /// <param name="parameters">Output parameter collection for SQL parameters</param>
        /// <param name="sortOptions">Sorting options for the query</param>
        /// <param name="pagination">Pagination options for the query</param>
        /// <returns>Complete SQL query with WHERE clause, ORDER BY, and pagination</returns>
        public static string BuildFilterQuery(this BatchQueryService service, BatchQueryFilter filter, 
            out List<IDbDataParameter> parameters, SortOptions sortOptions = null, PaginationOptions pagination = null)
        {
            parameters = new List<IDbDataParameter>();
            var queryBuilder = new StringBuilder();
            var whereConditions = new List<string>();
            var parameterIndex = 0;

            // Base query with performance optimization indexes
            queryBuilder.AppendLine("SELECT Id, FormulaName, FormulaNumber, StartDate, StartTime, EndDate, EndTime,");
            queryBuilder.AppendLine("       StorageTankName, StorageTankNumber, Volume, Temperature, Status, StatusText,");
            queryBuilder.AppendLine("       StartWeight, EndWeight");
            queryBuilder.AppendLine("FROM BatchQueries");

            // Build WHERE conditions
            if (filter != null)
            {
                // Date range filters with index optimization
                if (filter.StartDate.HasValue)
                {
                    whereConditions.Add($"StartDate >= @param{parameterIndex}");
                    parameters.Add(CreateParameter($"@param{parameterIndex}", filter.StartDate.Value.Date));
                    parameterIndex++;
                }

                if (filter.EndDate.HasValue)
                {
                    whereConditions.Add($"StartDate <= @param{parameterIndex}");
                    parameters.Add(CreateParameter($"@param{parameterIndex}", filter.EndDate.Value.Date.AddDays(1).AddTicks(-1)));
                    parameterIndex++;
                }

                // Formula number filter with index optimization
                if (filter.FormulaNumber.HasValue)
                {
                    whereConditions.Add($"FormulaNumber = @param{parameterIndex}");
                    parameters.Add(CreateParameter($"@param{parameterIndex}", filter.FormulaNumber.Value));
                    parameterIndex++;
                }

                // Storage tank number filter with index optimization
                if (filter.StorageTankNumber.HasValue)
                {
                    whereConditions.Add($"StorageTankNumber = @param{parameterIndex}");
                    parameters.Add(CreateParameter($"@param{parameterIndex}", filter.StorageTankNumber.Value));
                    parameterIndex++;
                }

                // Status filter with index optimization
                if (filter.Status.HasValue)
                {
                    whereConditions.Add($"Status = @param{parameterIndex}");
                    parameters.Add(CreateParameter($"@param{parameterIndex}", filter.Status.Value));
                    parameterIndex++;
                }

                // Text search across multiple fields with full-text search optimization
                if (!string.IsNullOrEmpty(filter.SearchText))
                {
                    var searchText = $"%{filter.SearchText}%";
                    whereConditions.Add($"(FormulaName LIKE @param{parameterIndex} OR " +
                                      $"StorageTankName LIKE @param{parameterIndex + 1} OR " +
                                      $"StatusText LIKE @param{parameterIndex + 2})");
                    parameters.Add(CreateParameter($"@param{parameterIndex}", searchText));
                    parameters.Add(CreateParameter($"@param{parameterIndex + 1}", searchText));
                    parameters.Add(CreateParameter($"@param{parameterIndex + 2}", searchText));
                    parameterIndex += 3;
                }
            }

            // Add WHERE clause if conditions exist
            if (whereConditions.Count > 0)
            {
                queryBuilder.AppendLine("WHERE " + string.Join(" AND ", whereConditions));
            }

            // Add ORDER BY clause with performance optimization
            if (sortOptions != null)
            {
                queryBuilder.AppendLine(BuildOrderByClause(sortOptions));
            }
            else
            {
                // Default sorting by start date descending (indexed column)
                queryBuilder.AppendLine("ORDER BY StartDate DESC, StartTime DESC");
            }

            // Add pagination with LIMIT and OFFSET
            if (pagination != null)
            {
                queryBuilder.AppendLine($"LIMIT @param{parameterIndex} OFFSET @param{parameterIndex + 1}");
                parameters.Add(CreateParameter($"@param{parameterIndex}", pagination.PageSize));
                parameters.Add(CreateParameter($"@param{parameterIndex + 1}", pagination.Offset));
            }

            LogQueryInfo($"Built SQL query: {queryBuilder.ToString()}");
            LogQueryInfo($"Parameters: {string.Join(", ", parameters.Select(p => $"{p.ParameterName}={p.Value}"))}");

            return queryBuilder.ToString();
        }

        /// <summary>
        /// Builds a count query for pagination support
        /// </summary>
        /// <param name="service">BatchQueryService instance</param>
        /// <param name="filter">Filter criteria for batch queries</param>
        /// <param name="parameters">Output parameter collection for SQL parameters</param>
        /// <returns>SQL count query</returns>
        public static string BuildCountQuery(this BatchQueryService service, BatchQueryFilter filter, 
            out List<IDbDataParameter> parameters)
        {
            parameters = new List<IDbDataParameter>();
            var queryBuilder = new StringBuilder();
            var whereConditions = new List<string>();
            var parameterIndex = 0;

            queryBuilder.AppendLine("SELECT COUNT(*) FROM BatchQueries");

            // Build WHERE conditions (same logic as BuildFilterQuery)
            if (filter != null)
            {
                if (filter.StartDate.HasValue)
                {
                    whereConditions.Add($"StartDate >= @param{parameterIndex}");
                    parameters.Add(CreateParameter($"@param{parameterIndex}", filter.StartDate.Value.Date));
                    parameterIndex++;
                }

                if (filter.EndDate.HasValue)
                {
                    whereConditions.Add($"StartDate <= @param{parameterIndex}");
                    parameters.Add(CreateParameter($"@param{parameterIndex}", filter.EndDate.Value.Date.AddDays(1).AddTicks(-1)));
                    parameterIndex++;
                }

                if (filter.FormulaNumber.HasValue)
                {
                    whereConditions.Add($"FormulaNumber = @param{parameterIndex}");
                    parameters.Add(CreateParameter($"@param{parameterIndex}", filter.FormulaNumber.Value));
                    parameterIndex++;
                }

                if (filter.StorageTankNumber.HasValue)
                {
                    whereConditions.Add($"StorageTankNumber = @param{parameterIndex}");
                    parameters.Add(CreateParameter($"@param{parameterIndex}", filter.StorageTankNumber.Value));
                    parameterIndex++;
                }

                if (filter.Status.HasValue)
                {
                    whereConditions.Add($"Status = @param{parameterIndex}");
                    parameters.Add(CreateParameter($"@param{parameterIndex}", filter.Status.Value));
                    parameterIndex++;
                }

                if (!string.IsNullOrEmpty(filter.SearchText))
                {
                    var searchText = $"%{filter.SearchText}%";
                    whereConditions.Add($"(FormulaName LIKE @param{parameterIndex} OR " +
                                      $"StorageTankName LIKE @param{parameterIndex + 1} OR " +
                                      $"StatusText LIKE @param{parameterIndex + 2})");
                    parameters.Add(CreateParameter($"@param{parameterIndex}", searchText));
                    parameters.Add(CreateParameter($"@param{parameterIndex + 1}", searchText));
                    parameters.Add(CreateParameter($"@param{parameterIndex + 2}", searchText));
                    parameterIndex += 3;
                }
            }

            if (whereConditions.Count > 0)
            {
                queryBuilder.AppendLine("WHERE " + string.Join(" AND ", whereConditions));
            }

            return queryBuilder.ToString();
        }

        /// <summary>
        /// Builds ORDER BY clause based on sort options with performance optimization
        /// </summary>
        /// <param name="sortOptions">Sorting options</param>
        /// <returns>ORDER BY clause</returns>
        private static string BuildOrderByClause(SortOptions sortOptions)
        {
            if (sortOptions == null || sortOptions.SortFields.Count == 0)
            {
                return "ORDER BY StartDate DESC, StartTime DESC";
            }

            var orderByClause = new StringBuilder("ORDER BY ");
            var sortClauses = new List<string>();

            foreach (var sortField in sortOptions.SortFields)
            {
                var direction = sortField.Direction == SortDirection.Ascending ? "ASC" : "DESC";
                var columnName = GetColumnName(sortField.Field);
                sortClauses.Add($"{columnName} {direction}");
            }

            orderByClause.Append(string.Join(", ", sortClauses));
            return orderByClause.ToString();
        }

        /// <summary>
        /// Maps sort field enum to actual database column name
        /// </summary>
        /// <param name="sortField">Sort field enum</param>
        /// <returns>Database column name</returns>
        private static string GetColumnName(SortField sortField)
        {
            return sortField switch
            {
                SortField.StartDate => "StartDate",
                SortField.EndDate => "EndDate",
                SortField.FormulaNumber => "FormulaNumber",
                SortField.FormulaName => "FormulaName",
                SortField.StorageTankNumber => "StorageTankNumber",
                SortField.StorageTankName => "StorageTankName",
                SortField.Volume => "Volume",
                SortField.Temperature => "Temperature",
                SortField.Status => "Status",
                SortField.StartWeight => "StartWeight",
                SortField.EndWeight => "EndWeight",
                _ => "StartDate"
            };
        }

        /// <summary>
        /// Creates a database parameter (placeholder for actual database implementation)
        /// </summary>
        /// <param name="parameterName">Parameter name</param>
        /// <param name="value">Parameter value</param>
        /// <returns>Database parameter</returns>
        private static IDbDataParameter CreateParameter(string parameterName, object value)
        {
            // This is a placeholder implementation
            // In a real implementation, you would create actual database parameters
            // based on your database provider (SQLite, SQL Server, etc.)
            return new MockDbParameter(parameterName, value);
        }

        /// <summary>
        /// Logs query information for debugging
        /// </summary>
        /// <param name="message">Message to log</param>
        private static void LogQueryInfo(string message)
        {
            System.Diagnostics.Debug.WriteLine($"[INFO] BatchQueryService: {message}");
        }

        /// <summary>
        /// Generates database index creation scripts for performance optimization
        /// </summary>
        /// <returns>List of index creation SQL scripts</returns>
        public static List<string> GetDatabaseIndexScripts()
        {
            return new List<string>
            {
                // Primary index on Id (usually auto-created)
                "CREATE INDEX IF NOT EXISTS IX_BatchQueries_Id ON BatchQueries(Id);",
                
                // Date range queries optimization
                "CREATE INDEX IF NOT EXISTS IX_BatchQueries_StartDate ON BatchQueries(StartDate);",
                "CREATE INDEX IF NOT EXISTS IX_BatchQueries_EndDate ON BatchQueries(EndDate);",
                
                // Filter queries optimization
                "CREATE INDEX IF NOT EXISTS IX_BatchQueries_FormulaNumber ON BatchQueries(FormulaNumber);",
                "CREATE INDEX IF NOT EXISTS IX_BatchQueries_StorageTankNumber ON BatchQueries(StorageTankNumber);",
                "CREATE INDEX IF NOT EXISTS IX_BatchQueries_Status ON BatchQueries(Status);",
                
                // Composite indexes for common query patterns
                "CREATE INDEX IF NOT EXISTS IX_BatchQueries_DateRange ON BatchQueries(StartDate, EndDate);",
                "CREATE INDEX IF NOT EXISTS IX_BatchQueries_FormulaDate ON BatchQueries(FormulaNumber, StartDate);",
                "CREATE INDEX IF NOT EXISTS IX_BatchQueries_TankDate ON BatchQueries(StorageTankNumber, StartDate);",
                "CREATE INDEX IF NOT EXISTS IX_BatchQueries_StatusDate ON BatchQueries(Status, StartDate);",
                
                // Full-text search optimization (if supported by database)
                "CREATE INDEX IF NOT EXISTS IX_BatchQueries_FormulaName ON BatchQueries(FormulaName);",
                "CREATE INDEX IF NOT EXISTS IX_BatchQueries_StorageTankName ON BatchQueries(StorageTankName);",
                "CREATE INDEX IF NOT EXISTS IX_BatchQueries_StatusText ON BatchQueries(StatusText);",
                
                // Sorting optimization
                "CREATE INDEX IF NOT EXISTS IX_BatchQueries_Volume ON BatchQueries(Volume);",
                "CREATE INDEX IF NOT EXISTS IX_BatchQueries_Temperature ON BatchQueries(Temperature);",
                "CREATE INDEX IF NOT EXISTS IX_BatchQueries_StartWeight ON BatchQueries(StartWeight);",
                "CREATE INDEX IF NOT EXISTS IX_BatchQueries_EndWeight ON BatchQueries(EndWeight);"
            };
        }
    }

    #endregion
}





