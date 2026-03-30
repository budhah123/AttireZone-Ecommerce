using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace AttireZone_Web_App.DataAccess
{
    public class DBHelper
    {
        
        private static readonly string _connectionString =
            ConfigurationManager.ConnectionStrings["AttireZone"].ConnectionString;

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        // Execute non-query (INSERT, UPDATE, DELETE) - returns rows affected
        public static int ExecuteNonQuery(string sql, SqlParameter[] parameters = null)
        {
            using (var conn = GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.CommandType = CommandType.Text;
                if (parameters != null) cmd.Parameters.AddRange(parameters);
                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        // Execute scalar (returns single value)
        public static object ExecuteScalar(string sql, SqlParameter[] parameters = null)
        {
            using (var conn = GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.CommandType = CommandType.Text;
                if (parameters != null) cmd.Parameters.AddRange(parameters);
                conn.Open();
                return cmd.ExecuteScalar();
            }
        }

        // Execute and return DataTable
        public static DataTable ExecuteDataTable(string sql, SqlParameter[] parameters = null)
        {
            using (var conn = GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            using (var adapter = new SqlDataAdapter(cmd))
            {
                cmd.CommandType = CommandType.Text;
                if (parameters != null) cmd.Parameters.AddRange(parameters);
                var dt = new DataTable();
                conn.Open();
                adapter.Fill(dt);
                return dt;
            }
        }

        // Execute stored procedure returning DataTable
        public static DataTable ExecuteStoredProcedure(string procName, SqlParameter[] parameters = null)
        {
            using (var conn = GetConnection())
            using (var cmd = new SqlCommand(procName, conn))
            using (var adapter = new SqlDataAdapter(cmd))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                if (parameters != null) cmd.Parameters.AddRange(parameters);
                var dt = new DataTable();
                conn.Open();
                adapter.Fill(dt);
                return dt;
            }
        }
    }
}
