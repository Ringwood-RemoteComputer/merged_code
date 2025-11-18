using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Reflection;

namespace Ring.Database
{
    public static class AlarmDatabaseHelper
    {
        // Use RingwoodDatabase.db in the project root or bin directory
        private static readonly string _dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "RingwoodDatabase.db");
        private static readonly string _connectionString = $"Data Source={_dbPath};Version=3;";
        
        static AlarmDatabaseHelper()
        {
            // Log the database path for debugging
            Console.WriteLine($"AlarmDatabaseHelper: Database path = {_dbPath}");
            Console.WriteLine($"AlarmDatabaseHelper: Database exists = {File.Exists(_dbPath)}");
            
            // Try to initialize SQLite - this helps with native DLL loading
            try
            {
                // Force SQLite to initialize by creating a test connection
                using (var testConn = new SQLiteConnection(_connectionString))
                {
                    testConn.Open();
                    testConn.Close();
                }
                Console.WriteLine("AlarmDatabaseHelper: SQLite initialized successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AlarmDatabaseHelper: WARNING - SQLite initialization failed: {ex.Message}");
                Console.WriteLine($"AlarmDatabaseHelper: This may be due to missing native DLL (e_sqlite3.dll)");
                Console.WriteLine($"AlarmDatabaseHelper: Try reinstalling System.Data.SQLite package or ensure native DLLs are in output directory");
            }
        }

        public static void CreateTable()
        {
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    
                    var cmd = connection.CreateCommand();
                    cmd.CommandText = @"
                        CREATE TABLE IF NOT EXISTS AlarmRecords (
                            ALMID INTEGER PRIMARY KEY AUTOINCREMENT,
                            ALMNUMBER INTEGER NOT NULL,
                            ALMTIME TEXT,
                            ALMDATE TEXT,
                            ACKTIME TEXT,
                            ACKDATE TEXT,
                            ALMTYPENUMBER INTEGER,
                            ALMSTATUSNUMBER INTEGER,
                            ALMIDTYPE INTEGER,
                            ALMNAME TEXT
                        );
                    ";
                    cmd.ExecuteNonQuery();
                    
                    Console.WriteLine("AlarmRecords table created/verified successfully.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating AlarmRecords table: {ex.Message}");
                throw;
            }
        }

        public static void InsertAlarm(AlarmRecord alarm)
        {
            try
            {
                // Ensure table exists
                CreateTable();
                
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    
                    var cmd = connection.CreateCommand();
                    cmd.CommandText = @"
                        INSERT INTO AlarmRecords 
                        (ALMNUMBER, ALMTIME, ALMDATE, ACKTIME, ACKDATE, ALMTYPENUMBER, ALMSTATUSNUMBER, ALMIDTYPE, ALMNAME)
                        VALUES 
                        (@ALMNUMBER, @ALMTIME, @ALMDATE, @ACKTIME, @ACKDATE, @ALMTYPENUMBER, @ALMSTATUSNUMBER, @ALMIDTYPE, @ALMNAME);
                    ";
                    
                    cmd.Parameters.AddWithValue("@ALMNUMBER", alarm.ALMNUMBER);
                    cmd.Parameters.AddWithValue("@ALMTIME", alarm.ALMTIME ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ALMDATE", alarm.ALMDATE ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ACKTIME", alarm.ACKTIME ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ACKDATE", alarm.ACKDATE ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ALMTYPENUMBER", alarm.ALMTYPENUMBER);
                    cmd.Parameters.AddWithValue("@ALMSTATUSNUMBER", alarm.ALMSTATUSNUMBER);
                    cmd.Parameters.AddWithValue("@ALMIDTYPE", alarm.ALMIDTYPE);
                    cmd.Parameters.AddWithValue("@ALMNAME", alarm.ALMNAME ?? (object)DBNull.Value);
                    
                    int rowsAffected = cmd.ExecuteNonQuery();
                    
                    // Get the last inserted ID
                    cmd.CommandText = "SELECT last_insert_rowid();";
                    long lastId = (long)cmd.ExecuteScalar();
                    
                    Console.WriteLine($"Alarm '{alarm.ALMNAME}' (ALMNUMBER: {alarm.ALMNUMBER}) saved to RingwoodDatabase.db successfully. ALMID: {lastId}");
                    Console.WriteLine($"  - ALMTIME: {alarm.ALMTIME}, ALMDATE: {alarm.ALMDATE}");
                    Console.WriteLine($"  - ALMSTATUSNUMBER: {alarm.ALMSTATUSNUMBER} (1=Active/Unacknowledged)");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inserting alarm to database: {ex.Message}");
                Console.WriteLine($"Database path: {_dbPath}");
                throw;
            }
        }

        public static List<AlarmRecord> GetAlarms()
        {
            var alarms = new List<AlarmRecord>();
            
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    
                    var cmd = connection.CreateCommand();
                    cmd.CommandText = "SELECT * FROM AlarmRecords ORDER BY ALMID DESC;";
                    
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            alarms.Add(new AlarmRecord
                            {
                                ALMID = Convert.ToInt32(reader["ALMID"]),
                                ALMNUMBER = Convert.ToInt32(reader["ALMNUMBER"]),
                                ALMTIME = reader["ALMTIME"]?.ToString(),
                                ALMDATE = reader["ALMDATE"]?.ToString(),
                                ACKTIME = reader["ACKTIME"]?.ToString(),
                                ACKDATE = reader["ACKDATE"]?.ToString(),
                                ALMTYPENUMBER = Convert.ToInt32(reader["ALMTYPENUMBER"]),
                                ALMSTATUSNUMBER = Convert.ToInt32(reader["ALMSTATUSNUMBER"]),
                                ALMIDTYPE = Convert.ToInt32(reader["ALMIDTYPE"]),
                                ALMNAME = reader["ALMNAME"]?.ToString()
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving alarms from database: {ex.Message}");
            }
            
            return alarms;
        }
        
        // Test method to verify database connection and table
        public static void TestDatabaseConnection()
        {
            try
            {
                Console.WriteLine($"=== DATABASE CONNECTION TEST ===");
                Console.WriteLine($"Database path: {_dbPath}");
                Console.WriteLine($"Database exists: {File.Exists(_dbPath)}");
                Console.WriteLine($"Connection string: {_connectionString}");
                
                // Ensure table exists
                CreateTable();
                
                // Test insert
                var testAlarm = new AlarmRecord
                {
                    ALMNUMBER = 999,
                    ALMTIME = DateTime.Now.ToString("HH:mm:ss"),
                    ALMDATE = DateTime.Now.ToString("yyyy-MM-dd"),
                    ACKTIME = null,
                    ACKDATE = null,
                    ALMTYPENUMBER = 1,
                    ALMSTATUSNUMBER = 1,
                    ALMIDTYPE = 1,
                    ALMNAME = "TEST ALARM - Database Connection Test"
                };
                
                InsertAlarm(testAlarm);
                Console.WriteLine("✓ Test alarm inserted successfully");
                
                // Test read
                var alarms = GetAlarms();
                Console.WriteLine($"✓ Retrieved {alarms.Count} alarms from database");
                
                Console.WriteLine($"=== DATABASE TEST COMPLETE ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ DATABASE TEST FAILED: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }
    }
}
