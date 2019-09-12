using Ctrl.Core.PetaPoco.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace Ctrl.Core.PetaPoco
{
    public interface IDatabase: IDisposable,IQuery,IAlterPoco,IExecute,ITransactionAccessor
    {
        /// <summary>
        ///     Gets the default mapper. (Default is <see cref="ConventionMapper" />)
        /// </summary>
        /// <returns>
        ///     The default mapper.
        /// </returns>
        IMapper DefaultMapper { get; }

        /// <summary>
        ///     Gets the SQL of the last executed statement
        /// </summary>
        /// <returns>
        ///     The last executed SQL.
        /// </returns>
        string LastSQL { get; }

        /// <summary>
        ///     Gets the arguments to the last execute statement
        /// </summary>
        /// <returns>
        ///     The last executed SQL arguments.
        /// </returns>
        object[] LastArgs { get; }

        /// <summary>
        ///     Gets a formatted string describing the last executed SQL statement and it's argument values
        /// </summary>
        /// <returns>
        ///     The formatted string.
        /// </returns>
        string LastCommand { get; }

        /// <summary>
        ///     Gets or sets the enable auto select. (Default is True)
        /// </summary>
        /// <remarks>
        ///     When set to true, PetaPoco will automatically create the "SELECT columns" section of the query for any query which
        ///     is found to require them.
        /// </remarks>
        /// <returns>
        ///     True, if auto select is enabled; Else, false.
        /// </returns>
        bool EnableAutoSelect { get; set; }

        /// <summary>
        ///     Gets the flag for whether named params are enabled. (Default is True)
        /// </summary>
        /// <remarks>
        ///     When set to true, parameters can be named ?myparam and populated from properties of the passed in argument values.
        /// </remarks>
        /// <returns>
        ///     True, if named parameters are enabled; Else, false.
        /// </returns>
        bool EnableNamedParams { get; set; }

        /// <summary>
        ///     Sets the timeout value, in seconds, which PetaPoco applies to all <see cref="IDbCommand.CommandTimeout" />.
        ///     (Default is 0)
        /// </summary>
        /// <remarks>
        ///     If the current value is zero PetaPoco will not set the command timeout, and therefor, the .net default (30 seconds)
        ///     will be in affect.
        /// </remarks>
        /// <returns>
        ///     The current command timeout.
        /// </returns>
        int CommandTimeout { get; set; }

        /// <summary>
        ///     Sets the timeout value for the next (and only next) SQL statement.
        /// </summary>
        /// <remarks>
        ///     This is a one-time settings, which after use, will return the <see cref="CommandTimeout" /> setting.
        /// </remarks>
        /// <returns>
        ///     The one time command timeout.
        /// </returns>
        int OneTimeCommandTimeout { get; set; }

        /// <summary>
        ///     Gets the current <seealso cref="Provider" />.
        /// </summary>
        /// <returns>
        ///     The current database provider.
        /// </returns>
        IProvider Provider { get; }

        /// <summary>
        ///     Gets the connection string.
        /// </summary>
        /// <returns>
        ///     The connection string.
        /// </returns>
        string ConnectionString { get; }

        /// <summary>
        ///     Gets or sets the transaction isolation level.
        /// </summary>
        /// <remarks>
        ///     When value is null, the underlying providers default isolation level is used.
        /// </remarks>
        IsolationLevel? IsolationLevel { get; set; }

        /// <summary>
        ///     Starts or continues a transaction.
        /// </summary>
        /// <returns>An ITransaction reference that must be Completed or disposed</returns>
        /// <remarks>
        ///     This method makes management of calls to Begin/End/CompleteTransaction easier.
        ///     The usage pattern for this should be:
        ///     using (var tx = db.GetTransaction())
        ///     {
        ///     // Do stuff
        ///     db.Update(...);
        ///     // Mark the transaction as complete
        ///     tx.Complete();
        ///     }
        ///     Transactions can be nested but they must all be completed otherwise the entire
        ///     transaction is aborted.
        /// </remarks>
        ITransaction GetTransaction();

        /// <summary>
        ///     Starts a transaction scope, see GetTransaction() for recommended usage
        /// </summary>
        void BeginTransaction();

        /// <summary>
        ///     Aborts the entire outer most transaction scope
        /// </summary>
        /// <remarks>
        ///     Called automatically by Transaction.Dispose()
        ///     if the transaction wasn't completed.
        /// </remarks>
        void AbortTransaction();

        /// <summary>
        ///     Marks the current transaction scope as complete.
        /// </summary>
        void CompleteTransaction();
    }
    #region IQuery
    public interface IQuery
    {
        /// <summary>
        ///     Runs a SELECT * query, returning the results as an IEnumerable collection
        /// </summary>
        /// <typeparam name="T">The Type representing a row in the result set</typeparam>
        /// <returns>An enumerable collection of result records</returns>
        /// <remarks>
        ///     For some DB providers, care should be taken to not start a new Query before finishing with
        ///     and disposing the previous one. In cases where this is an issue, consider using Fetch which
        ///     returns the results as a List rather than an IEnumerable.
        /// </remarks>
        IEnumerable<T> Query<T>();

        /// <summary>
        ///     Runs an SQL query, returning the results as an IEnumerable collection
        /// </summary>
        /// <typeparam name="T">The Type representing a row in the result set</typeparam>
        /// <param name="sql">The SQL query</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL statement</param>
        /// <returns>An enumerable collection of result records</returns>
        /// <remarks>
        ///     For some DB providers, care should be taken to not start a new Query before finishing with
        ///     and disposing the previous one. In cases where this is an issue, consider using Fetch which
        ///     returns the results as a List rather than an IEnumerable.
        /// </remarks>
        IEnumerable<T> Query<T>(string sql, params object[] args);

        /// <summary>
        ///     Runs an SQL query, returning the results as an IEnumerable collection
        /// </summary>
        /// <typeparam name="T">The Type representing a row in the result set</typeparam>
        /// <param name="sql">An SQL builder object representing the base SQL query and it's arguments</param>
        /// <returns>An enumerable collection of result records</returns>
        /// <remarks>
        ///     For some DB providers, care should be taken to not start a new Query before finishing with
        ///     and disposing the previous one. In cases where this is an issue, consider using Fetch which
        ///     returns the results as a List rather than an IEnumerable.
        /// </remarks>
        IEnumerable<T> Query<T>(Sql sql);

        /// <summary>
        ///     Perform a multi-poco query
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <typeparam name="TRet">The type of objects in the returned IEnumerable</typeparam>
        /// <param name="cb">A callback function to connect the POCO instances, or null to automatically guess the relationships</param>
        /// <param name="sql">The SQL query to be executed</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL</param>
        /// <returns>A collection of POCO's as an IEnumerable</returns>
        IEnumerable<TRet> Query<T1, T2, TRet>(Func<T1, T2, TRet> cb, string sql, params object[] args);

        /// <summary>
        ///     Perform a multi-poco query
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <typeparam name="T3">The third POCO type</typeparam>
        /// <typeparam name="TRet">The type of objects in the returned IEnumerable</typeparam>
        /// <param name="cb">A callback function to connect the POCO instances, or null to automatically guess the relationships</param>
        /// <param name="sql">The SQL query to be executed</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL</param>
        /// <returns>A collection of POCO's as an IEnumerable</returns>
        IEnumerable<TRet> Query<T1, T2, T3, TRet>(Func<T1, T2, T3, TRet> cb, string sql, params object[] args);

        /// <summary>
        ///     Perform a multi-poco query
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <typeparam name="T3">The third POCO type</typeparam>
        /// <typeparam name="T4">The fourth POCO type</typeparam>
        /// <typeparam name="TRet">The type of objects in the returned IEnumerable</typeparam>
        /// <param name="cb">A callback function to connect the POCO instances, or null to automatically guess the relationships</param>
        /// <param name="sql">The SQL query to be executed</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL</param>
        /// <returns>A collection of POCO's as an IEnumerable</returns>
        IEnumerable<TRet> Query<T1, T2, T3, T4, TRet>(Func<T1, T2, T3, T4, TRet> cb, string sql, params object[] args);

        /// <summary>
        ///     Perform a multi-poco query
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <typeparam name="T3">The third POCO type</typeparam>
        /// <typeparam name="T4">The fourth POCO type</typeparam>
        /// <typeparam name="T5">The fifth POCO type</typeparam>
        /// <typeparam name="TRet">The type of objects in the returned IEnumerable</typeparam>
        /// <param name="cb">A callback function to connect the POCO instances, or null to automatically guess the relationships</param>
        /// <param name="sql">The SQL query to be executed</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL</param>
        /// <returns>A collection of POCO's as an IEnumerable</returns>
        IEnumerable<TRet> Query<T1, T2, T3, T4, T5, TRet>(Func<T1, T2, T3, T4, T5, TRet> cb, string sql, params object[] args);

        /// <summary>
        ///     Perform a multi-poco query
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <typeparam name="TRet">The type of objects in the returned IEnumerable</typeparam>
        /// <param name="cb">A callback function to connect the POCO instances, or null to automatically guess the relationships</param>
        /// <param name="sql">An SQL builder object representing the query and it's arguments</param>
        /// <returns>A collection of POCO's as an IEnumerable</returns>
        IEnumerable<TRet> Query<T1, T2, TRet>(Func<T1, T2, TRet> cb, Sql sql);

        /// <summary>
        ///     Perform a multi-poco query
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <typeparam name="T3">The third POCO type</typeparam>
        /// <typeparam name="TRet">The type of objects in the returned IEnumerable</typeparam>
        /// <param name="cb">A callback function to connect the POCO instances, or null to automatically guess the relationships</param>
        /// <param name="sql">An SQL builder object representing the query and it's arguments</param>
        /// <returns>A collection of POCO's as an IEnumerable</returns>
        IEnumerable<TRet> Query<T1, T2, T3, TRet>(Func<T1, T2, T3, TRet> cb, Sql sql);

        /// <summary>
        ///     Perform a multi-poco query
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <typeparam name="T3">The third POCO type</typeparam>
        /// <typeparam name="T4">The fourth POCO type</typeparam>
        /// <typeparam name="TRet">The type of objects in the returned IEnumerable</typeparam>
        /// <param name="cb">A callback function to connect the POCO instances, or null to automatically guess the relationships</param>
        /// <param name="sql">An SQL builder object representing the query and it's arguments</param>
        /// <returns>A collection of POCO's as an IEnumerable</returns>
        IEnumerable<TRet> Query<T1, T2, T3, T4, TRet>(Func<T1, T2, T3, T4, TRet> cb, Sql sql);

        /// <summary>
        ///     Perform a multi-poco query
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <typeparam name="T3">The third POCO type</typeparam>
        /// <typeparam name="T4">The fourth POCO type</typeparam>
        /// <typeparam name="T5">The fifth POCO type</typeparam>
        /// <typeparam name="TRet">The type of objects in the returned IEnumerable</typeparam>
        /// <param name="cb">A callback function to connect the POCO instances, or null to automatically guess the relationships</param>
        /// <param name="sql">An SQL builder object representing the query and it's arguments</param>
        /// <returns>A collection of POCO's as an IEnumerable</returns>
        IEnumerable<TRet> Query<T1, T2, T3, T4, T5, TRet>(Func<T1, T2, T3, T4, T5, TRet> cb, Sql sql);

        /// <summary>
        ///     Perform a multi-poco query
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <param name="sql">The SQL query to be executed</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL</param>
        /// <returns>A collection of POCO's as an IEnumerable</returns>
        IEnumerable<T1> Query<T1, T2>(string sql, params object[] args);

        /// <summary>
        ///     Perform a multi-poco query
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <typeparam name="T3">The third POCO type</typeparam>
        /// <param name="sql">The SQL query to be executed</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL</param>
        /// <returns>A collection of POCO's as an IEnumerable</returns>
        IEnumerable<T1> Query<T1, T2, T3>(string sql, params object[] args);

        /// <summary>
        ///     Perform a multi-poco query
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <typeparam name="T3">The third POCO type</typeparam>
        /// <typeparam name="T4">The fourth POCO type</typeparam>
        /// <param name="sql">The SQL query to be executed</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL</param>
        /// <returns>A collection of POCO's as an IEnumerable</returns>
        IEnumerable<T1> Query<T1, T2, T3, T4>(string sql, params object[] args);

        /// <summary>
        ///     Perform a multi-poco query
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <typeparam name="T3">The third POCO type</typeparam>
        /// <typeparam name="T4">The fourth POCO type</typeparam>
        /// <typeparam name="T5">The fifth POCO type</typeparam>
        /// <param name="sql">The SQL query to be executed</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL</param>
        /// <returns>A collection of POCO's as an IEnumerable</returns>
        IEnumerable<T1> Query<T1, T2, T3, T4, T5>(string sql, params object[] args);

        /// <summary>
        ///     Perform a multi-poco query
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <param name="sql">An SQL builder object representing the query and it's arguments</param>
        /// <returns>A collection of POCO's as an IEnumerable</returns>
        IEnumerable<T1> Query<T1, T2>(Sql sql);

        /// <summary>
        ///     Perform a multi-poco query
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <typeparam name="T3">The third POCO type</typeparam>
        /// <param name="sql">An SQL builder object representing the query and it's arguments</param>
        /// <returns>A collection of POCO's as an IEnumerable</returns>
        IEnumerable<T1> Query<T1, T2, T3>(Sql sql);

        /// <summary>
        ///     Perform a multi-poco query
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <typeparam name="T3">The third POCO type</typeparam>
        /// <typeparam name="T4">The fourth POCO type</typeparam>
        /// <param name="sql">An SQL builder object representing the query and it's arguments</param>
        /// <returns>A collection of POCO's as an IEnumerable</returns>
        IEnumerable<T1> Query<T1, T2, T3, T4>(Sql sql);

        /// <summary>
        ///     Perform a multi-poco query
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <typeparam name="T3">The third POCO type</typeparam>
        /// <typeparam name="T4">The fourth POCO type</typeparam>
        /// <typeparam name="T5">The fifth POCO type</typeparam>
        /// <param name="sql">An SQL builder object representing the query and it's arguments</param>
        /// <returns>A collection of POCO's as an IEnumerable</returns>
        IEnumerable<T1> Query<T1, T2, T3, T4, T5>(Sql sql);

        /// <summary>
        ///     Performs a multi-poco query
        /// </summary>
        /// <typeparam name="TRet">The type of objects in the returned IEnumerable</typeparam>
        /// <param name="types">An array of Types representing the POCO types of the returned result set.</param>
        /// <param name="cb">A callback function to connect the POCO instances, or null to automatically guess the relationships</param>
        /// <param name="sql">The SQL query to be executed</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL</param>
        /// <returns>A collection of POCO's as an IEnumerable</returns>
        IEnumerable<TRet> Query<TRet>(Type[] types, object cb, string sql, params object[] args);

        /// <summary>
        ///     Runs a SELECT * query and returns the result set as a typed list
        /// </summary>
        /// <typeparam name="T">The Type representing a row in the result set</typeparam>
        /// <returns>A List holding the results of the query</returns>
        List<T> Fetch<T>();

        /// <summary>
        ///     Runs a query and returns the result set as a typed list
        /// </summary>
        /// <typeparam name="T">The Type representing a row in the result set</typeparam>
        /// <param name="sql">The SQL query to execute</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL</param>
        /// <returns>A List holding the results of the query</returns>
        List<T> Fetch<T>(string sql, params object[] args);

        /// <summary>
        ///     Runs a query and returns the result set as a typed list
        /// </summary>
        /// <typeparam name="T">The Type representing a row in the result set</typeparam>
        /// <param name="sql">An SQL builder object representing the query and it's arguments</param>
        /// <returns>A List holding the results of the query</returns>
        List<T> Fetch<T>(Sql sql);

        /// <summary>
        ///     Retrieves a page of records	and the total number of available records
        /// </summary>
        /// <typeparam name="T">The Type representing a row in the result set</typeparam>
        /// <param name="page">The 1 based page number to retrieve</param>
        /// <param name="itemsPerPage">The number of records per page</param>
        /// <param name="sqlCount">The SQL to retrieve the total number of records</param>
        /// <param name="countArgs">Arguments to any embedded parameters in the sqlCount statement</param>
        /// <param name="sqlPage">The SQL To retrieve a single page of results</param>
        /// <param name="pageArgs">Arguments to any embedded parameters in the sqlPage statement</param>
        /// <returns>A Page of results</returns>
        /// <remarks>
        ///     This method allows separate SQL statements to be explicitly provided for the two parts of the page query.
        ///     The page and itemsPerPage parameters are not used directly and are used simply to populate the returned Page
        ///     object.
        /// </remarks>
        Page<T> Page<T>(long page, long itemsPerPage, string sqlCount, object[] countArgs, string sqlPage, object[] pageArgs);

        /// <summary>
        ///     Retrieves a page of records	and the total number of available records
        /// </summary>
        /// <typeparam name="T">The Type representing a row in the result set</typeparam>
        /// <param name="page">The 1 based page number to retrieve</param>
        /// <param name="itemsPerPage">The number of records per page</param>
        /// <returns>A Page of results</returns>
        /// <remarks>
        ///     PetaPoco will automatically modify a default SELECT * statement to only retrieve the
        ///     records for the specified page.  It will also execute a second query to retrieve the
        ///     total number of records in the result set.
        /// </remarks>
        Page<T> Page<T>(long page, long itemsPerPage);

        /// <summary>
        ///     Retrieves a page of records	and the total number of available records
        /// </summary>
        /// <typeparam name="T">The Type representing a row in the result set</typeparam>
        /// <param name="page">The 1 based page number to retrieve</param>
        /// <param name="itemsPerPage">The number of records per page</param>
        /// <param name="sql">The base SQL query</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL statement</param>
        /// <returns>A Page of results</returns>
        /// <remarks>
        ///     PetaPoco will automatically modify the supplied SELECT statement to only retrieve the
        ///     records for the specified page.  It will also execute a second query to retrieve the
        ///     total number of records in the result set.
        /// </remarks>
        Page<T> Page<T>(long page, long itemsPerPage, string sql, params object[] args);

        /// <summary>
        ///     Retrieves a page of records	and the total number of available records
        /// </summary>
        /// <typeparam name="T">The Type representing a row in the result set</typeparam>
        /// <param name="page">The 1 based page number to retrieve</param>
        /// <param name="itemsPerPage">The number of records per page</param>
        /// <param name="sql">An SQL builder object representing the base SQL query and it's arguments</param>
        /// <returns>A Page of results</returns>
        /// <remarks>
        ///     PetaPoco will automatically modify the supplied SELECT statement to only retrieve the
        ///     records for the specified page.  It will also execute a second query to retrieve the
        ///     total number of records in the result set.
        /// </remarks>
        Page<T> Page<T>(long page, long itemsPerPage, Sql sql);

        /// <summary>
        ///     Retrieves a page of records	and the total number of available records
        /// </summary>
        /// <typeparam name="T">The Type representing a row in the result set</typeparam>
        /// <param name="page">The 1 based page number to retrieve</param>
        /// <param name="itemsPerPage">The number of records per page</param>
        /// <param name="sqlCount">An SQL builder object representing the SQL to retrieve the total number of records</param>
        /// <param name="sqlPage">An SQL builder object representing the SQL to retrieve a single page of results</param>
        /// <returns>A Page of results</returns>
        /// <remarks>
        ///     This method allows separate SQL statements to be explicitly provided for the two parts of the page query.
        ///     The page and itemsPerPage parameters are not used directly and are used simply to populate the returned Page
        ///     object.
        /// </remarks>
        Page<T> Page<T>(long page, long itemsPerPage, Sql sqlCount, Sql sqlPage);

        /// <summary>
        ///     Retrieves a page of records (without the total count)
        /// </summary>
        /// <typeparam name="T">The Type representing a row in the result set</typeparam>
        /// <param name="page">The 1 based page number to retrieve</param>
        /// <param name="itemsPerPage">The number of records per page</param>
        /// <returns>A List of results</returns>
        /// <remarks>
        ///     PetaPoco will automatically modify a default SELECT * statement to only retrieve the
        ///     records for the specified page.
        /// </remarks>
        List<T> Fetch<T>(long page, long itemsPerPage);

        /// <summary>
        ///     Retrieves a page of records (without the total count)
        /// </summary>
        /// <typeparam name="T">The Type representing a row in the result set</typeparam>
        /// <param name="page">The 1 based page number to retrieve</param>
        /// <param name="itemsPerPage">The number of records per page</param>
        /// <param name="sql">The base SQL query</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL statement</param>
        /// <returns>A List of results</returns>
        /// <remarks>
        ///     PetaPoco will automatically modify the supplied SELECT statement to only retrieve the
        ///     records for the specified page.
        /// </remarks>
        List<T> Fetch<T>(long page, long itemsPerPage, string sql, params object[] args);

        /// <summary>
        ///     Retrieves a page of records (without the total count)
        /// </summary>
        /// <typeparam name="T">The Type representing a row in the result set</typeparam>
        /// <param name="page">The 1 based page number to retrieve</param>
        /// <param name="itemsPerPage">The number of records per page</param>
        /// <param name="sql">An SQL builder object representing the base SQL query and it's arguments</param>
        /// <returns>A List of results</returns>
        /// <remarks>
        ///     PetaPoco will automatically modify the supplied SELECT statement to only retrieve the
        ///     records for the specified page.
        /// </remarks>
        List<T> Fetch<T>(long page, long itemsPerPage, Sql sql);


        /// <summary>
        ///     Retrieves a range of records from result set
        /// </summary>
        /// <typeparam name="T">The Type representing a row in the result set</typeparam>
        /// <param name="skip">The number of rows at the start of the result set to skip over</param>
        /// <param name="take">The number of rows to retrieve</param>
        /// <returns>A List of results</returns>
        /// <remarks>
        ///     PetaPoco will automatically modify a default SELECT * statement to only retrieve the
        ///     records for the specified range.
        /// </remarks>
        List<T> SkipTake<T>(long skip, long take);

        /// <summary>
        ///     Retrieves a range of records from result set
        /// </summary>
        /// <typeparam name="T">The Type representing a row in the result set</typeparam>
        /// <param name="skip">The number of rows at the start of the result set to skip over</param>
        /// <param name="take">The number of rows to retrieve</param>
        /// <param name="sql">The base SQL query</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL statement</param>
        /// <returns>A List of results</returns>
        /// <remarks>
        ///     PetaPoco will automatically modify the supplied SELECT statement to only retrieve the
        ///     records for the specified range.
        /// </remarks>
        List<T> SkipTake<T>(long skip, long take, string sql, params object[] args);

        /// <summary>
        ///     Retrieves a range of records from result set
        /// </summary>
        /// <typeparam name="T">The Type representing a row in the result set</typeparam>
        /// <param name="skip">The number of rows at the start of the result set to skip over</param>
        /// <param name="take">The number of rows to retrieve</param>
        /// <param name="sql">An SQL builder object representing the base SQL query and it's arguments</param>
        /// <returns>A List of results</returns>
        /// <remarks>
        ///     PetaPoco will automatically modify the supplied SELECT statement to only retrieve the
        ///     records for the specified range.
        /// </remarks>
        List<T> SkipTake<T>(long skip, long take, Sql sql);

        /// <summary>
        ///     Checks for the existence of a row with the specified primary key value.
        /// </summary>
        /// <typeparam name="T">The Type representing the table being queried</typeparam>
        /// <param name="primaryKey">The primary key value to look for</param>
        /// <returns>True if a record with the specified primary key value exists.</returns>
        bool Exists<T>(object primaryKey);

        /// <summary>
        ///     Checks for the existence of a row matching the specified condition
        /// </summary>
        /// <typeparam name="T">The Type representing the table being queried</typeparam>
        /// <param name="sqlCondition">The SQL expression to be tested for (ie: the WHERE expression)</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL statement</param>
        /// <returns>True if a record matching the condition is found.</returns>
        bool Exists<T>(string sqlCondition, params object[] args);

        /// <summary>
        ///     Returns the record with the specified primary key value
        /// </summary>
        /// <typeparam name="T">The Type representing a row in the result set</typeparam>
        /// <param name="primaryKey">The primary key value of the record to fetch</param>
        /// <returns>The single record matching the specified primary key value</returns>
        /// <remarks>
        ///     Throws an exception if there are zero or more than one record with the specified primary key value.
        /// </remarks>
        T Single<T>(object primaryKey);

        /// <summary>
        ///     Runs a query that should always return a single row.
        /// </summary>
        /// <typeparam name="T">The Type representing a row in the result set</typeparam>
        /// <param name="sql">The SQL query</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL statement</param>
        /// <returns>The single record matching the specified primary key value</returns>
        /// <remarks>
        ///     Throws an exception if there are zero or more than one matching record
        /// </remarks>
        T Single<T>(string sql, params object[] args);

        /// <summary>
        ///     Runs a query that should always return a single row.
        /// </summary>
        /// <typeparam name="T">The Type representing a row in the result set</typeparam>
        /// <param name="sql">An SQL builder object representing the query and it's arguments</param>
        /// <returns>The single record matching the specified primary key value</returns>
        /// <remarks>
        ///     Throws an exception if there are zero or more than one matching record
        /// </remarks>
        T Single<T>(Sql sql);

        /// <summary>
        ///     Runs a query that should always return either a single row, or no rows
        /// </summary>
        /// <typeparam name="T">The Type representing a row in the result set</typeparam>
        /// <param name="sql">An SQL builder object representing the query and it's arguments</param>
        /// <returns>The single record matching the specified primary key value, or default(T) if no matching rows</returns>
        T SingleOrDefault<T>(Sql sql);

        /// <summary>
        ///     Returns the record with the specified primary key value, or the default value if not found
        /// </summary>
        /// <typeparam name="T">The Type representing a row in the result set</typeparam>
        /// <param name="primaryKey">The primary key value of the record to fetch</param>
        /// <returns>The single record matching the specified primary key value</returns>
        /// <remarks>
        ///     If there are no records with the specified primary key value, default(T) (typically null) is returned.
        /// </remarks>
        T SingleOrDefault<T>(object primaryKey);

        /// <summary>
        ///     Runs a query that should always return either a single row, or no rows
        /// </summary>
        /// <typeparam name="T">The Type representing a row in the result set</typeparam>
        /// <param name="sql">The SQL query</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL statement</param>
        /// <returns>The single record matching the specified primary key value, or default(T) if no matching rows</returns>
        T SingleOrDefault<T>(string sql, params object[] args);

        /// <summary>
        ///     Runs a query that should always return at least one return
        /// </summary>
        /// <typeparam name="T">The Type representing a row in the result set</typeparam>
        /// <param name="sql">The SQL query</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL statement</param>
        /// <returns>The first record in the result set</returns>
        T First<T>(string sql, params object[] args);

        /// <summary>
        ///     Runs a query that should always return at least one return
        /// </summary>
        /// <typeparam name="T">The Type representing a row in the result set</typeparam>
        /// <param name="sql">An SQL builder object representing the query and it's arguments</param>
        /// <returns>The first record in the result set</returns>
        T First<T>(Sql sql);

        /// <summary>
        ///     Runs a query and returns the first record, or the default value if no matching records
        /// </summary>
        /// <typeparam name="T">The Type representing a row in the result set</typeparam>
        /// <param name="sql">The SQL query</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL statement</param>
        /// <returns>The first record in the result set, or default(T) if no matching rows</returns>
        T FirstOrDefault<T>(string sql, params object[] args);

        /// <summary>
        ///     Runs a query and returns the first record, or the default value if no matching records
        /// </summary>
        /// <typeparam name="T">The Type representing a row in the result set</typeparam>
        /// <param name="sql">An SQL builder object representing the query and it's arguments</param>
        /// <returns>The first record in the result set, or default(T) if no matching rows</returns>
        T FirstOrDefault<T>(Sql sql);

        /// <summary>
        ///     Perform a multi-poco fetch
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <typeparam name="TRet">The returned list POCO type</typeparam>
        /// <param name="cb">A callback function to connect the POCO instances, or null to automatically guess the relationships</param>
        /// <param name="sql">The SQL query to be executed</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL</param>
        /// <returns>A collection of POCO's as a List</returns>
        List<TRet> Fetch<T1, T2, TRet>(Func<T1, T2, TRet> cb, string sql, params object[] args);

        /// <summary>
        ///     Perform a multi-poco fetch
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <typeparam name="T3">The third POCO type</typeparam>
        /// <typeparam name="TRet">The returned list POCO type</typeparam>
        /// <param name="cb">A callback function to connect the POCO instances, or null to automatically guess the relationships</param>
        /// <param name="sql">The SQL query to be executed</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL</param>
        /// <returns>A collection of POCO's as a List</returns>
        List<TRet> Fetch<T1, T2, T3, TRet>(Func<T1, T2, T3, TRet> cb, string sql, params object[] args);

        /// <summary>
        ///     Perform a multi-poco fetch
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <typeparam name="T3">The third POCO type</typeparam>
        /// <typeparam name="T4">The fourth POCO type</typeparam>
        /// <typeparam name="TRet">The returned list POCO type</typeparam>
        /// <param name="cb">A callback function to connect the POCO instances, or null to automatically guess the relationships</param>
        /// <param name="sql">The SQL query to be executed</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL</param>
        /// <returns>A collection of POCO's as a List</returns>
        List<TRet> Fetch<T1, T2, T3, T4, TRet>(Func<T1, T2, T3, T4, TRet> cb, string sql, params object[] args);

        /// <summary>
        ///     Perform a multi-poco fetch
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <typeparam name="T3">The third POCO type</typeparam>
        /// <typeparam name="T4">The fourth POCO type</typeparam>
        /// <typeparam name="T5">The fifth POCO type</typeparam>
        /// <typeparam name="TRet">The returned list POCO type</typeparam>
        /// <param name="cb">A callback function to connect the POCO instances, or null to automatically guess the relationships</param>
        /// <param name="sql">The SQL query to be executed</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL</param>
        /// <returns>A collection of POCO's as a List</returns>
        List<TRet> Fetch<T1, T2, T3, T4, T5, TRet>(Func<T1, T2, T3, T4, T5, TRet> cb, string sql, params object[] args);

        /// <summary>
        ///     Perform a multi-poco fetch
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <typeparam name="TRet">The returned list POCO type</typeparam>
        /// <param name="cb">A callback function to connect the POCO instances, or null to automatically guess the relationships</param>
        /// <param name="sql">An SQL builder object representing the query and it's arguments</param>
        /// <returns>A collection of POCO's as a List</returns>
        List<TRet> Fetch<T1, T2, TRet>(Func<T1, T2, TRet> cb, Sql sql);

        /// <summary>
        ///     Perform a multi-poco fetch
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <typeparam name="T3">The third POCO type</typeparam>
        /// <typeparam name="TRet">The returned list POCO type</typeparam>
        /// <param name="cb">A callback function to connect the POCO instances, or null to automatically guess the relationships</param>
        /// <param name="sql">An SQL builder object representing the query and it's arguments</param>
        /// <returns>A collection of POCO's as a List</returns>
        List<TRet> Fetch<T1, T2, T3, TRet>(Func<T1, T2, T3, TRet> cb, Sql sql);

        /// <summary>
        ///     Perform a multi-poco fetch
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <typeparam name="T3">The third POCO type</typeparam>
        /// <typeparam name="T4">The fourth POCO type</typeparam>
        /// <typeparam name="TRet">The returned list POCO type</typeparam>
        /// <param name="cb">A callback function to connect the POCO instances, or null to automatically guess the relationships</param>
        /// <param name="sql">An SQL builder object representing the query and it's arguments</param>
        /// <returns>A collection of POCO's as a List</returns>
        List<TRet> Fetch<T1, T2, T3, T4, TRet>(Func<T1, T2, T3, T4, TRet> cb, Sql sql);

        /// <summary>
        ///     Perform a multi-poco fetch
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <typeparam name="T3">The third POCO type</typeparam>
        /// <typeparam name="T4">The fourth POCO type</typeparam>
        /// <typeparam name="T5">The fifth POCO type</typeparam>
        /// <typeparam name="TRet">The returned list POCO type</typeparam>
        /// <param name="cb">A callback function to connect the POCO instances, or null to automatically guess the relationships</param>
        /// <param name="sql">An SQL builder object representing the query and it's arguments</param>
        /// <returns>A collection of POCO's as a List</returns>
        List<TRet> Fetch<T1, T2, T3, T4, T5, TRet>(Func<T1, T2, T3, T4, T5, TRet> cb, Sql sql);

        /// <summary>
        ///     Perform a multi-poco fetch
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <param name="sql">The SQL query to be executed</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL</param>
        /// <returns>A collection of POCO's as a List</returns>
        List<T1> Fetch<T1, T2>(string sql, params object[] args);

        /// <summary>
        ///     Perform a multi-poco fetch
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <typeparam name="T3">The third POCO type</typeparam>
        /// <param name="sql">The SQL query to be executed</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL</param>
        /// <returns>A collection of POCO's as a List</returns>
        List<T1> Fetch<T1, T2, T3>(string sql, params object[] args);

        /// <summary>
        ///     Perform a multi-poco fetch
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <typeparam name="T3">The third POCO type</typeparam>
        /// <typeparam name="T4">The fourth POCO type</typeparam>
        /// <param name="sql">The SQL query to be executed</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL</param>
        /// <returns>A collection of POCO's as a List</returns>
        List<T1> Fetch<T1, T2, T3, T4>(string sql, params object[] args);

        /// <summary>
        ///     Perform a multi-poco fetch
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <typeparam name="T3">The third POCO type</typeparam>
        /// <typeparam name="T4">The fourth POCO type</typeparam>
        /// <typeparam name="T5">The fifth POCO type</typeparam>
        /// <param name="sql">The SQL query to be executed</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL</param>
        /// <returns>A collection of POCO's as a List</returns>
        List<T1> Fetch<T1, T2, T3, T4, T5>(string sql, params object[] args);

        /// <summary>
        ///     Perform a multi-poco fetch
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <param name="sql">An SQL builder object representing the query and it's arguments</param>
        /// <returns>A collection of POCO's as a List</returns>
        List<T1> Fetch<T1, T2>(Sql sql);

        /// <summary>
        ///     Perform a multi-poco fetch
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <typeparam name="T3">The third POCO type</typeparam>
        /// <param name="sql">An SQL builder object representing the query and it's arguments</param>
        /// <returns>A collection of POCO's as a List</returns>
        List<T1> Fetch<T1, T2, T3>(Sql sql);

        /// <summary>
        ///     Perform a multi-poco fetch
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <typeparam name="T3">The third POCO type</typeparam>
        /// <typeparam name="T4">The fourth POCO type</typeparam>
        /// <param name="sql">An SQL builder object representing the query and it's arguments</param>
        /// <returns>A collection of POCO's as a List</returns>
        List<T1> Fetch<T1, T2, T3, T4>(Sql sql);

        /// <summary>
        ///     Perform a multi-poco fetch
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <typeparam name="T3">The third POCO type</typeparam>
        /// <typeparam name="T4">The fourth POCO type</typeparam>
        /// <typeparam name="T5">The fourth POCO type</typeparam>
        /// <param name="sql">An SQL builder object representing the query and it's arguments</param>
        /// <returns>A collection of POCO's as a List</returns>
        List<T1> Fetch<T1, T2, T3, T4, T5>(Sql sql);

        /// <summary> 
        /// Perform a multi-results set query 
        /// </summary> 
        /// <param name="sql">An SQL builder object representing the query and it's arguments</param> 
        /// <returns>A GridReader to be queried</returns> 
        IGridReader QueryMultiple(Sql sql);


        /// <summary> 
        /// Perform a multi-results set query 
        /// </summary> 
        /// <param name="sql">The SQL query to be executed</param> 
        /// <param name="args">Arguments to any embedded parameters in the SQL</param> 
        /// <returns>A GridReader to be queried</returns>
        IGridReader QueryMultiple(string sql, params object[] args);
    }
    #endregion

    #region IMapper
    public interface IMapper
    {
        /// <summary>
        ///     Get information about the table associated with a POCO class
        /// </summary>
        /// <param name="pocoType">The poco type.</param>
        /// <returns>A TableInfo instance</returns>
        /// <remarks>
        ///     This method must return a valid TableInfo.
        ///     To create a TableInfo from a POCO's attributes, use TableInfo.FromPoco
        /// </remarks>
        TableInfo GetTableInfo(Type pocoType);

        /// <summary>
        ///     Get information about the column associated with a property of a POCO
        /// </summary>
        /// <param name="pocoProperty">The PropertyInfo of the property being queried</param>
        /// <returns>A reference to a ColumnInfo instance, or null to ignore this property</returns>
        /// <remarks>
        ///     To create a ColumnInfo from a property's attributes, use PropertyInfo.FromProperty
        /// </remarks>
        ColumnInfo GetColumnInfo(PropertyInfo pocoProperty);

        /// <summary>
        ///     Supply a function to convert a database value to the correct property value
        /// </summary>
        /// <param name="targetProperty">The target property</param>
        /// <param name="sourceType">The type of data returned by the DB</param>
        /// <returns>A Func that can do the conversion, or null for no conversion</returns>
        Func<object, object> GetFromDbConverter(PropertyInfo targetProperty, Type sourceType);

        /// <summary>
        ///     Supply a function to convert a property value into a database value
        /// </summary>
        /// <param name="sourceProperty">The property to be converted</param>
        /// <returns>A Func that can do the conversion</returns>
        /// <remarks>
        ///     This conversion is only used for converting values from POCO's that are
        ///     being Inserted or Updated.
        ///     Conversion is not available for parameter values passed directly to queries.
        /// </remarks>
        Func<object, object> GetToDbConverter(PropertyInfo sourceProperty);
    }
    #endregion

    #region IAlterPoco
    public interface IAlterPoco
    {
        /// <summary>
        ///     Performs an SQL Insert.
        /// </summary>
        /// <param name="tableName">The name of the table to insert into.</param>
        /// <param name="poco">The POCO object that specifies the column values to be inserted.</param>
        /// <returns>The auto allocated primary key of the new record, or null for non-auto-increment tables.</returns>
        object Insert(string tableName, object poco);

        /// <summary>
        ///     Performs an SQL Insert.
        /// </summary>
        /// <param name="tableName">The name of the table to insert into.</param>
        /// <param name="primaryKeyName">The name of the primary key column of the table.</param>
        /// <param name="poco">The POCO object that specifies the column values to be inserted.</param>
        /// <returns>The auto allocated primary key of the new record, or null for non-auto-increment tables.</returns>
        object Insert(string tableName, string primaryKeyName, object poco);

        /// <summary>
        ///     Performs an SQL Insert.
        /// </summary>
        /// <param name="tableName">The name of the table to insert into.</param>
        /// <param name="primaryKeyName">The name of the primary key column of the table.</param>
        /// <param name="autoIncrement">True if the primary key is automatically allocated by the DB.</param>
        /// <param name="poco">The POCO object that specifies the column values to be inserted.</param>
        /// <returns>The auto allocated primary key of the new record, or null for non-auto-increment tables.</returns>
        /// <remarks>
        ///     Inserts a poco into a table. If the poco has a property with the same name
        ///     as the primary key, the id of the new record is assigned to it. Either way,
        ///     the new id is returned.
        /// </remarks>
        object Insert(string tableName, string primaryKeyName, bool autoIncrement, object poco);

        /// <summary>
        ///     Performs an SQL Insert.
        /// </summary>
        /// <param name="poco">The POCO object that specifies the column values to be inserted.</param>
        /// <returns>The auto allocated primary key of the new record, or null for non-auto-increment tables.</returns>
        /// <remarks>
        ///     The name of the table, it's primary key and whether it's an auto-allocated primary key are retrieved
        ///     from the POCO's attributes
        /// </remarks>
        object Insert(object poco);

        /// <summary>
        ///     Performs an SQL update
        /// </summary>
        /// <param name="tableName">The name of the table to update</param>
        /// <param name="primaryKeyName">The name of the primary key column of the table</param>
        /// <param name="poco">The POCO object that specifies the column values to be updated</param>
        /// <param name="primaryKeyValue">The primary key of the record to be updated</param>
        /// <returns>The number of affected records</returns>
        int Update(string tableName, string primaryKeyName, object poco, object primaryKeyValue);

        /// <summary>
        ///     Performs an SQL update
        /// </summary>
        /// <param name="tableName">The name of the table to update</param>
        /// <param name="primaryKeyName">The name of the primary key column of the table</param>
        /// <param name="poco">The POCO object that specifies the column values to be updated</param>
        /// <param name="primaryKeyValue">The primary key of the record to be updated</param>
        /// <param name="columns">The column names of the columns to be updated, or null for all</param>
        /// <returns>The number of affected rows</returns>
        int Update(string tableName, string primaryKeyName, object poco, object primaryKeyValue, IEnumerable<string> columns);

        /// <summary>
        ///     Performs an SQL update
        /// </summary>
        /// <param name="tableName">The name of the table to update</param>
        /// <param name="primaryKeyName">The name of the primary key column of the table</param>
        /// <param name="poco">The POCO object that specifies the column values to be updated</param>
        /// <returns>The number of affected rows</returns>
        int Update(string tableName, string primaryKeyName, object poco);

        /// <summary>
        ///     Performs an SQL update
        /// </summary>
        /// <param name="tableName">The name of the table to update</param>
        /// <param name="primaryKeyName">The name of the primary key column of the table</param>
        /// <param name="poco">The POCO object that specifies the column values to be updated</param>
        /// <param name="columns">The column names of the columns to be updated, or null for all</param>
        /// <returns>The number of affected rows</returns>
        int Update(string tableName, string primaryKeyName, object poco, IEnumerable<string> columns);

        /// <summary>
        ///     Performs an SQL update
        /// </summary>
        /// <param name="poco">The POCO object that specifies the column values to be updated</param>
        /// <param name="columns">The column names of the columns to be updated, or null for all</param>
        /// <returns>The number of affected rows</returns>
        int Update(object poco, IEnumerable<string> columns);

        /// <summary>
        ///     Performs an SQL update
        /// </summary>
        /// <param name="poco">The POCO object that specifies the column values to be updated</param>
        /// <returns>The number of affected rows</returns>
        int Update(object poco);

        /// <summary>
        ///     Performs an SQL update
        /// </summary>
        /// <param name="poco">The POCO object that specifies the column values to be updated</param>
        /// <param name="primaryKeyValue">The primary key of the record to be updated</param>
        /// <returns>The number of affected rows</returns>
        int Update(object poco, object primaryKeyValue);

        /// <summary>
        ///     Performs an SQL update
        /// </summary>
        /// <param name="poco">The POCO object that specifies the column values to be updated</param>
        /// <param name="primaryKeyValue">The primary key of the record to be updated</param>
        /// <param name="columns">The column names of the columns to be updated, or null for all</param>
        /// <returns>The number of affected rows</returns>
        int Update(object poco, object primaryKeyValue, IEnumerable<string> columns);

        /// <summary>
        ///     Performs an SQL update
        /// </summary>
        /// <typeparam name="T">The POCO class who's attributes specify the name of the table to update</typeparam>
        /// <param name="sql">The SQL update and condition clause (ie: everything after "UPDATE tablename"</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL</param>
        /// <returns>The number of affected rows</returns>
        int Update<T>(string sql, params object[] args);

        /// <summary>
        ///     Performs an SQL update
        /// </summary>
        /// <typeparam name="T">The POCO class who's attributes specify the name of the table to update</typeparam>
        /// <param name="sql">
        ///     An SQL builder object representing the SQL update and condition clause (ie: everything after "UPDATE
        ///     tablename"
        /// </param>
        /// <returns>The number of affected rows</returns>
        int Update<T>(Sql sql);

        /// <summary>
        ///     Performs and SQL Delete
        /// </summary>
        /// <param name="tableName">The name of the table to delete from</param>
        /// <param name="primaryKeyName">The name of the primary key column</param>
        /// <param name="poco">The POCO object whose primary key value will be used to delete the row</param>
        /// <returns>The number of rows affected</returns>
        int Delete(string tableName, string primaryKeyName, object poco);

        /// <summary>
        ///     Performs and SQL Delete
        /// </summary>
        /// <param name="tableName">The name of the table to delete from</param>
        /// <param name="primaryKeyName">The name of the primary key column</param>
        /// <param name="poco">
        ///     The POCO object whose primary key value will be used to delete the row (or null to use the supplied
        ///     primary key value)
        /// </param>
        /// <param name="primaryKeyValue">
        ///     The value of the primary key identifing the record to be deleted (or null, or get this
        ///     value from the POCO instance)
        /// </param>
        /// <returns>The number of rows affected</returns>
        int Delete(string tableName, string primaryKeyName, object poco, object primaryKeyValue);

        /// <summary>
        ///     Performs an SQL Delete
        /// </summary>
        /// <param name="poco">The POCO object specifying the table name and primary key value of the row to be deleted</param>
        /// <returns>The number of rows affected</returns>
        int Delete(object poco);

        /// <summary>
        ///     Performs an SQL Delete
        /// </summary>
        /// <typeparam name="T">The POCO class whose attributes identify the table and primary key to be used in the delete</typeparam>
        /// <param name="pocoOrPrimaryKey">The value of the primary key of the row to delete</param>
        /// <returns></returns>
        int Delete<T>(object pocoOrPrimaryKey);

        /// <summary>
        ///     Performs an SQL Delete
        /// </summary>
        /// <typeparam name="T">The POCO class who's attributes specify the name of the table to delete from</typeparam>
        /// <param name="sql">The SQL condition clause identifying the row to delete (ie: everything after "DELETE FROM tablename"</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL</param>
        /// <returns>The number of affected rows</returns>
        int Delete<T>(string sql, params object[] args);

        /// <summary>
        ///     Performs an SQL Delete
        /// </summary>
        /// <typeparam name="T">The POCO class who's attributes specify the name of the table to delete from</typeparam>
        /// <param name="sql">
        ///     An SQL builder object representing the SQL condition clause identifying the row to delete (ie:
        ///     everything after "UPDATE tablename"
        /// </param>
        /// <returns>The number of affected rows</returns>
        int Delete<T>(Sql sql);

        /// <summary>
        ///     Check if a poco represents a new row
        /// </summary>
        /// <param name="primaryKeyName">The name of the primary key column</param>
        /// <param name="poco">The object instance whose "newness" is to be tested</param>
        /// <returns>True if the POCO represents a record already in the database</returns>
        /// <remarks>This method simply tests if the POCO's primary key column property has been set to something non-zero.</remarks>
        bool IsNew(string primaryKeyName, object poco);

        /// <summary>
        ///     Check if a poco represents a new row
        /// </summary>
        /// <param name="poco">The object instance whose "newness" is to be tested</param>
        /// <returns>True if the POCO represents a record already in the database</returns>
        /// <remarks>This method simply tests if the POCO's primary key column property has been set to something non-zero.</remarks>
        bool IsNew(object poco);

        /// <summary>
        ///     Saves a POCO by either performing either an SQL Insert or SQL Update
        /// </summary>
        /// <param name="tableName">The name of the table to be updated</param>
        /// <param name="primaryKeyName">The name of the primary key column</param>
        /// <param name="poco">The POCO object to be saved</param>
        void Save(string tableName, string primaryKeyName, object poco);

        /// <summary>
        ///     Saves a POCO by either performing either an SQL Insert or SQL Update
        /// </summary>
        /// <param name="poco">The POCO object to be saved</param>
        void Save(object poco);
    }
    #endregion

    #region IExecute
    public interface IExecute
    {
        /// <summary>
        ///     Executes a non-query command
        /// </summary>
        /// <param name="sql">The SQL statement to execute</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL</param>
        /// <returns>The number of rows affected</returns>
        int Execute(string sql, params object[] args);

        /// <summary>
        ///     Executes a non-query command
        /// </summary>
        /// <param name="sql">An SQL builder object representing the query and it's arguments</param>
        /// <returns>The number of rows affected</returns>
        int Execute(Sql sql);

        /// <summary>
        ///     Executes a query and return the first column of the first row in the result set.
        /// </summary>
        /// <typeparam name="T">The type that the result value should be cast to</typeparam>
        /// <param name="sql">The SQL query to execute</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL</param>
        /// <returns>The scalar value cast to T</returns>
        T ExecuteScalar<T>(string sql, params object[] args);

        /// <summary>
        ///     Executes a query and return the first column of the first row in the result set.
        /// </summary>
        /// <typeparam name="T">The type that the result value should be cast to</typeparam>
        /// <param name="sql">An SQL builder object representing the query and it's arguments</param>
        /// <returns>The scalar value cast to T</returns>
        T ExecuteScalar<T>(Sql sql);
    }
    #endregion

    #region ITransactionAccessor
    /// <summary>
    ///     Represents a contract which exposes the current <see cref="IDbTransaction" /> instance.
    /// </summary>
    public interface ITransactionAccessor
    {
        /// <summary>
        ///     Gets the current transaction instance.
        /// </summary>
        /// <returns>
        ///     The current transaction instance; else, <c>null</c> if not transaction is in progress.
        /// </returns>
        IDbTransaction Transaction { get; }
    }
    #endregion

    #region IDatabaseBuildConfiguration

    /// <summary>
    ///     A helper interface which enables fluent configuration extension methods.
    /// </summary>
    public interface IDatabaseBuildConfiguration
    {
        //IDatabaseBuildConfiguration UsingProviderName(string providerName);
    }
    #endregion

    #region IBuildConfigurationSettings

    /// <summary>
    ///     Represents the build configuration settings contract.
    /// </summary>
    public interface IBuildConfigurationSettings
    {
        /// <summary>
        ///     Sets the setting against the specified key.
        /// </summary>
        /// <param name="key">The setting's key.</param>
        /// <param name="value">The setting's value.</param>
        void SetSetting(string key, object value);

        /// <summary>
        ///     Tries to get the setting and calls the <paramref name="setSetting" /> to set the value if found.
        /// </summary>
        /// <typeparam name="T">The setting type.</typeparam>
        /// <param name="key">The setting's key.</param>
        /// <param name="setSetting">The set setting callback.</param>
        /// <param name="onFail">The on fail callback, called when no setting can be set.</param>
        void TryGetSetting<T>(string key, Action<T> setSetting, Action onFail = null);
    }
    #endregion
}
