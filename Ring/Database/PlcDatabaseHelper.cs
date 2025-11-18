//using Microsoft.Data.Sqlite;
//using System;

//namespace Ring.Database
//{
//    public static class PlcDatabaseHelper
//    {
//        private static readonly string _connStr = "Data Source=plc_data.db";

//        public static void Initialize()
//        {
//            using var conn = new SqliteConnection(_connStr);
//            conn.Open();

//            var cmd = conn.CreateCommand();
//            cmd.CommandText = @"
//                CREATE TABLE IF NOT EXISTS PlcTagData (
//                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
//                    TagName TEXT,
//                    Value TEXT,
//                    Timestamp TEXT
//                );
//            ";
//            cmd.ExecuteNonQuery();
//        }

//        public static void InsertTagValue(string tagName, string value)
//        {
//            using var conn = new SqliteConnection(_connStr);
//            conn.Open();

//            var cmd = conn.CreateCommand();
//            cmd.CommandText = @"
//                INSERT INTO PlcTagData (TagName, Value, Timestamp)
//                VALUES ($name, $value, $time);
//            ";
//            cmd.Parameters.AddWithValue("$name", tagName);
//            cmd.Parameters.AddWithValue("$value", value);
//            cmd.Parameters.AddWithValue("$time", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

//            cmd.ExecuteNonQuery();
//        }
//    }
//}
