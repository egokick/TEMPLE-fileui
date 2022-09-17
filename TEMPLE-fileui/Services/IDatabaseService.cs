using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace fileui.Models
{
    /// <summary>
    /// Database Service.
    /// </summary>
    public interface IDatabaseService
    {
        /// <summary>
        /// Executes a stored procedure with the given dictionary of parameters.
        /// </summary>
        /// <param name="storedProcedure"></param>
        /// <param name="parameters"></param>
        /// <returns>DataTable</returns>
        Task<DataTable> ExecuteCommand(string storedProcedure, Dictionary<string, string> parameters, int timeout = 10);

        /// <summary>
        /// Executes a stored procedure with the given dictionary of parameters.
        /// </summary>
        /// <param name="storedProcedure"></param>
        /// <param name="parameters"></param>
        /// <returns>DataSet</returns>
        Task<DataSet> Execute(string storedProcedure, Dictionary<string, string> parameters, int timeout = 10);

        /// <summary>
        /// Converts the given object to a dictionary of parameters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        Dictionary<string, string> GetParameters<T>(T obj);
        
        /// <summary>
        /// Gets an integer from the given row or column.
        /// Default return zero.
        /// </summary>
        /// <param name="dataRow"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        int GetInteger(DataRow dataRow, string columnName);

        /// <summary>
        /// Gets a string from the given row or column.
        /// Default return zero.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        string GetString(DataRow row, string columnName);

        /// <summary>
        /// Gets a boolean from the given row and column.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        bool GetBoolean(DataRow row, string columnName);

        /// <summary>
        /// Checks that the MySql database is alive.
        /// </summary>
        /// <returns></returns>
        Task<bool> CheckHealthAsync();

        /// <summary>
        /// Executes a mysql query. You must specify the schema to use within the query.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public Task<DataTable> ExecuteQuery(string query, int timeout = 10);

        /// <summary>
        /// Executes a mysql query. You must specify the schema to use within the query.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public Task<IList<T>> ExecuteQuery<T>(string query, int timeout = 10);

    }
}
