using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace fileui.Models
{
    /// <summary>
    /// Supports database access.
    /// </summary>
    public class DatabaseService : IDatabaseService
    {
        private readonly string _connection;
        private readonly string _connectionNoCredentials;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="sharedSettings"></param>
        public DatabaseService(string databaseConnection, string password)
        {
            _connection = $"server={databaseConnection};user=admin;port=3306;password={password};Allow User Variables=True;CharSet=utf8;";
            _connectionNoCredentials = "";// $"{sharedSettings.DatabaseConnection}";
        }

        /// <inheritdoc />
        public async Task<DataTable> ExecuteCommand(string storedProcedure, Dictionary<string, string> parameters, int timeout = 10)
        {
            for (var i = 0; i <= 4; i++)
            {
                var delayInSeconds = ((1d / 2d) * (Math.Pow(2d, i + 1) - 1d)); // exponential back off: 0s, 0.5s, 1.5s, 3.5s, 7.5s, 15.5s
                var delayMs = (int)(delayInSeconds * 1000);
                if(i != 0) Console.WriteLine($"INFO Waiting {delayMs}ms before trying to call db again");

                using (var conn = new MySqlConnection(_connection))
                {
                    using var cmd = new MySqlCommand(storedProcedure, conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = timeout;
                    AddParameters(cmd, parameters);
                    Console.WriteLine($"DatabaseService.ExecuteCommand CommandText: {cmd.CommandText}");
                    try
                    {
                        if (conn.State != ConnectionState.Open) await conn.OpenAsync();

                        var sqlDataAdapter = new MySqlDataAdapter(cmd);
                        var data = new DataTable();
                        await sqlDataAdapter.FillAsync(data);

                        if (data.HasErrors) Console.WriteLine($"ERROR DatabaseService.ExecuteCommand CommandText: {cmd.CommandText}, connection: {_connectionNoCredentials}");

                        if (conn.State == ConnectionState.Open) await conn.CloseAsync();
                        return data;
                    }
                    catch (Exception ex)
                    {
                        if (conn.State == ConnectionState.Open) await conn.CloseAsync();
                        if (i == 4) // only log error on last attempt in case this is a deadlock
                        {
                            Console.WriteLine(ex.Message + $"DatabaseService.ExecuteCommand connection: {_connectionNoCredentials} { Environment.NewLine}" +
                                      $"Stored procedure: CALL {storedProcedure} {Environment.NewLine}" +
                                      $"CommandText: {cmd.CommandText} {Environment.NewLine}", parameters);
                        }
                    }
                }
                await Task.Delay(delayMs);
            }
            return null;
        }

        /// <inheritdoc />
        public async Task<DataSet> Execute(string storedProcedure, Dictionary<string, string> parameters, int timeout = 10)
        {
            for (var i = 0; i <= 4; i++)
            {
                var delayInSeconds = ((1d / 2d) * (Math.Pow(2d, i + 1) - 1d)); // exponential back off: 0s, 0.5s, 1.5s, 3.5s, 7.5s, 15.5s
                var delayMs = (int)(delayInSeconds * 1000);
                if (i != 0) Console.WriteLine($"INFO Waiting {delayMs}ms before trying to call db again");

                using (var conn = new MySqlConnection(_connection))
                {
                    using var cmd = new MySqlCommand(storedProcedure, conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = timeout;
                    AddParameters(cmd, parameters);
                    Console.WriteLine($"DatabaseService.Execute CommandText: {cmd.CommandText}");
                    try
                    {
                        if (conn.State != ConnectionState.Open) await conn.OpenAsync();

                        var sqlDataAdapter = new MySqlDataAdapter(cmd);
                        var data = new DataSet();
                        await sqlDataAdapter.FillAsync(data);

                        if (data.HasErrors) Console.WriteLine($"ERROR DatabaseService.ExecuteCommand CommandText: {cmd.CommandText}, connection: {_connectionNoCredentials}");

                        if (conn.State == ConnectionState.Open) await conn.CloseAsync();
                        return data;
                    }
                    catch (Exception ex)
                    {
                        if (conn.State == ConnectionState.Open) await conn.CloseAsync();
                        if (i == 4) // only log error on last attempt in case this is a deadlock
                        {
                            Console.WriteLine(ex + $"DatabaseService.Execute connection: {_connectionNoCredentials} {Environment.NewLine}" +
                                $"Stored procedure: CALL {storedProcedure} {Environment.NewLine}" +
                                $"CommandText: {cmd.CommandText} {Environment.NewLine}", parameters);
                        }
                    }
                }
                await Task.Delay(delayMs);
            }
            return null;
        }

        /// <inheritdoc />
        public Dictionary<string, string> GetParameters<T>(T obj)
        {
            var parameters = obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToDictionary(prop => $"p_{prop.Name}", prop => GetValue(obj, prop));
                
            return parameters;
        }

        /// <summary>
        /// Gets the value from the given property and converts it to the 
        /// MySql database equivalent.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="prop"></param>
        /// <returns></returns>
        private string GetValue<T>(T obj, PropertyInfo prop)
        {
            var value = prop.GetValue(obj, null);
            return value switch
            {
                DateTime dt => dt.ToMySqlDateTime(),
                bool boolValue => boolValue ? "1" : "0",
                _ => value?.ToString()
            };
        }

        /// <inheritdoc />
        public int GetInteger(DataRow row, string columnName)
        {
            if (!row.Table.Columns.Contains(columnName)) return 0;
            int.TryParse(row[columnName].ToString(), out var val);
            return val;
        }

        /// <inheritdoc />
        public string GetString(DataRow row, string columnName)
        {
            return !row.Table.Columns.Contains(columnName) ? "" : row[columnName].ToString();
        }

        /// <inheritdoc />
        public bool GetBoolean(DataRow row, string columnName)
        {
            if (!row.Table.Columns.Contains(columnName)) return false;

            var value = row[columnName].ToString();
            var isNumeric = int.TryParse(value, out var n);

            if (isNumeric)
            {
                return Convert.ToBoolean(n);
            }

            bool.TryParse(row[columnName].ToString(), out var val);
            return val;
        }

        /// <inheritdoc/>
        /// <inheritdoc/>
        public async Task<bool> CheckHealthAsync()
        {
            var isConnectionValid = false;
            
            try
            {
                using var conn = new MySqlConnection(_connection);
                if (conn.State != ConnectionState.Open) await conn.OpenAsync();
                isConnectionValid = true;
                if (conn.State == ConnectionState.Open) await conn.CloseAsync();
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex + "DatabaseService.CheckHealthAsync");
            }
            catch (MySqlException sqlEx)
            {
                isConnectionValid = false;
                Console.WriteLine(sqlEx + "DatabaseService.CheckHealthAsync");
            }

            return isConnectionValid;
        }

        public async Task<IList<T>> ExecuteQuery<T>(string query, int timeout = 10)
        {
            for (var i = 0; i <= 4; i++)
            {
                var delayInSeconds = ((1d / 2d) * (Math.Pow(2d, i + 1) - 1d)); // exponential backoff: 0s, 0.5s, 1.5s, 3.5s, 7.5s, 15.5s
                var delayMs = (int)(delayInSeconds * 1000);
                if (i != 0) Console.WriteLine($"INFO Waiting {delayMs}ms before trying to call db again");

                using (var conn = new MySqlConnection(_connection))
                {
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        try
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.CommandTimeout = timeout;
                            Console.WriteLine($"DatabaseService.ExecuteQuery CommandText: {cmd.CommandText}");
                            if (conn.State != ConnectionState.Open) await conn.OpenAsync();

                            var sqlDataAdapter = new MySqlDataAdapter(cmd);
                            var data = new DataTable();
                            await sqlDataAdapter.FillAsync(data);

                            if (data.HasErrors) Console.WriteLine($"ERROR DatabaseService.ExecuteCommand CommandText: {cmd.CommandText}, connection: {_connectionNoCredentials}");
                            if (conn.State == ConnectionState.Open) await conn.CloseAsync();

                            if (data.Rows.Count <= 0)
                            {
                                Console.WriteLine($"ERROR TaskService.ExecuteQuery: returned null object for SQL query: {query}, connection: {_connectionNoCredentials}");
                                return null;
                            }

                            var mappedData = PropertyMapper.ConvertTo<T>(data.Rows);
                            return mappedData;
                        }
                        catch (Exception ex)
                        {
                            if (conn.State == ConnectionState.Open) await conn.CloseAsync();
                            if (i == 4) // only log error on last attempt in case this is a deadlock
                            {
                                Console.WriteLine(ex + $"{ex.InnerException} DatabaseService.ExecuteCommand connection: {_connectionNoCredentials}{Environment.NewLine}" +
                                              $"Query: {query} {Environment.NewLine}" +
                                              $"CommandText: {cmd.CommandText} {Environment.NewLine}", query);
                            }
                        }
                    }
                }

                await Task.Delay(delayMs);
            }
            return null;
        }

        /// <inheritdoc />
        public async Task<DataTable> ExecuteQuery(string query, int timeout = 10)
        {
            using (var conn = new MySqlConnection(_connection))
            {
                using (var cmd = new MySqlCommand(query, conn))
                {
                    try
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandTimeout = timeout;
                        Console.WriteLine($"DatabaseService.ExecuteQuery CommandText: {cmd.CommandText}");
                        if (conn.State != ConnectionState.Open) await conn.OpenAsync();

                        var sqlDataAdapter = new MySqlDataAdapter(cmd);
                        var data = new DataTable();
                        await sqlDataAdapter.FillAsync(data);

                        if (data.HasErrors) Console.WriteLine($"ERROR DatabaseService.ExecuteCommand CommandText: {cmd.CommandText}, connection: {_connectionNoCredentials}");
                        if (conn.State == ConnectionState.Open) await conn.CloseAsync();

                        return data;
                    }
                    catch (Exception ex)
                    {
                        if (conn.State == ConnectionState.Open) await conn.CloseAsync();
                          
                            Console.WriteLine(ex + $"{ex.InnerException} DatabaseService.ExecuteCommand connection: {_connectionNoCredentials}{Environment.NewLine}" +
                                            $"Query: {query} {Environment.NewLine}" +
                                            $"CommandText: {cmd.CommandText} {Environment.NewLine}", query);
                           
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Adds parameters to the Sql Command.
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="parameters"></param>
        private void AddParameters(MySqlCommand cmd, Dictionary<string, string> parameters)
        {
            if ((parameters == null) || (parameters.Count <= 0)) return;
            foreach (var key in parameters.Keys)
            {
                cmd.Parameters.AddWithValue(key, parameters[key]);
            }
        }
    }
}
