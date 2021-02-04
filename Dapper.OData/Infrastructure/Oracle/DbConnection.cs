using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper.Oracle;
using Oracle.ManagedDataAccess.Client;

namespace Dapper.OData.Infrastructure.Oracle
{
    /// <summary>
    /// Handles the Db connections and commands
    /// </summary>
    internal class DbConnection : IDbConnection
    {
        private readonly IDbConfiguration _configuration;
        private readonly ITryCatch _tryCatch;
        //private readonly ILogger //_logger;

        public DbConnection(IDbConfiguration configuration, ITryCatch tryCatch/*, ILogger logger*/)
        {
            _configuration = configuration;
            _tryCatch = tryCatch;
            //_logger = logger;
        }
        /// <summary>
        /// Returns the data extracted from the db in the form of list
        /// </summary>
        /// <typeparam name="T">Any Model Matching the query result</typeparam>
        /// <param name="query">The command which fetches the data</param>
        /// <param name="commandType">Type of the command</param>
        /// <param name="isDataFound">Out flag which indicates the presence of data</param>
        /// <param name="params">parameters of a query</param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public List<T> GetListResult<T>(string query, CommandType commandType, out bool isDataFound, object @params = null, IDbTransaction transaction = null)
        {
            List<T> result;
            OracleDynamicParameters oracleParameters = GetOracleParameters(@params, commandType);
            //_logger.LogInformation($"Query: {query}");
            //_logger.LogInformation($"Command Type: {commandType}");
            //_logger.LogInformation($"Has Params: {@params is not null}");
            using (System.Data.IDbConnection con = new OracleConnection(_configuration.ConnectionString))
            {
                //_logger.LogInformation("Established Db Connection");
                result = _tryCatch.Try(() =>
                {
                    return con.Query<T>(sql: query, commandType: commandType, commandTimeout: _configuration.ConnectionTimeout, param: (oracleParameters is null ? @params : oracleParameters), transaction: transaction).ToList();
                }, out bool isSuccessfull, true);
                //_logger.LogInformation($"Result Count: {result?.Count}");
            }
            isDataFound = (result != null || result.Count > 0);
            //_logger.LogInformation($"Data Found: {isDataFound}");
            return result;
        }
        /// <summary>
        /// This action will return those rows which will be fetched by the OData Endpoint
        /// </summary>
        /// <typeparam name="T">Type Result Model</typeparam>
        /// <param name="query">Command To Fetch Data (Should only be query/text type)</param>
        /// <param name="isDataFound">flag to check whether record is present for a certain query</param>
        /// <param name="params">parameters to pass into the query</param>
        /// <param name="filter">Filter to be applied on to the data into the database</param>
        /// <param name="top">Number of top rows selected</param>
        /// <param name="skip">Skip the number of rows (Mostly used to get nth rows)</param>
        /// <param name="take">Number of rows taken after nth skip (Should be used with skip)</param>
        /// <param name="orderBy">Orders the data</param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public IQueryable<T> GetQueryableResult<T>(string query, out bool isDataFound, object @params = null, string filter = "", int? top = null, int? skip = null, int? take = null, string orderBy = null, IDbTransaction transaction = null)
        {
            IQueryable<T> result;
            //_logger.LogInformation($"Query: {query}");
            //_logger.LogInformation($"Command Type: {commandType}");
            //_logger.LogInformation($"Has Params: {@params is not null}");
            string whereClause = GetWhereClause(filter);
            //_logger.LogInformation($"Where Clause: {whereClause}");
            string formattedQuery = GetFormattedQueryForOData(query, skip, take, orderBy, whereClause, top);
            //_logger.LogInformation($"Formatted Query: {formattedQuery}");
            using (System.Data.IDbConnection con = new OracleConnection(_configuration.ConnectionString))
            {
                //_logger.LogInformation("Established Db Connection");
                result = _tryCatch.Try(() =>
                {
                    return con.Query<T>(sql: formattedQuery, commandType: CommandType.Text, commandTimeout: _configuration.ConnectionTimeout, param: @params, transaction: transaction).AsQueryable();
                }, out bool isSuccessFull, true);
            }
            isDataFound = (result != null || result.Any());
            return result;
        }
        /// <summary>
        /// This action will return those rows which will be fetched by the OData Endpoint
        /// </summary>
        /// <typeparam name="T">Type Result Model</typeparam>
        /// <typeparam name="U">Sub-model</typeparam>
        /// <param name="query">Command To Fetch Data (Should only be query/text type)</param>
        /// <param name="map">Model Mapping</param>
        /// <param name="isDataFound">flag to check whether record is present for a certain query</param>
        /// <param name="splitOn">It is a comma-separated string that tells Dapper when the returned columns must be mapped to the next object.</param>
        /// <param name="params">parameters to pass into the query</param>
        /// <param name="filter">Filter to be applied on to the data into the database</param>
        /// <param name="top">Number of top rows selected</param>
        /// <param name="skip">Skip the number of rows (Mostly used to get nth rows)</param>
        /// <param name="take">Number of rows taken after nth skip (Should be used with skip)</param>
        /// <param name="orderBy">Orders the data</param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public IQueryable<T> GetQueryableResult<T, U>(string query, Func<T, U, T> map, string splitOn, out bool isDataFound, object @params = null, string filter = "", int? top = null, int? skip = null, int? take = null, string orderBy = null, IDbTransaction transaction = null)
        {
            IQueryable<T> result;
            //_logger.LogInformation($"Query: {query}");
            //_logger.LogInformation($"Command Type: {commandType}");
            //_logger.LogInformation($"Has Params: {@params is not null}");
            string whereClause = GetWhereClause(filter);
            //_logger.LogInformation($"Where Clause: {whereClause}");
            string formattedQuery = GetFormattedQueryForOData(query, skip, take, orderBy, whereClause, top);
            //_logger.LogInformation($"Formatted Query: {formattedQuery}");
            using (System.Data.IDbConnection con = new OracleConnection(_configuration.ConnectionString))
            {
                //_logger.LogInformation("Established Db Connection");
                result = _tryCatch.Try(() =>
                {
                    return con.Query(sql: formattedQuery, commandType: CommandType.Text, commandTimeout: _configuration.ConnectionTimeout, param: @params, transaction: transaction, map: map, splitOn: splitOn).AsQueryable();
                }, out bool isSuccessFull, true);
            }
            isDataFound = (result != null || result.Any());
            return result;
        }
        /// <summary>
        /// This action will return those rows which will be fetched by the OData Endpoint
        /// </summary>
        /// <typeparam name="T">Type Result Model</typeparam>
        /// <typeparam name="U">Sub-model</typeparam>
        /// <typeparam name="V">Sub-model</typeparam>
        /// <param name="query">Command To Fetch Data (Should only be query/text type)</param>
        /// <param name="map">Model Mapping</param>
        /// <param name="splitOn">It is a comma-separated string that tells Dapper when the returned columns must be mapped to the next object.</param>
        /// <param name="isDataFound">flag to check whether record is present for a certain query</param>
        /// <param name="params">parameters to pass into the query</param>
        /// <param name="filter">Filter to be applied on to the data into the database</param>
        /// <param name="top">Number of top rows selected</param>
        /// <param name="skip">Skip the number of rows (Mostly used to get nth rows)</param>
        /// <param name="take">Number of rows taken after nth skip (Should be used with skip)</param>
        /// <param name="orderBy">Orders the data</param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public IQueryable<T> GetQueryableResult<T, U, V>(string query, Func<T, U, V, T> map, string splitOn, out bool isDataFound, object @params = null, string filter = "", int? top = null, int? skip = null, int? take = null, string orderBy = null, IDbTransaction transaction = null)
        {
            IQueryable<T> result;
            //_logger.LogInformation($"Query: {query}");
            //_logger.LogInformation($"Command Type: {commandType}");
            //_logger.LogInformation($"Has Params: {@params is not null}");
            string whereClause = GetWhereClause(filter);
            //_logger.LogInformation($"Where Clause: {whereClause}");
            string formattedQuery = GetFormattedQueryForOData(query, skip, take, orderBy, whereClause, top);
            //_logger.LogInformation($"Formatted Query: {formattedQuery}");
            using (System.Data.IDbConnection con = new OracleConnection(_configuration.ConnectionString))
            {
                //_logger.LogInformation("Established Db Connection");
                result = _tryCatch.Try(() =>
                {
                    return con.Query(sql: formattedQuery, commandType: CommandType.Text, commandTimeout: _configuration.ConnectionTimeout, param: @params, transaction: transaction, map: map, splitOn: splitOn).AsQueryable();
                }, out bool isSuccessFull, true);
            }
            isDataFound = (result != null || result.Any());
            return result;
        }
        /// <summary>
        /// This action will return those rows which will be fetched by the OData Endpoint
        /// </summary>
        /// <typeparam name="T">Type Result Model</typeparam>
        /// <typeparam name="U">Sub-model</typeparam>
        /// <typeparam name="V">Sub-model</typeparam>
        /// <typeparam name="W">Sub-model</typeparam>
        /// <param name="query">Command To Fetch Data (Should only be query/text type)</param>
        /// <param name="map">Model Mapping</param>
        /// <param name="splitOn">It is a comma-separated string that tells Dapper when the returned columns must be mapped to the next object.</param>
        /// <param name="isDataFound">flag to check whether record is present for a certain query</param>
        /// <param name="params">parameters to pass into the query</param>
        /// <param name="filter">Filter to be applied on to the data into the database</param>
        /// <param name="top">Number of top rows selected</param>
        /// <param name="skip">Skip the number of rows (Mostly used to get nth rows)</param>
        /// <param name="take">Number of rows taken after nth skip (Should be used with skip)</param>
        /// <param name="orderBy">Orders the data</param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public IQueryable<T> GetQueryableResult<T, U, V, W>(string query, Func<T, U, V, W, T> map, string splitOn, out bool isDataFound, object @params = null, string filter = "", int? top = null, int? skip = null, int? take = null, string orderBy = null, IDbTransaction transaction = null)
        {
            IQueryable<T> result;
            //_logger.LogInformation($"Query: {query}");
            //_logger.LogInformation($"Command Type: {commandType}");
            //_logger.LogInformation($"Has Params: {@params is not null}");
            string whereClause = GetWhereClause(filter);
            //_logger.LogInformation($"Where Clause: {whereClause}");
            string formattedQuery = GetFormattedQueryForOData(query, skip, take, orderBy, whereClause, top);
            //_logger.LogInformation($"Formatted Query: {formattedQuery}");
            using (System.Data.IDbConnection con = new OracleConnection(_configuration.ConnectionString))
            {
                //_logger.LogInformation("Established Db Connection");
                result = _tryCatch.Try(() =>
                {
                    return con.Query(sql: formattedQuery, commandType: CommandType.Text, commandTimeout: _configuration.ConnectionTimeout, param: @params, transaction: transaction, map: map).AsQueryable();
                }, out bool isSuccessFull, true);
            }
            isDataFound = (result != null || result.Any());
            return result;
        }
        /// <summary>
        /// This action will return those rows which will be fetched by the OData Endpoint
        /// </summary>
        /// <typeparam name="T">Type Result Model</typeparam>
        /// <typeparam name="U">Sub-model</typeparam>
        /// <typeparam name="V">Sub-model</typeparam>
        /// <typeparam name="W">Sub-model</typeparam>
        /// <typeparam name="X">Sub-model</typeparam>
        /// <param name="query">Command To Fetch Data (Should only be query/text type)</param>
        /// <param name="map">Model Mapping</param>
        /// <param name="splitOn">It is a comma-separated string that tells Dapper when the returned columns must be mapped to the next object.</param>
        /// <param name="isDataFound">flag to check whether record is present for a certain query</param>
        /// <param name="params">parameters to pass into the query</param>
        /// <param name="filter">Filter to be applied on to the data into the database</param>
        /// <param name="top">Number of top rows selected</param>
        /// <param name="skip">Skip the number of rows (Mostly used to get nth rows)</param>
        /// <param name="take">Number of rows taken after nth skip (Should be used with skip)</param>
        /// <param name="orderBy">Orders the data</param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public IQueryable<T> GetQueryableResult<T, U, V, W, X>(string query, Func<T, U, V, W, X, T> map, string splitOn, out bool isDataFound, object @params = null, string filter = "", int? top = null, int? skip = null, int? take = null, string orderBy = null, IDbTransaction transaction = null)
        {
            IQueryable<T> result;
            //_logger.LogInformation($"Query: {query}");
            //_logger.LogInformation($"Command Type: {commandType}");
            //_logger.LogInformation($"Has Params: {@params is not null}");
            string whereClause = GetWhereClause(filter);
            //_logger.LogInformation($"Where Clause: {whereClause}");
            string formattedQuery = GetFormattedQueryForOData(query, skip, take, orderBy, whereClause, top);
            //_logger.LogInformation($"Formatted Query: {formattedQuery}");
            using (System.Data.IDbConnection con = new OracleConnection(_configuration.ConnectionString))
            {
                //_logger.LogInformation("Established Db Connection");
                result = _tryCatch.Try(() =>
                {
                    return con.Query(sql: formattedQuery, commandType: CommandType.Text, commandTimeout: _configuration.ConnectionTimeout, param: @params, transaction: transaction, map: map).AsQueryable();
                }, out bool isSuccessFull, true);
            }
            isDataFound = (result != null || result.Any());
            return result;
        }
        /// <summary>
        /// This action will return those rows which will be fetched by the OData Endpoint
        /// </summary>
        /// <typeparam name="T">Type Result Model</typeparam>
        /// <typeparam name="U">Sub-model</typeparam>
        /// <typeparam name="V">Sub-model</typeparam>
        /// <typeparam name="W">Sub-model</typeparam>
        /// <typeparam name="X">Sub-model</typeparam>
        /// <typeparam name="Y">Sub-model</typeparam>
        /// <param name="query">Command To Fetch Data (Should only be query/text type)</param>
        /// <param name="map">Model Mapping</param>
        /// <param name="splitOn">It is a comma-separated string that tells Dapper when the returned columns must be mapped to the next object.</param>
        /// <param name="isDataFound">flag to check whether record is present for a certain query</param>
        /// <param name="params">parameters to pass into the query</param>
        /// <param name="filter">Filter to be applied on to the data into the database</param>
        /// <param name="top">Number of top rows selected</param>
        /// <param name="skip">Skip the number of rows (Mostly used to get nth rows)</param>
        /// <param name="take">Number of rows taken after nth skip (Should be used with skip)</param>
        /// <param name="orderBy">Orders the data</param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public IQueryable<T> GetQueryableResult<T, U, V, W, X, Y>(string query, Func<T, U, V, W, X, Y, T> map, string splitOn, out bool isDataFound, object @params = null, string filter = "", int? top = null, int? skip = null, int? take = null, string orderBy = null, IDbTransaction transaction = null)
        {
            IQueryable<T> result;
            //_logger.LogInformation($"Query: {query}");
            //_logger.LogInformation($"Command Type: {commandType}");
            //_logger.LogInformation($"Has Params: {@params is not null}");
            string whereClause = GetWhereClause(filter);
            //_logger.LogInformation($"Where Clause: {whereClause}");
            string formattedQuery = GetFormattedQueryForOData(query, skip, take, orderBy, whereClause, top);
            //_logger.LogInformation($"Formatted Query: {formattedQuery}");
            using (System.Data.IDbConnection con = new OracleConnection(_configuration.ConnectionString))
            {
                //_logger.LogInformation("Established Db Connection");
                result = _tryCatch.Try(() =>
                {
                    return con.Query(sql: formattedQuery, commandType: CommandType.Text, commandTimeout: _configuration.ConnectionTimeout, param: @params, transaction: transaction, map: map).AsQueryable();
                }, out bool isSuccessFull, true);
            }
            isDataFound = (result != null || result.Any());
            return result;
        }
        /// <summary>
        /// This action will return those rows which will be fetched by the OData Endpoint
        /// </summary>
        /// <typeparam name="T">Type Result Model</typeparam>
        /// <typeparam name="U">Sub-model</typeparam>
        /// <typeparam name="V">Sub-model</typeparam>
        /// <typeparam name="W">Sub-model</typeparam>
        /// <typeparam name="X">Sub-model</typeparam>
        /// <typeparam name="Y">Sub-model</typeparam>
        /// <typeparam name="Z">Sub-model</typeparam>
        /// <param name="query">Command To Fetch Data (Should only be query/text type)</param>
        /// <param name="map">Model Mapping</param>
        /// <param name="splitOn">It is a comma-separated string that tells Dapper when the returned columns must be mapped to the next object.</param>
        /// <param name="isDataFound">flag to check whether record is present for a certain query</param>
        /// <param name="params">parameters to pass into the query</param>
        /// <param name="filter">Filter to be applied on to the data into the database</param>
        /// <param name="top">Number of top rows selected</param>
        /// <param name="skip">Skip the number of rows (Mostly used to get nth rows)</param>
        /// <param name="take">Number of rows taken after nth skip (Should be used with skip)</param>
        /// <param name="orderBy">Orders the data</param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public IQueryable<T> GetQueryableResult<T, U, V, W, X, Y, Z>(string query, Func<T, U, V, W, X, Y, Z, T> map, string splitOn, out bool isDataFound, object @params = null, string filter = "", int? top = null, int? skip = null, int? take = null, string orderBy = null, IDbTransaction transaction = null)
        {
            IQueryable<T> result;
            //_logger.LogInformation($"Query: {query}");
            //_logger.LogInformation($"Command Type: {commandType}");
            //_logger.LogInformation($"Has Params: {@params is not null}");
            string whereClause = GetWhereClause(filter);
            //_logger.LogInformation($"Where Clause: {whereClause}");
            string formattedQuery = GetFormattedQueryForOData(query, skip, take, orderBy, whereClause, top);
            //_logger.LogInformation($"Formatted Query: {formattedQuery}");
            using (System.Data.IDbConnection con = new OracleConnection(_configuration.ConnectionString))
            {
                //_logger.LogInformation("Established Db Connection");
                result = _tryCatch.Try(() =>
                {
                    return con.Query(sql: formattedQuery, commandType: CommandType.Text, commandTimeout: _configuration.ConnectionTimeout, param: @params, transaction: transaction, map: map).AsQueryable();
                }, out bool isSuccessFull, true);
            }
            isDataFound = (result != null || result.Any());
            return result;
        }
        private string GetWhereClause(string filter)
        {
            if (!string.IsNullOrEmpty(filter) && !string.IsNullOrWhiteSpace(filter))
            {
                var grammer = new ODataGrammer();
                filter = grammer.ReplaceExpression(filter);
                filter = filter.Replace("UPDATE", string.Empty);
                filter = filter.Replace("DELETE", string.Empty);
                filter = filter.Replace("INSERT", string.Empty);
                filter = filter.Replace("EXEC", string.Empty);
                filter = filter.Replace("EXECUTE", string.Empty);
                filter = filter.Replace("DROP", string.Empty);
                filter = filter.Replace("TRUNC", string.Empty);
                return $"AND {filter}";
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Returns the data extracted from the db in the form of list (Multi object mapper)
        /// </summary>
        /// <typeparam name="T">Any Model Matching the query result</typeparam>
        /// <typeparam name="U">Sub-model</typeparam>
        /// <param name="query">The command which fetches the data</param>
        /// <param name="commandType">Type of the command</param>
        /// <param name="map">object mapping anonymous function</param>
        /// <param name="isDataFound">Out flag which indicates the presence of data</param>
        /// <param name="params">parameters of a query</param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public List<T> GetListResult<T, U>(string query, CommandType commandType, Func<T, U, T> map, out bool isDataFound, object @params = null, IDbTransaction transaction = null)
        {
            List<T> result;
            OracleDynamicParameters oracleParameters = GetOracleParameters(@params, commandType);
            //_logger.LogInformation($"Query: {query}");
            //_logger.LogInformation($"Command Type: {commandType}");
            //_logger.LogInformation($"Has Params: {@params is not null}");
            using (System.Data.IDbConnection con = new OracleConnection(_configuration.ConnectionString))
            {
                //_logger.LogInformation("Established Db Connection");
                result = _tryCatch.Try(() =>
                {
                    return con.Query(sql: query, commandType: commandType, commandTimeout: _configuration.ConnectionTimeout, param: (oracleParameters is null ? @params : oracleParameters), transaction: transaction, map: map).ToList();
                }, out bool isSuccessfull, true);
                //_logger.LogInformation($"Result Count: {result?.Count}");
            }
            isDataFound = (result != null || result.Count > 0);
            //_logger.LogInformation($"Data Found: {isDataFound}");
            return result;
        }
        /// <summary>
        /// Returns the first row of the data extracted from the db
        /// </summary>
        /// <typeparam name="T">Any Model Matching the query result</typeparam>
        /// <param name="query">The command which fetches the data</param>
        /// <param name="commandType">Type of the command</param>
        /// <param name="isDataFound">Out flag which indicates the presence of data</param>
        /// <param name="params">parameters of a query</param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public T GetSingleResult<T>(string query, CommandType commandType, out bool isDataFound, object @params = null, IDbTransaction transaction = null)
        {
            T result;
            OracleDynamicParameters oracleParameters = GetOracleParameters(@params, commandType);
            //_logger.LogInformation($"Query: {query}");
            //_logger.LogInformation($"Command Type: {commandType}");
            //_logger.LogInformation($"Has Params: {@params is not null}");
            using (System.Data.IDbConnection con = new OracleConnection(_configuration.ConnectionString))
            {
                //_logger.LogInformation("Established Db Connection");
                result = _tryCatch.Try(() =>
                {
                    return con.Query<T>(sql: query, commandType: commandType, commandTimeout: _configuration.ConnectionTimeout, param: (oracleParameters is null ? @params : oracleParameters), transaction: transaction).First();
                }, out bool isSuccessfull, true);
            }
            isDataFound = (result != null);
            //_logger.LogInformation($"Data Found: {isDataFound}");
            return result;
        }
        /// <summary>
        /// Returns the first row of the data extracted from the db (Multi object mapper)
        /// </summary>
        /// <typeparam name="T">Any Model Matching the query result</typeparam>
        /// <typeparam name="U">Sub-model</typeparam>
        /// <param name="query">The command which fetches the data</param>
        /// <param name="commandType">Type of the command</param>
        /// <param name="map">object mapping anonymous function</param>
        /// <param name="isDataFound">Out flag which indicates the presence of data</param>
        /// <param name="params">parameters of a query</param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public T GetSingleResult<T, U>(string query, CommandType commandType, Func<T, U, T> map, out bool isDataFound, object @params = null, IDbTransaction transaction = null)
        {
            T result;
            OracleDynamicParameters oracleParameters = GetOracleParameters(@params, commandType);
            //_logger.LogInformation($"Query: {query}");
            //_logger.LogInformation($"Command Type: {commandType}");
            //_logger.LogInformation($"Has Params: {@params is not null}");
            using (System.Data.IDbConnection con = new OracleConnection(_configuration.ConnectionString))
            {
                //_logger.LogInformation("Established Db Connection");
                result = _tryCatch.Try(() =>
                {
                    return con.Query(sql: query, commandType: commandType, commandTimeout: _configuration.ConnectionTimeout, param: (oracleParameters is null ? @params : oracleParameters), transaction: transaction, map: map).Single();
                }, out bool isSuccessfull, true);
            }
            isDataFound = (result != null);
            //_logger.LogInformation($"Data Found: {isDataFound}");
            return result;
        }
        /// <summary>
        /// Returns the first column of the row of the data extracted from the db
        /// </summary>
        /// <typeparam name="T">Any Model Matching the query result</typeparam>
        /// <param name="query">The command which fetches the data</param>
        /// <param name="commandType">Type of the command</param>
        /// <param name="isDataFound">Out flag which indicates the presence of data</param>
        /// <param name="params">parameters of a query</param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public T GetScalerResult<T>(string query, CommandType commandType, out bool isDataFound, object @params = null, IDbTransaction transaction = null)
        {
            T result;
            OracleDynamicParameters oracleParameters = GetOracleParameters(@params, commandType);
            //_logger.LogInformation($"Query: {query}");
            //_logger.LogInformation($"Command Type: {commandType}");
            //_logger.LogInformation($"Has Params: {@params is not null}");
            using (System.Data.IDbConnection con = new OracleConnection(_configuration.ConnectionString))
            {
                //_logger.LogInformation("Established Db Connection");
                result = _tryCatch.Try(() =>
                {
                    return (T)con.ExecuteScalar(sql: query, commandType: commandType, commandTimeout: _configuration.ConnectionTimeout, param: (oracleParameters is null ? @params : oracleParameters), transaction: transaction);
                }, out bool isSuccessfull, true);
            }
            isDataFound = (result != null);
            //_logger.LogInformation($"Data Found: {isDataFound}");
            return result;
        }
        /// <summary>
        /// Returns the number of rows affected by the command
        /// </summary>
        /// <param name="query">the command to be executed</param>
        /// <param name="commandType">type of the command (query/procedure)</param>
        /// <param name="isSuccessfull">out flag which indicates the success of the operation</param>
        /// <param name="params">parameters for the command</param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public int ExecuteQuery(string query, CommandType commandType, out bool isSuccessfull, object @params = null, IDbTransaction transaction = null)
        {
            int result = 0;
            OracleDynamicParameters oracleParameters = GetOracleParameters(@params, commandType);
            //_logger.LogInformation($"Query: {query}");
            //_logger.LogInformation($"Command Type: {commandType}");
            //_logger.LogInformation($"Has Params: {@params is not null}");
            using (System.Data.IDbConnection con = new OracleConnection(_configuration.ConnectionString))
            {
                //_logger.LogInformation("Established Db Connection");
                result = _tryCatch.Try(() =>
                {
                    return con.Execute(query, param: (oracleParameters is null ? @params : oracleParameters), commandTimeout: _configuration.ConnectionTimeout, commandType: commandType, transaction: transaction);
                }, out isSuccessfull, true);
            }
            isSuccessfull = isSuccessfull ? (result > 0) : isSuccessfull;
            return result;
        }
        /// <summary>
        /// Runs a set on commands in a transaction
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public bool ExecuteTransaction(Func<IDbTransaction, bool> transaction)
        {
            if (transaction is null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }
            using System.Data.IDbConnection con = new OracleConnection(_configuration.ConnectionString);
            var dbTransaction = con.BeginTransaction();
            try
            {
                transaction.Invoke(dbTransaction);
                dbTransaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                dbTransaction.Rollback();
                throw;
            }
        }
        private static string GetFormattedQueryForOData(string query, int? skip, int? take, string orderBy, string whereClause, int? top)
        {
            string formattedQuery;
            if (skip is not null)
            {
                if (take is not null && orderBy is not null)
                {
                    formattedQuery = $@"SELECT * FROM ({query.ToUpper()}) V WHERE 1=1 {whereClause} ORDER BY {orderBy.ToUpper()} OFFSET ({skip}) ROWS FETCH NEXT ({take}) ROWS ONLY";
                }
                else
                {
                    throw new Exception("Every skip must have a take and an order by");
                }
            }
            else if (top is not null)
            {
                formattedQuery = $@"SELECT * FROM ({query.ToUpper()}) V WHERE ROWNUM <= {top} {whereClause}";
            }
            else
            {
                formattedQuery = $@"SELECT * FROM ({query.ToUpper()}) V WHERE 1=1 {whereClause}";
            }
            if (skip is null)
            {
                formattedQuery = orderBy is not null ? $"{formattedQuery} ORDER BY {orderBy.ToUpper()}" : formattedQuery;
            }
            return formattedQuery;
        }
        private static OracleDynamicParameters GetOracleParameters(object parameters, CommandType commandType)
        {
            OracleDynamicParameters oracleParameters = null;
            if (commandType == CommandType.StoredProcedure)
            {
                try
                {
                    oracleParameters = (OracleDynamicParameters)parameters;
                }
                catch (InvalidCastException ex)
                {
                    throw new Exception("In case of oracle connection, the params must be provided of the type of OracleDynamicParameters", ex);
                }
            }
            return oracleParameters;
        }
    }
}
