using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Dapper.OData.Infrastructure
{
    /// <summary>
    /// Handles the Db connections and commands
    /// </summary>
    public interface IDbConnection
    {
        /// <summary>
        /// Returns the number of rows affected by the command
        /// </summary>
        /// <param name="query">the command to be executed</param>
        /// <param name="commandType">type of the command (query/procedure)</param>
        /// <param name="isSuccessfull">out flag which indicates the success of the operation</param>
        /// <param name="params">parameters for the command</param>
        /// <param name="transaction"></param>
        int ExecuteQuery(string query, CommandType commandType, out bool isSuccessfull, object @params = null, IDbTransaction transaction = null);
        /// <summary>
        /// Runs a set on commands in a transaction
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        bool ExecuteTransaction(Func<IDbTransaction, bool> transaction);
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
        List<T> GetListResult<T>(string query, CommandType commandType, out bool isDataFound, object @params = null, IDbTransaction transaction = null);
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
        List<T> GetListResult<T, U>(string query, CommandType commandType, Func<T, U, T> map, out bool isDataFound, object @params = null, IDbTransaction transaction = null);
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
        IQueryable<T> GetQueryableResult<T>(string query, out bool isDataFound, object @params = null, string filter = "", int? top = null, int? skip = null, int? take = null, string orderBy = null, IDbTransaction transaction = null);
        /// <summary>
        /// This action will return those rows which will be fetched by the OData Endpoint
        /// </summary>
        /// <typeparam name="T">Type Result Model</typeparam>
        /// <typeparam name="U">Sub-model</typeparam>
        /// <param name="splitOn">It is a comma-separated string that tells Dapper when the returned columns must be mapped to the next object.</param>
        /// <param name="query">Command To Fetch Data (Should only be query/text type)</param>
        /// <param name="map">Model Mapping</param>
        /// <param name="isDataFound">flag to check whether record is present for a certain query</param>
        /// <param name="params">parameters to pass into the query</param>
        /// <param name="filter">Filter to be applied on to the data into the database</param>
        /// <param name="top">Number of top rows selected</param>
        /// <param name="skip">Skip the number of rows (Mostly used to get nth rows)</param>
        /// <param name="take">Number of rows taken after nth skip (Should be used with skip)</param>
        /// <param name="orderBy">Orders the data</param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        IQueryable<T> GetQueryableResult<T, U>(string query, Func<T, U, T> map, string splitOn, out bool isDataFound, object @params = null, string filter = "", int? top = null, int? skip = null, int? take = null, string orderBy = null, IDbTransaction transaction = null);
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
        IQueryable<T> GetQueryableResult<T, U, V>(string query, Func<T, U, V, T> map, string splitOn, out bool isDataFound, object @params = null, string filter = "", int? top = null, int? skip = null, int? take = null, string orderBy = null, IDbTransaction transaction = null);
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
        IQueryable<T> GetQueryableResult<T, U, V, W>(string query, Func<T, U, V, W, T> map, string splitOn, out bool isDataFound, object @params = null, string filter = "", int? top = null, int? skip = null, int? take = null, string orderBy = null, IDbTransaction transaction = null);
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
        IQueryable<T> GetQueryableResult<T, U, V, W, X>(string query, Func<T, U, V, W, X, T> map, string splitOn, out bool isDataFound, object @params = null, string filter = "", int? top = null, int? skip = null, int? take = null, string orderBy = null, IDbTransaction transaction = null);
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
        IQueryable<T> GetQueryableResult<T, U, V, W, X, Y>(string query, Func<T, U, V, W, X, Y, T> map, string splitOn, out bool isDataFound, object @params = null, string filter = "", int? top = null, int? skip = null, int? take = null, string orderBy = null, IDbTransaction transaction = null);
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
        IQueryable<T> GetQueryableResult<T, U, V, W, X, Y, Z>(string query, Func<T, U, V, W, X, Y, Z, T> map, string splitOn, out bool isDataFound, object @params = null, string filter = "", int? top = null, int? skip = null, int? take = null, string orderBy = null, IDbTransaction transaction = null);

        /// <summary>
        /// Returns the first column of the row of the data extracted from the db
        /// </summary>
        /// <typeparam name="T">Any Model Matching the query result</typeparam>
        /// <param name="query">The command which fetches the data</param>
        /// <param name="commandType">Type of the command</param>
        /// <param name="isDataFound">Out flag which indicates the presence of data</param>
        /// <param name="params">parameters of a query</param>
        /// <param name="transaction"></param>
        T GetScalerResult<T>(string query, CommandType commandType, out bool isDataFound, object @params = null, IDbTransaction transaction = null);
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
        T GetSingleResult<T>(string query, CommandType commandType, out bool isDataFound, object @params = null, IDbTransaction transaction = null);
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
        T GetSingleResult<T, U>(string query, CommandType commandType, Func<T, U, T> map, out bool isDataFound, object @params = null, IDbTransaction transaction = null);
    }
}
