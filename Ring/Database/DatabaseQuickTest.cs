using System;
using System.Windows;

namespace Ring.Database
{
    /// <summary>
    /// Quick test class to verify SQLite database functionality
    /// </summary>
    public static class DatabaseQuickTest
    {
        /// <summary>
        /// Run a quick test of the database functionality
        /// </summary>
        public static void RunQuickTest()
        {
            try
            {
                // Test 1: Create database service
                var dbService = new SqliteDatabaseService();
                
                // Test 2: Test connection
                bool connected = dbService.TestConnection();
                if (!connected)
                {
                    MessageBox.Show("Database connection test failed!", "Database Test", 
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                
                // Test 3: Initialize database
                dbService.Initialize();
                
                // Test 4: Execute a simple query
                var result = dbService.ExecuteQuery("SELECT COUNT(*) as UserCount FROM Users");
                if (result.Rows.Count > 0)
                {
                    var userCount = result.Rows[0]["UserCount"];
                    MessageBox.Show($"Database test successful!\nUsers in database: {userCount}", 
                                  "Database Test", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Database test completed, but no data found.", 
                                  "Database Test", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                
                // Test 5: Test database example class
                var dbExample = new DatabaseExample();
                var stats = dbExample.GetDatabaseStats();
                MessageBox.Show($"Database Statistics:\n{stats}", 
                              "Database Stats", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database test failed: {ex.Message}", 
                              "Database Test Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

