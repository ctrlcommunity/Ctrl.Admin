using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using Ctrl.Core.Core.Log;
using Ctrl.Core.Core.Utils;
using Ctrl.Core.PetaPoco.Core;
using Ctrl.Core.PetaPoco.Core.Inflection;
using Ctrl.Core.PetaPoco.Utilities;
#if !NETSTANDARD
using System.Configuration;
#endif
namespace Ctrl.Core.PetaPoco {
    /// <summary>
    ///     The main PetaPoco Database class.  You can either use this class directly, or derive from it.
    /// </summary>
    public class Database : IDatabase {
        #region 写入日志
        /// <summary>
        ///     写入SqlLog日志
        /// </summary>
        /// <returns></returns>
        public static void WriteSqlLog (SqlLog log) {
            SqlLogHandler handler = new SqlLogHandler (log.OperateSql, Convert.ToDateTime (log.EndDateTime), log.ElapsedTime, log.Parameter);
            handler.WriteLog ();
        }

        #endregion
        #region IDisposable

        /// <summary>
        ///     Automatically close one open shared connection
        /// </summary>
        public void Dispose () {
            // Automatically close one open connection reference
            //  (Works with KeepConnectionAlive and manually opening a shared connection)
            CloseSharedConnection ();
        }

        #endregion

        #region Internal operations

        internal void DoPreExecute (IDbCommand cmd) {
            // Setup command timeout
            if (OneTimeCommandTimeout != 0) {
                cmd.CommandTimeout = OneTimeCommandTimeout;
                OneTimeCommandTimeout = 0;
            } else if (CommandTimeout != 0) {
                cmd.CommandTimeout = CommandTimeout;
            }

            // Call hook
            OnExecutingCommand (cmd);

            // Save it
            _lastSql = cmd.CommandText;
            _lastArgs = (from IDataParameter parameter in cmd.Parameters select parameter.Value).ToArray ();
        }

        #endregion

        #region Constructors

#if !NETSTANDARD&&!NETCOREAPP
        /// <summary>
        ///     Constructs an instance using the first connection string found in the app/web configuration file.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when no connection strings can registered.</exception>
        public Database () {
            if (ConfigurationManager.ConnectionStrings.Count == 0)
                throw new InvalidOperationException ("One or more connection strings must be registered to use the no paramater constructor");

            var entry = ConfigurationManager.ConnectionStrings[0];
            _connectionString = entry.ConnectionString;
            var providerName = !string.IsNullOrEmpty (entry.ProviderName) ? entry.ProviderName : "System.Data.SqlClient";
            Initialise (DatabaseProvider.Resolve (providerName, false, _connectionString), null);
        }

        /// <summary>
        ///     Constructs an instance using a supplied connection string name. The actual connection string and provider will be
        ///     read from app/web.config.
        /// </summary>
        /// <param name="connectionStringName">The name of the connection.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="connectionStringName" /> is null or empty.</exception>
        /// <exception cref="InvalidOperationException">Thrown when a connection string cannot be found.</exception>
        public Database (string connectionStringName) {
            if (string.IsNullOrEmpty (connectionStringName))
                throw new ArgumentException ("Connection string name must not be null or empty", nameof (connectionStringName));

            var entry = ConfigurationManager.ConnectionStrings[connectionStringName];

            if (entry == null)
                throw new InvalidOperationException (string.Format ("Can't find a connection string with the name '{0}'", connectionStringName));

            _connectionString = entry.ConnectionString;
            var providerName = !string.IsNullOrEmpty (entry.ProviderName) ? entry.ProviderName : "System.Data.SqlClient";
            Initialise (DatabaseProvider.Resolve (providerName, false, _connectionString), null);
        }
#endif

        /// <summary>
        ///     Constructs an instance using a supplied IDbConnection.
        /// </summary>
        /// <param name="connection">The IDbConnection to use.</param>
        /// <param name="defaultMapper">The default mapper to use when no specific mapper has been registered.</param>
        /// <remarks>
        ///     The supplied IDbConnection will not be closed/disposed by PetaPoco - that remains
        ///     the responsibility of the caller.
        /// </remarks>
        /// <exception cref="ArgumentException">Thrown when <paramref name="connection" /> is null or empty.</exception>
        public Database (IDbConnection connection, IMapper defaultMapper = null) {
            _sharedConnection = connection ??
                throw new ArgumentNullException (nameof (connection));
            _connectionString = connection.ConnectionString;

            // Prevent closing external connection
            _sharedConnectionDepth = 2;

            Initialise (DatabaseProvider.Resolve (_sharedConnection.GetType (), false, _connectionString), defaultMapper);
        }

        /// <summary>
        ///     Constructs an instance using a supplied connections string and provider name.
        /// </summary>
        /// <param name="connectionString">The database connection string.</param>
        /// <param name="providerName">The database provider name.</param>
        /// <remarks>
        ///     PetaPoco will automatically close and dispose any connections it creates.
        /// </remarks>
        /// <exception cref="ArgumentException">Thrown when <paramref name="connectionString" /> is null or empty.</exception>
        public Database (string connectionString, string providerName) {
            if (string.IsNullOrEmpty (connectionString))
                throw new ArgumentException ("Connection string cannot be null or empty", nameof (connectionString));

            _connectionString = connectionString;
            Initialise (DatabaseProvider.Resolve (providerName, true, _connectionString), null);
        }

        /// <summary>
        ///     Constructs an instance using the supplied connection string and DbProviderFactory.
        /// </summary>
        /// <param name="connectionString">The database connection string.</param>
        /// <param name="factory">The DbProviderFactory to use for instantiating IDbConnection's.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="connectionString" /> is null or empty.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="factory" /> is null.</exception>
        public Database (string connectionString, DbProviderFactory factory) {
            if (string.IsNullOrEmpty (connectionString))
                throw new ArgumentException ("Connection string must not be null or empty", nameof (connectionString));

            if (factory == null)
                throw new ArgumentNullException (nameof (factory));

            _connectionString = connectionString;
            Initialise (DatabaseProvider.Resolve (DatabaseProvider.Unwrap (factory).GetType (), false, _connectionString), null);
        }

        /// <summary>
        ///     Constructs an instance using the supplied provider and optional default mapper.
        /// </summary>
        /// <param name="connectionString">The database connection string.</param>
        /// <param name="provider">The provider to use.</param>
        /// <param name="defaultMapper">The default mapper to use when no specific mapper has been registered.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="connectionString" /> is null or empty.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="provider" /> is null.</exception>
        public Database (string connectionString, IProvider provider, IMapper defaultMapper = null) {
            if (string.IsNullOrEmpty (connectionString))
                throw new ArgumentException ("Connection string must not be null or empty", nameof (connectionString));

            if (provider == null)
                throw new ArgumentNullException (nameof (provider));

            _connectionString = connectionString;
            Initialise (provider, defaultMapper);
        }

        /// <summary>
        ///     Constructs an instance using the supplied <paramref name="configuration" />.
        /// </summary>
        /// <param name="configuration">The configuration for constructing an instance.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="configuration" /> is null.</exception>
        /// <exception cref="InvalidOperationException">
        ///     Thrown when no configuration string is configured and app/web config does
        ///     any connection string registered.
        /// </exception>
        /// <exception cref="InvalidOperationException">Thrown when a connection string configured and no provider is configured.</exception>
        public Database (IDatabaseBuildConfiguration configuration) {
            if (configuration == null)
                throw new ArgumentNullException (nameof (configuration));

            var settings = (IBuildConfigurationSettings) configuration;

            IMapper defaultMapper = null;
            settings.TryGetSetting<IMapper> (DatabaseConfigurationExtensions.DefaultMapper, v => defaultMapper = v);

#if !NETSTANDARD&&!NETCOREAPP
            ConnectionStringSettings entry = null;
            settings.TryGetSetting<string> (DatabaseConfigurationExtensions.ConnectionString, cs => _connectionString = cs, () => {
                settings.TryGetSetting<string> (DatabaseConfigurationExtensions.ConnectionStringName, cn => {
                    entry = ConfigurationManager.ConnectionStrings[cn];

                    if (entry == null)
                        throw new InvalidOperationException (string.Format ("Can't find a connection string with the name '{0}'", cn));
                }, () => {
                    if (ConfigurationManager.ConnectionStrings.Count == 0)
                        throw new InvalidOperationException ("One or more connection strings must be registered, when not providing a connection string");

                    entry = ConfigurationManager.ConnectionStrings[0];
                });

                // ReSharper disable once PossibleNullReferenceException
                _connectionString = entry.ConnectionString;
            });

            settings.TryGetSetting<IProvider> (DatabaseConfigurationExtensions.Provider, v => Initialise (v, defaultMapper), () => {
                if (entry == null)
                    throw new InvalidOperationException ("Both a connection string and provider are required or neither.");

                var providerName = !string.IsNullOrEmpty (entry.ProviderName) ? entry.ProviderName : "System.Data.SqlClient";
                Initialise (DatabaseProvider.Resolve (providerName, false, _connectionString), defaultMapper);
            });
#else
            settings.TryGetSetting<string> (DatabaseConfigurationExtensions.ConnectionString, cs => _connectionString = cs, () =>
                throw new InvalidOperationException ("A connection string is required."));
            settings.TryGetSetting<IProvider> (DatabaseConfigurationExtensions.Provider, v => Initialise (v, defaultMapper), () => {
                settings.TryGetSetting<string> (DatabaseConfigurationExtensions.ProviderName,
                    v => Initialise (DatabaseProvider.Resolve (v, false, _connectionString), defaultMapper),
                    () =>
                    throw new InvalidOperationException ("Either a provider name or provider must be registered."));
            });
#endif

            settings.TryGetSetting<bool> (DatabaseConfigurationExtensions.EnableNamedParams, v => EnableNamedParams = v);
            settings.TryGetSetting<bool> (DatabaseConfigurationExtensions.EnableAutoSelect, v => EnableAutoSelect = v);
            settings.TryGetSetting<int> (DatabaseConfigurationExtensions.CommandTimeout, v => CommandTimeout = v);
            settings.TryGetSetting<IsolationLevel> (DatabaseConfigurationExtensions.IsolationLevel, v => IsolationLevel = v);
        }

        /// <summary>
        ///     Provides common initialization for the various constructors.
        /// </summary>
        private void Initialise (IProvider provider, IMapper mapper) {
            // Reset
            _transactionDepth = 0;
            EnableAutoSelect = true;
            EnableNamedParams = true;

            // What character is used for delimiting parameters in SQL
            _provider = provider;
            _paramPrefix = _provider.GetParameterPrefix (_connectionString);
            _factory = _provider.GetFactory ();

            _defaultMapper = mapper ?? new ConventionMapper ();
        }

        #endregion

        #region Connection Management

        /// <summary>
        ///     When set to true the first opened connection is kept alive until this object is disposed
        /// </summary>
        public bool KeepConnectionAlive { get; set; }

        /// <summary>
        ///     Open a connection that will be used for all subsequent queries.
        /// </summary>
        /// <remarks>
        ///     Calls to Open/CloseSharedConnection are reference counted and should be balanced
        /// </remarks>
        public void OpenSharedConnection () {
            if (_sharedConnectionDepth == 0) {
                _sharedConnection = _factory.CreateConnection ();
                _sharedConnection.ConnectionString = _connectionString;

                if (_sharedConnection.State == ConnectionState.Broken)
                    _sharedConnection.Close ();

                if (_sharedConnection.State == ConnectionState.Closed)
                    _sharedConnection.Open ();

                _sharedConnection = OnConnectionOpened (_sharedConnection);

                if (KeepConnectionAlive)
                    _sharedConnectionDepth++; // Make sure you call Dispose
            }

            _sharedConnectionDepth++;
        }

        /// <summary>
        ///     Releases the shared connection
        /// </summary>
        public void CloseSharedConnection () {
            if (_sharedConnectionDepth > 0) {
                _sharedConnectionDepth--;
                if (_sharedConnectionDepth == 0) {
                    OnConnectionClosing (_sharedConnection);
                    _sharedConnection.Dispose ();
                    _sharedConnection = null;
                }
            }
        }

        /// <summary>
        ///     Provides access to the currently open shared connection (or null if none)
        /// </summary>
        public IDbConnection Connection => _sharedConnection;

        #endregion

        #region Transaction Management

        /// <summary>
        ///     Gets the current transaction instance.
        /// </summary>
        /// <returns>
        ///     The current transaction instance; else, <c>null</c> if not transaction is in progress.
        /// </returns>
        IDbTransaction ITransactionAccessor.Transaction => _transaction;

        // Helper to create a transaction scope

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
        public ITransaction GetTransaction () => new Transaction (this);

        /// <summary>
        ///     Called when a transaction starts.  Overridden by the T4 template generated database
        ///     classes to ensure the same DB instance is used throughout the transaction.
        /// </summary>
        public virtual void OnBeginTransaction () { }

        /// <summary>
        ///     Called when a transaction ends.
        /// </summary>
        public virtual void OnEndTransaction () { }

        /// <summary>
        ///     Starts a transaction scope, see GetTransaction() for recommended usage
        /// </summary>
        public void BeginTransaction () {
            _transactionDepth++;

            if (_transactionDepth == 1) {
                OpenSharedConnection ();
                _transaction = !_isolationLevel.HasValue ? _sharedConnection.BeginTransaction () : _sharedConnection.BeginTransaction (_isolationLevel.Value);
                _transactionCancelled = false;
                OnBeginTransaction ();
            }
        }

        /// <summary>
        ///     Internal helper to cleanup transaction
        /// </summary>
        private void CleanupTransaction () {
            OnEndTransaction ();

            if (_transactionCancelled)
                _transaction.Rollback ();
            else
                _transaction.Commit ();

            _transaction.Dispose ();
            _transaction = null;

            CloseSharedConnection ();
        }

        /// <summary>
        ///     Aborts the entire outer most transaction scope
        /// </summary>
        /// <remarks>
        ///     Called automatically by Transaction.Dispose()
        ///     if the transaction wasn't completed.
        /// </remarks>
        public void AbortTransaction () {
            _transactionCancelled = true;
            if ((--_transactionDepth) == 0)
                CleanupTransaction ();
        }

        /// <summary>
        ///     Marks the current transaction scope as complete.
        /// </summary>
        public void CompleteTransaction () {
            if ((--_transactionDepth) == 0)
                CleanupTransaction ();
        }

        #endregion

        #region Command Management

        /// <summary>
        ///     Add a parameter to a DB command
        /// </summary>
        /// <param name="cmd">A reference to the IDbCommand to which the parameter is to be added</param>
        /// <param name="value">The value to assign to the parameter</param>
        /// <param name="pi">Optional, a reference to the property info of the POCO property from which the value is coming.</param>
        private void AddParam (IDbCommand cmd, object value, PropertyInfo pi) {
            // Convert value to from poco type to db type
            if (pi != null) {
                var mapper = Mappers.GetMapper (pi.DeclaringType, _defaultMapper);
                var fn = mapper.GetToDbConverter (pi);
                if (fn != null)
                    value = fn (value);
            }

            // Support passed in parameters
            var idbParam = value as IDbDataParameter;
            if (idbParam != null) {
                idbParam.ParameterName = string.Format ("{0}{1}", _paramPrefix, cmd.Parameters.Count);
                cmd.Parameters.Add (idbParam);
                return;
            }

            // Create the parameter
            var p = cmd.CreateParameter ();
            p.ParameterName = string.Format ("{0}{1}", _paramPrefix, cmd.Parameters.Count);

            // Assign the parmeter value
            if (value == null) {
                p.Value = DBNull.Value;

                if (pi != null && pi.PropertyType.Name == "Byte[]") {
                    p.DbType = DbType.Binary;
                }
            } else {
                // Give the database type first crack at converting to DB required type
                value = _provider.MapParameterValue (value);

                var t = value.GetType ();
                if (t.IsEnum) // PostgreSQL .NET driver wont cast enum to int
                {
                    p.Value = Convert.ChangeType (value, ((Enum) value).GetTypeCode ());
                } else if (t == typeof (Guid) && !_provider.HasNativeGuidSupport) {
                    p.Value = value.ToString ();
                    p.DbType = DbType.String;
                    p.Size = 40;
                } else if (t == typeof (string)) {
                    // out of memory exception occurs if trying to save more than 4000 characters to SQL Server CE NText column. Set before attempting to set Size, or Size will always max out at 4000
                    if ((value as string).Length + 1 > 4000 && p.GetType ().Name == "SqlCeParameter")
                        p.GetType ().GetProperty ("SqlDbType").SetValue (p, SqlDbType.NText, null);

                    p.Size = Math.Max ((value as string).Length + 1, 4000); // Help query plan caching by using common size
                    p.Value = value;
                } else if (t == typeof (AnsiString)) {
                    // Thanks @DataChomp for pointing out the SQL Server indexing performance hit of using wrong string type on varchar
                    p.Size = Math.Max ((value as AnsiString).Value.Length + 1, 4000);
                    p.Value = (value as AnsiString).Value;
                    p.DbType = DbType.AnsiString;
                } else if (value.GetType ().Name == "SqlGeography") //SqlGeography is a CLR Type
                {
                    p.GetType ().GetProperty ("UdtTypeName").SetValue (p, "geography", null); //geography is the equivalent SQL Server Type
                    p.Value = value;
                } else if (value.GetType ().Name == "SqlGeometry") //SqlGeometry is a CLR Type
                {
                    p.GetType ().GetProperty ("UdtTypeName").SetValue (p, "geometry", null); //geography is the equivalent SQL Server Type
                    p.Value = value;
                } else {
                    p.Value = value;
                }
            }

            // Add to the collection
            cmd.Parameters.Add (p);
        }

        // Create a command
        private static Regex rxParamsPrefix = new Regex (@"(?<!@)@\w+", RegexOptions.Compiled);

        public IDbCommand CreateCommand (IDbConnection connection, string sql, params object[] args) {
            // Perform named argument replacements
            if (EnableNamedParams) {
                var new_args = new List<object> ();
                sql = ParametersHelper.ProcessParams (sql, args, new_args);
                args = new_args.ToArray ();
            }

            // Perform parameter prefix replacements
            if (_paramPrefix != "@")
                sql = rxParamsPrefix.Replace (sql, m => _paramPrefix + m.Value.Substring (1));
            sql = sql.Replace ("@@", "@"); // <- double @@ escapes a single @

            // Create the command and add parameters
            IDbCommand cmd = connection.CreateCommand ();
            cmd.Connection = connection;
            cmd.CommandText = sql;
            cmd.Transaction = _transaction;
            foreach (var item in args) {
                AddParam (cmd, item, null);
            }

            // Notify the DB type
            _provider.PreExecute (cmd);

            // Call logging
            if (!String.IsNullOrEmpty (sql))
                DoPreExecute (cmd);

            return cmd;
        }

        #endregion

        #region Exception Reporting and Logging

        /// <summary>
        ///     Called if an exception occurs during processing of a DB operation.  Override to provide custom logging/handling.
        /// </summary>
        /// <param name="x">The exception instance</param>
        /// <returns>True to re-throw the exception, false to suppress it</returns>
        public virtual bool OnException (Exception x) {
            System.Diagnostics.Debug.WriteLine (x.ToString ());
            System.Diagnostics.Debug.WriteLine (LastCommand);
            return true;
        }

        /// <summary>
        ///     Called when DB connection opened
        /// </summary>
        /// <param name="conn">The newly opened IDbConnection</param>
        /// <returns>The same or a replacement IDbConnection</returns>
        /// <remarks>
        ///     Override this method to provide custom logging of opening connection, or
        ///     to provide a proxy IDbConnection.
        /// </remarks>
        public virtual IDbConnection OnConnectionOpened (IDbConnection conn) {
            return conn;
        }

        /// <summary>
        ///     Called when DB connection closed
        /// </summary>
        /// <param name="conn">The soon to be closed IDBConnection</param>
        public virtual void OnConnectionClosing (IDbConnection conn) { }

        /// <summary>
        ///     Called just before an DB command is executed
        /// </summary>
        /// <param name="cmd">The command to be executed</param>
        /// <remarks>
        ///     Override this method to provide custom logging of commands and/or
        ///     modification of the IDbCommand before it's executed
        /// </remarks>
        public virtual void OnExecutingCommand (IDbCommand cmd) { }

        /// <summary>
        ///     Called on completion of command execution
        /// </summary>
        /// <param name="cmd">The IDbCommand that finished executing</param>
        public virtual void OnExecutedCommand (IDbCommand cmd) { }

        #endregion

        #region operation : Execute 

        /// <summary>
        ///     Executes a non-query command
        /// </summary>
        /// <param name="sql">The SQL statement to execute</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL</param>
        /// <returns>The number of rows affected</returns>
        public int Execute (string sql, params object[] args) {
            try {
                OpenSharedConnection ();
                try {
                    using (var cmd = CreateCommand (_sharedConnection, sql, args)) {
                        var retv = cmd.ExecuteNonQuery ();
                        OnExecutedCommand (cmd);
                        return retv;
                    }
                } finally {
                    CloseSharedConnection ();
                }
            } catch (Exception x) {
                if (OnException (x))
                    throw;
                return -1;
            }
        }

        /// <summary>
        ///     Executes a non-query command
        /// </summary>
        /// <param name="sql">An SQL builder object representing the query and it's arguments</param>
        /// <returns>The number of rows affected</returns>
        public int Execute (Sql sql) {
            return Execute (sql.SQL, sql.Arguments);
        }

        #endregion

        #region operation : ExecuteScalar

        /// <summary>
        ///     Executes a query and return the first column of the first row in the result set.
        /// </summary>
        /// <typeparam name="T">The type that the result value should be cast to</typeparam>
        /// <param name="sql">The SQL query to execute</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL</param>
        /// <returns>The scalar value cast to T</returns>
        public T ExecuteScalar<T> (string sql, params object[] args) {
            try {
                Stopwatch stopwatch = CommonHelper.TimerStart ();
                OpenSharedConnection ();
                try {
                    using (var cmd = CreateCommand (_sharedConnection, sql, args)) {
                        object val = cmd.ExecuteScalar ();
                        OnExecutedCommand (cmd);

                        // Handle nullable types
                        Type u = Nullable.GetUnderlyingType (typeof (T));
                        if (u != null && (val == null || val == DBNull.Value))
                            return default (T);
                        SqlLog log = new SqlLog {
                            CreateTime = DateTime.Now.ToString ("yyyy-MM-dd HH:mm:ss"),
                            OperateSql = sql,
                            Parameter = args == null ? "" : string.Join (",", args)
                        };
                        var result = (T) Convert.ChangeType (val, u == null ? typeof (T) : u);
                        log.EndDateTime = DateTime.Now.ToString ("yyyy-MM-dd HH:mm:ss");
                        log.ElapsedTime = CommonHelper.TimerEnd (stopwatch);
                        WriteSqlLog (log);
                        return result;
                    }
                } finally {
                    CloseSharedConnection ();
                }
            } catch (Exception x) {
                if (OnException (x))
                    throw;
                return default (T);
            }
        }

        /// <summary>
        ///     Executes a query and return the first column of the first row in the result set.
        /// </summary>
        /// <typeparam name="T">The type that the result value should be cast to</typeparam>
        /// <param name="sql">An SQL builder object representing the query and it's arguments</param>
        /// <returns>The scalar value cast to T</returns>
        public T ExecuteScalar<T> (Sql sql) {
            return ExecuteScalar<T> (sql.SQL, sql.Arguments);
        }

        #endregion

        #region operation : Fetch

        /// <summary>
        ///     Runs a SELECT * query and returns the result set as a typed list
        /// </summary>
        /// <typeparam name="T">The Type representing a row in the result set</typeparam>
        /// <returns>A List holding the results of the query</returns>
        public List<T> Fetch<T> () => Fetch<T> (String.Empty);

        /// <summary>
        ///     Runs a query and returns the result set as a typed list
        /// </summary>
        /// <typeparam name="T">The Type representing a row in the result set</typeparam>
        /// <param name="sql">The SQL query to execute</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL</param>
        /// <returns>A List holding the results of the query</returns>
        public List<T> Fetch<T> (string sql, params object[] args) {
            return Query<T> (sql, args).ToList ();
        }

        /// <summary>
        ///     Runs a query and returns the result set as a typed list
        /// </summary>
        /// <typeparam name="T">The Type representing a row in the result set</typeparam>
        /// <param name="sql">An SQL builder object representing the query and it's arguments</param>
        /// <returns>A List holding the results of the query</returns>
        public List<T> Fetch<T> (Sql sql) {
            return Fetch<T> (sql.SQL, sql.Arguments);
        }

        #endregion

        #region operation : Page

        /// <summary>
        ///     Starting with a regular SELECT statement, derives the SQL statements required to query a
        ///     DB for a page of records and the total number of records
        /// </summary>
        /// <typeparam name="T">The Type representing a row in the result set</typeparam>
        /// <param name="skip">The number of rows to skip before the start of the page</param>
        /// <param name="take">The number of rows in the page</param>
        /// <param name="sql">The original SQL select statement</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL</param>
        /// <param name="sqlCount">Outputs the SQL statement to query for the total number of matching rows</param>
        /// <param name="sqlPage">Outputs the SQL statement to retrieve a single page of matching rows</param>
        private void BuildPageQueries<T> (long skip, long take, string sql, ref object[] args, out string sqlCount, out string sqlPage) {
            // Add auto select clause
            if (EnableAutoSelect)
                sql = AutoSelectHelper.AddSelectClause<T> (_provider, sql, _defaultMapper);

            // Split the SQL
            SQLParts parts;
            if (!Provider.PagingUtility.SplitSQL (sql, out parts))
                throw new Exception ("Unable to parse SQL statement for paged query");

            sqlPage = _provider.BuildPageQuery (skip, take, parts, ref args);
            sqlCount = parts.SqlCount;
        }

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
        public Page<T> Page<T> (long page, long itemsPerPage, string sqlCount, object[] countArgs, string sqlPage, object[] pageArgs) {
            // Save the one-time command time out and use it for both queries
            var saveTimeout = OneTimeCommandTimeout;

            // Setup the paged result
            var result = new Page<T> {
                CurrentPage = page,
                ItemsPerPage = itemsPerPage,
                TotalItems = ExecuteScalar<long> (sqlCount, countArgs)
            };
            result.TotalPages = result.TotalItems / itemsPerPage;

            if ((result.TotalItems % itemsPerPage) != 0)
                result.TotalPages++;

            OneTimeCommandTimeout = saveTimeout;

            // Get the records
            result.Items = Fetch<T> (sqlPage, pageArgs);

            // Done
            return result;
        }

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
        public Page<T> Page<T> (long page, long itemsPerPage) => Page<T> (page, itemsPerPage, String.Empty);

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
        public Page<T> Page<T> (long page, long itemsPerPage, string sql, params object[] args) {
            string sqlCount, sqlPage;
            BuildPageQueries<T> ((page - 1) * itemsPerPage, itemsPerPage, sql, ref args, out sqlCount, out sqlPage);
            return Page<T> (page, itemsPerPage, sqlCount, args, sqlPage, args);
        }

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
        public Page<T> Page<T> (long page, long itemsPerPage, Sql sql) {
            return Page<T> (page, itemsPerPage, sql.SQL, sql.Arguments);
        }

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
        public Page<T> Page<T> (long page, long itemsPerPage, Sql sqlCount, Sql sqlPage) {
            return Page<T> (page, itemsPerPage, sqlCount.SQL, sqlCount.Arguments, sqlPage.SQL, sqlPage.Arguments);
        }

        #endregion

        #region operation : Fetch (page)

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
        public List<T> Fetch<T> (long page, long itemsPerPage) => Fetch<T> (page, itemsPerPage, String.Empty);

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
        public List<T> Fetch<T> (long page, long itemsPerPage, string sql, params object[] args) {
            return SkipTake<T> ((page - 1) * itemsPerPage, itemsPerPage, sql, args);
        }

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
        public List<T> Fetch<T> (long page, long itemsPerPage, Sql sql) {
            return SkipTake<T> ((page - 1) * itemsPerPage, itemsPerPage, sql.SQL, sql.Arguments);
        }

        #endregion

        #region operation : SkipTake

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
        public List<T> SkipTake<T> (long skip, long take) => SkipTake<T> (skip, take, String.Empty);

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
        public List<T> SkipTake<T> (long skip, long take, string sql, params object[] args) {
            string sqlCount, sqlPage;
            BuildPageQueries<T> (skip, take, sql, ref args, out sqlCount, out sqlPage);
            return Fetch<T> (sqlPage, args);
        }

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
        public List<T> SkipTake<T> (long skip, long take, Sql sql) {
            return SkipTake<T> (skip, take, sql.SQL, sql.Arguments);
        }

        #endregion

        #region operation : Query

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
        public IEnumerable<T> Query<T> () => Query<T> (String.Empty);

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
        public IEnumerable<T> Query<T> (string sql, params object[] args) {
            Stopwatch stopwatch = CommonHelper.TimerStart ();
            if (EnableAutoSelect)
                sql = AutoSelectHelper.AddSelectClause<T> (_provider, sql, _defaultMapper);

            OpenSharedConnection ();
            try {
                using (var cmd = CreateCommand (_sharedConnection, sql, args)) {
                    IDataReader r;
                    var pd = PocoData.ForType (typeof (T), _defaultMapper);
                    try {
                        SqlLog log = new SqlLog {
                            CreateTime = DateTime.Now.ToString ("yyyy-MM-dd HH:mm:ss"),
                            OperateSql = sql,
                            Parameter = args == null ? "" : string.Join (",", args)
                        };
                        r =  cmd.ExecuteReader ();
                        log.EndDateTime = DateTime.Now.ToString ("yyyy-MM-dd HH:mm:ss");
                        log.ElapsedTime = CommonHelper.TimerEnd (stopwatch);
                        WriteSqlLog (log);
                        OnExecutedCommand (cmd);
                    } catch (Exception x) {
                        if (OnException (x))
                            throw;
                        yield break;
                    }

                    var factory = pd.GetFactory (cmd.CommandText, _sharedConnection.ConnectionString, 0, r.FieldCount, r,
                            _defaultMapper) as Func<IDataReader, T>;
                    using (r) {
                        while (true) {
                            T poco;
                            try {
                                if (!r.Read ())
                                    yield break;
                                poco = factory (r);
                            } catch (Exception x) {
                                if (OnException (x))
                                    throw;
                                yield break;
                            }

                            yield return poco;
                        }
                    }
                }
            } finally {
                CloseSharedConnection ();
            }
        }

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
        public IEnumerable<T> Query<T> (Sql sql) {
            return Query<T> (sql.SQL, sql.Arguments);
        }

        #endregion

        #region operation : Exists

        /// <summary>
        ///     Checks for the existence of a row matching the specified condition
        /// </summary>
        /// <typeparam name="T">The Type representing the table being queried</typeparam>
        /// <param name="sqlCondition">The SQL expression to be tested for (ie: the WHERE expression)</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL statement</param>
        /// <returns>True if a record matching the condition is found.</returns>
        public bool Exists<T> (string sqlCondition, params object[] args) {
            var poco = PocoData.ForType (typeof (T), _defaultMapper).TableInfo;

            if (sqlCondition.TrimStart ().StartsWith ("where", StringComparison.OrdinalIgnoreCase))
                sqlCondition = sqlCondition.TrimStart ().Substring (5);

            return ExecuteScalar<int> (string.Format (_provider.GetExistsSql (), Provider.EscapeTableName (poco.TableName), sqlCondition), args) != 0;
        }

        /// <summary>
        ///     Checks for the existence of a row with the specified primary key value.
        /// </summary>
        /// <typeparam name="T">The Type representing the table being queried</typeparam>
        /// <param name="primaryKey">The primary key value to look for</param>
        /// <returns>True if a record with the specified primary key value exists.</returns>
        public bool Exists<T> (object primaryKey) {
            var poco = PocoData.ForType (typeof (T), _defaultMapper);
            return Exists<T> (string.Format ("{0}=@0", _provider.EscapeSqlIdentifier (poco.TableInfo.PrimaryKey)),
                primaryKey is T ? poco.Columns[poco.TableInfo.PrimaryKey].GetValue (primaryKey) : primaryKey);
        }

        #endregion

        #region operation : linq style (Exists, Single, SingleOrDefault etc...)

        /// <summary>
        ///     Returns the record with the specified primary key value
        /// </summary>
        /// <typeparam name="T">The Type representing a row in the result set</typeparam>
        /// <param name="primaryKey">The primary key value of the record to fetch</param>
        /// <returns>The single record matching the specified primary key value</returns>
        /// <remarks>
        ///     Throws an exception if there are zero or more than one record with the specified primary key value.
        /// </remarks>
        public T Single<T> (object primaryKey) {
            return Single<T> (GenerateSingleByKeySql<T> (primaryKey));
        }

        /// <summary>
        ///     Returns the record with the specified primary key value, or the default value if not found
        /// </summary>
        /// <typeparam name="T">The Type representing a row in the result set</typeparam>
        /// <param name="primaryKey">The primary key value of the record to fetch</param>
        /// <returns>The single record matching the specified primary key value</returns>
        /// <remarks>
        ///     If there are no records with the specified primary key value, default(T) (typically null) is returned.
        /// </remarks>
        public T SingleOrDefault<T> (object primaryKey) {
            return SingleOrDefault<T> (GenerateSingleByKeySql<T> (primaryKey));
        }

        private Sql GenerateSingleByKeySql<T> (object primaryKey) {
            string pkName = _provider.EscapeSqlIdentifier (PocoData.ForType (typeof (T), _defaultMapper).TableInfo.PrimaryKey);
            var sql = $"WHERE {pkName} = @0";

            if (!EnableAutoSelect)
                // We're going to be nice and add the SELECT anyway
                sql = AutoSelectHelper.AddSelectClause<T> (_provider, sql, _defaultMapper);

            return new Sql (sql, primaryKey);
        }

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
        public T Single<T> (string sql, params object[] args) {
            return Query<T> (sql, args).Single ();
        }

        /// <summary>
        ///     Runs a query that should always return either a single row, or no rows
        /// </summary>
        /// <typeparam name="T">The Type representing a row in the result set</typeparam>
        /// <param name="sql">The SQL query</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL statement</param>
        /// <returns>The single record matching the specified primary key value, or default(T) if no matching rows</returns>
        public T SingleOrDefault<T> (string sql, params object[] args) {
            return Query<T> (sql, args).SingleOrDefault ();
        }

        /// <summary>
        ///     Runs a query that should always return at least one return
        /// </summary>
        /// <typeparam name="T">The Type representing a row in the result set</typeparam>
        /// <param name="sql">The SQL query</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL statement</param>
        /// <returns>The first record in the result set</returns>
        public T First<T> (string sql, params object[] args) {
            return Query<T> (sql, args).First ();
        }

        /// <summary>
        ///     Runs a query and returns the first record, or the default value if no matching records
        /// </summary>
        /// <typeparam name="T">The Type representing a row in the result set</typeparam>
        /// <param name="sql">The SQL query</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL statement</param>
        /// <returns>The first record in the result set, or default(T) if no matching rows</returns>
        public T FirstOrDefault<T> (string sql, params object[] args) {
            return Query<T> (sql, args).FirstOrDefault ();
        }

        /// <summary>
        ///     Runs a query that should always return a single row.
        /// </summary>
        /// <typeparam name="T">The Type representing a row in the result set</typeparam>
        /// <param name="sql">An SQL builder object representing the query and it's arguments</param>
        /// <returns>The single record matching the specified primary key value</returns>
        /// <remarks>
        ///     Throws an exception if there are zero or more than one matching record
        /// </remarks>
        public T Single<T> (Sql sql) {
            return Query<T> (sql).Single ();
        }

        /// <summary>
        ///     Runs a query that should always return either a single row, or no rows
        /// </summary>
        /// <typeparam name="T">The Type representing a row in the result set</typeparam>
        /// <param name="sql">An SQL builder object representing the query and it's arguments</param>
        /// <returns>The single record matching the specified primary key value, or default(T) if no matching rows</returns>
        public T SingleOrDefault<T> (Sql sql) {
            return Query<T> (sql).SingleOrDefault ();
        }

        /// <summary>
        ///     Runs a query that should always return at least one return
        /// </summary>
        /// <typeparam name="T">The Type representing a row in the result set</typeparam>
        /// <param name="sql">An SQL builder object representing the query and it's arguments</param>
        /// <returns>The first record in the result set</returns>
        public T First<T> (Sql sql) {
            return Query<T> (sql).First ();
        }

        /// <summary>
        ///     Runs a query and returns the first record, or the default value if no matching records
        /// </summary>
        /// <typeparam name="T">The Type representing a row in the result set</typeparam>
        /// <param name="sql">An SQL builder object representing the query and it's arguments</param>
        /// <returns>The first record in the result set, or default(T) if no matching rows</returns>
        public T FirstOrDefault<T> (Sql sql) {
            return Query<T> (sql).FirstOrDefault ();
        }

        #endregion

        #region operation : Insert

        /// <summary>
        ///     Performs an SQL Insert
        /// </summary>
        /// <param name="tableName">The name of the table to insert into</param>
        /// <param name="poco">The POCO object that specifies the column values to be inserted</param>
        /// <returns>The auto allocated primary key of the new record, or null for non-auto-increment tables</returns>
        public object Insert (string tableName, object poco) {
            if (string.IsNullOrEmpty (tableName))
                throw new ArgumentNullException (nameof (tableName));

            if (poco == null)
                throw new ArgumentNullException (nameof (poco));

            var pd = PocoData.ForType (poco.GetType (), _defaultMapper);

            return ExecuteInsert (tableName, pd == null ? null : pd.TableInfo.PrimaryKey, pd != null && pd.TableInfo.AutoIncrement, poco);
        }

        /// <summary>
        ///     Performs an SQL Insert
        /// </summary>
        /// <param name="tableName">The name of the table to insert into</param>
        /// <param name="primaryKeyName">The name of the primary key column of the table</param>
        /// <param name="poco">The POCO object that specifies the column values to be inserted</param>
        /// <returns>The auto allocated primary key of the new record, or null for non-auto-increment tables</returns>
        public object Insert (string tableName, string primaryKeyName, object poco) {
            if (string.IsNullOrEmpty (tableName))
                throw new ArgumentNullException (nameof (tableName));

            if (string.IsNullOrEmpty (primaryKeyName))
                throw new ArgumentNullException (nameof (primaryKeyName));

            if (poco == null)
                throw new ArgumentNullException (nameof (poco));

            var t = poco.GetType ();
            var pd = PocoData.ForType (poco.GetType (), _defaultMapper);
            var autoIncrement = pd == null || pd.TableInfo.AutoIncrement ||
                t.Name.Contains ("AnonymousType") &&
                !t.GetProperties ().Any (p => p.Name.Equals (primaryKeyName, StringComparison.OrdinalIgnoreCase));

            return ExecuteInsert (tableName, primaryKeyName, autoIncrement, poco);
        }

        /// <summary>
        ///     Performs an SQL Insert
        /// </summary>
        /// <param name="tableName">The name of the table to insert into</param>
        /// <param name="primaryKeyName">The name of the primary key column of the table</param>
        /// <param name="autoIncrement">True if the primary key is automatically allocated by the DB</param>
        /// <param name="poco">The POCO object that specifies the column values to be inserted</param>
        /// <returns>The auto allocated primary key of the new record, or null for non-auto-increment tables</returns>
        /// <remarks>
        ///     Inserts a poco into a table.  If the poco has a property with the same name
        ///     as the primary key the id of the new record is assigned to it.  Either way,
        ///     the new id is returned.
        /// </remarks>
        public object Insert (string tableName, string primaryKeyName, bool autoIncrement, object poco) {
            if (string.IsNullOrEmpty (tableName))
                throw new ArgumentNullException (nameof (tableName));

            if (string.IsNullOrEmpty (primaryKeyName))
                throw new ArgumentNullException (nameof (primaryKeyName));

            if (poco == null)
                throw new ArgumentNullException (nameof (poco));

            return ExecuteInsert (tableName, primaryKeyName, autoIncrement, poco);
        }

        /// <summary>
        ///     Performs an SQL Insert
        /// </summary>
        /// <param name="poco">The POCO object that specifies the column values to be inserted</param>
        /// <returns>The auto allocated primary key of the new record, or null for non-auto-increment tables</returns>
        /// <remarks>
        ///     The name of the table, it's primary key and whether it's an auto-allocated primary key are retrieved
        ///     from the POCO's attributes
        /// </remarks>
        public object Insert (object poco) {
            if (poco == null)
                throw new ArgumentNullException (nameof (poco));

            var pd = PocoData.ForType (poco.GetType (), _defaultMapper);
            return ExecuteInsert (pd.TableInfo.TableName, pd.TableInfo.PrimaryKey, pd.TableInfo.AutoIncrement, poco);
        }

        private object ExecuteInsert (string tableName, string primaryKeyName, bool autoIncrement, object poco) {
            SqlLog log=new SqlLog();
            try {
                Stopwatch stopwatch = CommonHelper.TimerStart ();
                OpenSharedConnection ();
                try {
                    using (var cmd = CreateCommand (_sharedConnection, "")) {
                        var pd = PocoData.ForObject (poco, primaryKeyName, _defaultMapper);
                        var names = new List<string> ();
                        var values = new List<string> ();
                        var index = 0;
                        foreach (var i in pd.Columns) {
                            // Don't insert result columns
                            if (i.Value.ResultColumn)
                                continue;

                            // Don't insert the primary key (except under oracle where we need bring in the next sequence value)
                            if (autoIncrement && primaryKeyName != null && string.Compare (i.Key, primaryKeyName, true) == 0) {
                                // Setup auto increment expression
                                string autoIncExpression = _provider.GetAutoIncrementExpression (pd.TableInfo);
                                if (autoIncExpression != null) {
                                    names.Add (i.Key);
                                    values.Add (autoIncExpression);
                                }

                                continue;
                            }

                            names.Add (_provider.EscapeSqlIdentifier (i.Key));
                            values.Add (string.Format (i.Value.InsertTemplate ?? "{0}{1}", _paramPrefix, index++));
                            AddParam (cmd, i.Value.GetValue (poco), i.Value.PropertyInfo);
                        }

                        string outputClause = String.Empty;
                        if (autoIncrement) {
                            outputClause = _provider.GetInsertOutputClause (primaryKeyName);
                        }

                        cmd.CommandText = string.Format ("INSERT INTO {0} ({1}){2} VALUES ({3})",
                            _provider.EscapeTableName (tableName),
                            string.Join (",", names.ToArray ()),
                            outputClause,
                            string.Join (",", values.ToArray ())
                        );
                        log = new SqlLog()
                        {
                            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            OperateSql = cmd.CommandText
                        };
                        if (!autoIncrement) {
                            DoPreExecute (cmd);
                            cmd.ExecuteNonQuery ();
                            OnExecutedCommand (cmd);

                            PocoColumn pkColumn;
                            if (primaryKeyName != null && pd.Columns.TryGetValue (primaryKeyName, out pkColumn))
                                return pkColumn.GetValue (poco);
                            else
                                return "1";
                        }
                       
                        object id = _provider.ExecuteInsert (this, cmd, primaryKeyName);
                        // Assign the ID back to the primary key property
                        if (primaryKeyName != null && !poco.GetType ().Name.Contains ("AnonymousType")) {
                            PocoColumn pc;
                            if (pd.Columns.TryGetValue (primaryKeyName, out pc)) {
                                pc.SetValue (poco, pc.ChangeType (id));
                            }
                        }
                        return id;
                    }
                } finally {
                  
                    log.EndDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    log.ElapsedTime = CommonHelper.TimerEnd(stopwatch);
                    WriteSqlLog(log);
                    CloseSharedConnection();
                }
            } catch (Exception x) {
                if (OnException (x))
                    throw;
                return null;
            }
        }
        /// <summary>
        /// Bulk inserts multiple rows to SQL
        /// </summary>
        /// <param name="tableName">The name of the table to insert into</param>
        /// <param name="primaryKeyName">The name of the primary key column of the table</param>
        /// <param name="autoIncrement">True if the primary key is automatically allocated by the DB</param>
        /// <param name="pocos">The POCO objects that specifies the column values to be inserted</param>
        /// <param name="batchSize">The number of POCOS to be grouped together for each database rounddtrip</param>        
        public void BulkInsert (string tableName, string primaryKeyName, bool autoIncrement, IEnumerable<object> pocos, int batchSize = 25) {
            try {
                Stopwatch stopwatch = CommonHelper.TimerStart ();
                OpenSharedConnection ();
                try {
                    using (var cmd = CreateCommand (_sharedConnection, "")) {
                        var pd = PocoData.ForObject (pocos.First (), primaryKeyName, _defaultMapper);
                        // Create list of columnnames only once
                        var names = new List<string> ();
                        foreach (var i in pd.Columns) {
                            // Don't insert result columns
                            if (i.Value.ResultColumn)
                                continue;

                            // Don't insert the primary key (except under oracle where we need bring in the next sequence value)
                            if (autoIncrement && primaryKeyName != null && string.Compare (i.Key, primaryKeyName, true) == 0) {
                                // Setup auto increment expression
                                string autoIncExpression = _provider.GetAutoIncrementExpression (pd.TableInfo);
                                if (autoIncExpression != null) {
                                    names.Add (i.Key);
                                }
                                continue;
                            }
                            names.Add (_provider.EscapeSqlIdentifier (i.Key));
                        }
                        var namesArray = names.ToArray ();

                        var values = new List<string> ();
                        int count = 0;
                        do {
                            cmd.CommandText = "";
                            cmd.Parameters.Clear ();
                            var index = 0;
                            foreach (var poco in pocos.Skip (count).Take (batchSize)) {
                                values.Clear ();
                                foreach (var i in pd.Columns) {
                                    // Don't insert result columns
                                    if (i.Value.ResultColumn) continue;

                                    // Don't insert the primary key (except under oracle where we need bring in the next sequence value)
                                    if (autoIncrement && primaryKeyName != null && string.Compare (i.Key, primaryKeyName, true) == 0) {
                                        // Setup auto increment expression
                                        string autoIncExpression = _provider.GetAutoIncrementExpression (pd.TableInfo);
                                        if (autoIncExpression != null) {
                                            values.Add (autoIncExpression);
                                        }
                                        continue;
                                    }

                                    values.Add (string.Format ("{0}{1}", _paramPrefix, index++));
                                    AddParam (cmd, i.Value.GetValue (poco), i.Value.PropertyInfo);
                                }

                                string outputClause = String.Empty;
                                if (autoIncrement) {
                                    outputClause = _provider.GetInsertOutputClause (primaryKeyName);
                                }

                                cmd.CommandText += string.Format ("INSERT INTO {0} ({1}){2} VALUES ({3})", _provider.EscapeTableName (tableName),
                                    string.Join (",", namesArray), outputClause, string.Join (",", values.ToArray ()));
                            }
                            // Are we done?
                            if (cmd.CommandText == "") break;
                            count += batchSize;
                            DoPreExecute (cmd);
                            cmd.ExecuteNonQuery ();
                            OnExecutedCommand (cmd);
                        }
                        while (true);

                    }
                    SqlLog log = new SqlLog {
                        CreateTime = DateTime.Now.ToString ("yyyy-MM-dd HH:mm:ss"),
                        OperateSql = "BulkCopy批量插入"
                    };
                    log.EndDateTime = DateTime.Now.ToString ("yyyy-MM-dd HH:mm:ss");
                    log.ElapsedTime = CommonHelper.TimerEnd (stopwatch);
                    WriteSqlLog (log);
                } finally {
                    CloseSharedConnection ();
                }
            } catch (Exception x) {
                if (OnException (x))
                    throw;
            }
        }

        /// <summary>
        /// Performs a SQL Bulk Insert
        /// </summary>
        /// <param name="pocos">The POCO objects that specifies the column values to be inserted</param>        
        /// <param name="batchSize">The number of POCOS to be grouped together for each database rounddtrip</param>        
        public void BulkInsert (IEnumerable<object> pocos, int batchSize = 25) {
            if (!pocos.Any ()) return;
            var pd = PocoData.ForType (pocos.First ().GetType (), _defaultMapper);
            BulkInsert (pd.TableInfo.TableName, pd.TableInfo.PrimaryKey, pd.TableInfo.AutoIncrement, pocos);
        }
        #endregion

        #region operation : Update

        /// <summary>
        ///     Performs an SQL update
        /// </summary>
        /// <param name="tableName">The name of the table to update</param>
        /// <param name="primaryKeyName">The name of the primary key column of the table</param>
        /// <param name="poco">The POCO object that specifies the column values to be updated</param>
        /// <param name="primaryKeyValue">The primary key of the record to be updated</param>
        /// <returns>The number of affected records</returns>
        public int Update (string tableName, string primaryKeyName, object poco, object primaryKeyValue) {
            if (string.IsNullOrEmpty (tableName))
                throw new ArgumentNullException (nameof (tableName));

            if (string.IsNullOrEmpty (primaryKeyName))
                throw new ArgumentNullException (nameof (primaryKeyName));

            if (poco == null)
                throw new ArgumentNullException (nameof (poco));

            return ExecuteUpdate (tableName, primaryKeyName, poco, primaryKeyValue, null);
        }

        /// <summary>
        ///     Performs an SQL update
        /// </summary>
        /// <param name="tableName">The name of the table to update</param>
        /// <param name="primaryKeyName">The name of the primary key column of the table</param>
        /// <param name="poco">The POCO object that specifies the column values to be updated</param>
        /// <param name="primaryKeyValue">The primary key of the record to be updated</param>
        /// <param name="columns">The column names of the columns to be updated, or null for all</param>
        /// <returns>The number of affected rows</returns>
        public int Update (string tableName, string primaryKeyName, object poco, object primaryKeyValue, IEnumerable<string> columns) {
            if (string.IsNullOrEmpty (tableName))
                throw new ArgumentNullException (nameof (tableName));

            if (string.IsNullOrEmpty (primaryKeyName))
                throw new ArgumentNullException (nameof (primaryKeyName));

            if (poco == null)
                throw new ArgumentNullException (nameof (poco));

            return ExecuteUpdate (tableName, primaryKeyName, poco, primaryKeyValue, columns);
        }

        /// <summary>
        ///     Performs an SQL update
        /// </summary>
        /// <param name="tableName">The name of the table to update</param>
        /// <param name="primaryKeyName">The name of the primary key column of the table</param>
        /// <param name="poco">The POCO object that specifies the column values to be updated</param>
        /// <returns>The number of affected rows</returns>
        public int Update (string tableName, string primaryKeyName, object poco) {
            return Update (tableName, primaryKeyName, poco, null);
        }

        /// <summary>
        ///     Performs an SQL update
        /// </summary>
        /// <param name="tableName">The name of the table to update</param>
        /// <param name="primaryKeyName">The name of the primary key column of the table</param>
        /// <param name="poco">The POCO object that specifies the column values to be updated</param>
        /// <param name="columns">The column names of the columns to be updated, or null for all</param>
        /// <returns>The number of affected rows</returns>
        public int Update (string tableName, string primaryKeyName, object poco, IEnumerable<string> columns) {
            if (string.IsNullOrEmpty (tableName))
                throw new ArgumentNullException (nameof (tableName));

            if (string.IsNullOrEmpty (primaryKeyName))
                throw new ArgumentNullException (nameof (primaryKeyName));

            if (poco == null)
                throw new ArgumentNullException (nameof (poco));

            return ExecuteUpdate (tableName, primaryKeyName, poco, null, columns);
        }

        /// <summary>
        ///     Performs an SQL update
        /// </summary>
        /// <param name="poco">The POCO object that specifies the column values to be updated</param>
        /// <param name="columns">The column names of the columns to be updated, or null for all</param>
        /// <returns>The number of affected rows</returns>
        public int Update (object poco, IEnumerable<string> columns) {
            return Update (poco, null, columns);
        }

        /// <summary>
        ///     Performs an SQL update
        /// </summary>
        /// <param name="poco">The POCO object that specifies the column values to be updated</param>
        /// <returns>The number of affected rows</returns>
        public int Update (object poco) {
            return Update (poco, null, null);
        }

        /// <summary>
        ///     Performs an SQL update
        /// </summary>
        /// <param name="poco">The POCO object that specifies the column values to be updated</param>
        /// <param name="primaryKeyValue">The primary key of the record to be updated</param>
        /// <returns>The number of affected rows</returns>
        public int Update (object poco, object primaryKeyValue) {
            return Update (poco, primaryKeyValue, null);
        }

        /// <summary>
        ///     Performs an SQL update
        /// </summary>
        /// <param name="poco">The POCO object that specifies the column values to be updated</param>
        /// <param name="primaryKeyValue">The primary key of the record to be updated</param>
        /// <param name="columns">The column names of the columns to be updated, or null for all</param>
        /// <returns>The number of affected rows</returns>
        public int Update (object poco, object primaryKeyValue, IEnumerable<string> columns) {
            if (poco == null)
                throw new ArgumentNullException (nameof (poco));

            var pd = PocoData.ForType (poco.GetType (), _defaultMapper);
            return ExecuteUpdate (pd.TableInfo.TableName, pd.TableInfo.PrimaryKey, poco, primaryKeyValue, columns);
        }

        /// <summary>
        ///     Performs an SQL update
        /// </summary>
        /// <typeparam name="T">The POCO class who's attributes specify the name of the table to update</typeparam>
        /// <param name="sql">The SQL update and condition clause (ie: everything after "UPDATE tablename"</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL</param>
        /// <returns>The number of affected rows</returns>
        public int Update<T> (string sql, params object[] args) {
            if (string.IsNullOrEmpty (sql))
                throw new ArgumentNullException (nameof (sql));

            var pd = PocoData.ForType (typeof (T), _defaultMapper);
            return Execute (string.Format ("UPDATE {0} {1}", _provider.EscapeTableName (pd.TableInfo.TableName), sql), args);
        }

        /// <summary>
        ///     Performs an SQL update
        /// </summary>
        /// <typeparam name="T">The POCO class who's attributes specify the name of the table to update</typeparam>
        /// <param name="sql">
        ///     An SQL builder object representing the SQL update and condition clause (ie: everything after "UPDATE
        ///     tablename"
        /// </param>
        /// <returns>The number of affected rows</returns>
        public int Update<T> (Sql sql) {
            if (sql == null)
                throw new ArgumentNullException (nameof (sql));

            var pd = PocoData.ForType (typeof (T), _defaultMapper);
            return Execute (new Sql (string.Format ("UPDATE {0}", _provider.EscapeTableName (pd.TableInfo.TableName))).Append (sql));
        }

        private int ExecuteUpdate (string tableName, string primaryKeyName, object poco, object primaryKeyValue, IEnumerable<string> columns) {
            try {
                OpenSharedConnection ();
                Stopwatch stopwatch = CommonHelper.TimerStart ();
                try {
                    using (var cmd = CreateCommand (_sharedConnection, "")) {
                        var sb = new StringBuilder ();
                        var index = 0;
                        var pd = PocoData.ForObject (poco, primaryKeyName, _defaultMapper);
                        if (columns == null) {
                            foreach (var i in pd.Columns) {
                                // Don't update the primary key, but grab the value if we don't have it
                                if (string.Compare (i.Key, primaryKeyName, true) == 0) {
                                    if (primaryKeyValue == null)
                                        primaryKeyValue = i.Value.GetValue (poco);
                                    continue;
                                }

                                // Dont update result only columns
                                if (i.Value.ResultColumn)
                                    continue;

                                // Build the sql
                                if (index > 0)
                                    sb.Append (", ");
                                sb.AppendFormat (i.Value.UpdateTemplate ?? "{0} = {1}{2}", _provider.EscapeSqlIdentifier (i.Key), _paramPrefix, index++);

                                // Store the parameter in the command
                                AddParam (cmd, i.Value.GetValue (poco), i.Value.PropertyInfo);
                            }
                        } else {
                            foreach (var colname in columns) {
                                var pc = pd.Columns[colname];

                                // Build the sql
                                if (index > 0)
                                    sb.Append (", ");
                                sb.AppendFormat (pc.UpdateTemplate ?? "{0} = {1}{2}", _provider.EscapeSqlIdentifier (colname), _paramPrefix, index++);

                                // Store the parameter in the command
                                AddParam (cmd, pc.GetValue (poco), pc.PropertyInfo);
                            }

                            // Grab primary key value
                            if (primaryKeyValue == null) {
                                var pc = pd.Columns[primaryKeyName];
                                primaryKeyValue = pc.GetValue (poco);
                            }
                        }

                        // Find the property info for the primary key
                        PropertyInfo pkpi = null;
                        if (primaryKeyName != null) {
                            PocoColumn col;
                            pkpi = pd.Columns.TryGetValue (primaryKeyName, out col) ?
                                col.PropertyInfo :
                                new { Id = primaryKeyValue }.GetType ().GetProperties () [0];
                        }

                        cmd.CommandText = string.Format ("UPDATE {0} SET {1} WHERE {2} = {3}{4}",
                            _provider.EscapeTableName (tableName), sb.ToString (), _provider.EscapeSqlIdentifier (primaryKeyName), _paramPrefix, index++);
                        AddParam (cmd, primaryKeyValue, pkpi);

                        DoPreExecute (cmd);
                        SqlLog log = new SqlLog {
                            CreateTime = DateTime.Now.ToString ("yyyy-MM-dd HH:mm:ss"),
                            OperateSql = cmd.CommandText,
                            Parameter = cmd.Parameters == null ? "" : cmd.Parameters.ToString ()
                        };
                        // Do it
                        var retv = cmd.ExecuteNonQuery ();
                        OnExecutedCommand (cmd);
                        log.EndDateTime = DateTime.Now.ToString ("yyyy-MM-dd HH:mm:ss");
                        log.ElapsedTime = CommonHelper.TimerEnd (stopwatch);
                        WriteSqlLog (log);
                        return retv;
                    }
                } finally {
                    CloseSharedConnection ();
                }
            } catch (Exception x) {
                if (OnException (x))
                    throw;
                return -1;
            }
        }

        #endregion

        #region operation : Delete

        /// <summary>
        ///     Performs and SQL Delete
        /// </summary>
        /// <param name="tableName">The name of the table to delete from</param>
        /// <param name="primaryKeyName">The name of the primary key column</param>
        /// <param name="poco">The POCO object whose primary key value will be used to delete the row</param>
        /// <returns>The number of rows affected</returns>
        public int Delete (string tableName, string primaryKeyName, object poco) {
            return Delete (tableName, primaryKeyName, poco, null);
        }

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
        public int Delete (string tableName, string primaryKeyName, object poco, object primaryKeyValue) {
            var stopwatch = CommonHelper.TimerStart ();
            // If primary key value not specified, pick it up from the object
            if (primaryKeyValue == null) {
                var pd = PocoData.ForObject (poco, primaryKeyName, _defaultMapper);
                PocoColumn pc;
                if (pd.Columns.TryGetValue (primaryKeyName, out pc)) {
                    primaryKeyValue = pc.GetValue (poco);
                }
            }

            // Do it
            var sql = string.Format ("DELETE FROM {0} WHERE {1}=@0", _provider.EscapeTableName (tableName), _provider.EscapeSqlIdentifier (primaryKeyName));
            SqlLog log = new SqlLog {
                CreateTime = DateTime.Now.ToString ("yyyy-MM-dd HH:mm:ss"),
                OperateSql = sql,
                Parameter = primaryKeyValue.ToString ()
            };
            var result = Execute (sql, primaryKeyValue);
            log.EndDateTime = DateTime.Now.ToString ("yyyy-MM-dd HH:mm:ss");
            log.ElapsedTime = CommonHelper.TimerEnd (stopwatch);
            WriteSqlLog (log);
            return result;
        }

        /// <summary>
        ///     Performs an SQL Delete
        /// </summary>
        /// <param name="poco">The POCO object specifying the table name and primary key value of the row to be deleted</param>
        /// <returns>The number of rows affected</returns>
        public int Delete (object poco) {
            var pd = PocoData.ForType (poco.GetType (), _defaultMapper);
            return Delete (pd.TableInfo.TableName, pd.TableInfo.PrimaryKey, poco);
        }

        /// <summary>
        ///     Performs an SQL Delete
        /// </summary>
        /// <typeparam name="T">The POCO class whose attributes identify the table and primary key to be used in the delete</typeparam>
        /// <param name="pocoOrPrimaryKey">The value of the primary key of the row to delete</param>
        /// <returns></returns>
        public int Delete<T> (object pocoOrPrimaryKey) {
            if (pocoOrPrimaryKey.GetType () == typeof (T))
                return Delete (pocoOrPrimaryKey);

            var pd = PocoData.ForType (typeof (T), _defaultMapper);

            if (pocoOrPrimaryKey.GetType ().Name.Contains ("AnonymousType")) {
                var pi = pocoOrPrimaryKey.GetType ().GetProperty (pd.TableInfo.PrimaryKey);

                if (pi == null)
                    throw new InvalidOperationException (string.Format ("Anonymous type does not contain an id for PK column `{0}`.", pd.TableInfo.PrimaryKey));

                pocoOrPrimaryKey = pi.GetValue (pocoOrPrimaryKey, new object[0]);
            }

            return Delete (pd.TableInfo.TableName, pd.TableInfo.PrimaryKey, null, pocoOrPrimaryKey);
        }

        /// <summary>
        ///     Performs an SQL Delete
        /// </summary>
        /// <typeparam name="T">The POCO class who's attributes specify the name of the table to delete from</typeparam>
        /// <param name="sql">The SQL condition clause identifying the row to delete (ie: everything after "DELETE FROM tablename"</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL</param>
        /// <returns>The number of affected rows</returns>
        public int Delete<T> (string sql, params object[] args) {
            var pd = PocoData.ForType (typeof (T), _defaultMapper);
            return Execute (string.Format ("DELETE FROM {0} {1}", _provider.EscapeTableName (pd.TableInfo.TableName), sql), args);
        }

        /// <summary>
        ///     Performs an SQL Delete
        /// </summary>
        /// <typeparam name="T">The POCO class who's attributes specify the name of the table to delete from</typeparam>
        /// <param name="sql">
        ///     An SQL builder object representing the SQL condition clause identifying the row to delete (ie:
        ///     everything after "UPDATE tablename"
        /// </param>
        /// <returns>The number of affected rows</returns>
        public int Delete<T> (Sql sql) {
            var pd = PocoData.ForType (typeof (T), _defaultMapper);
            return Execute (new Sql (string.Format ("DELETE FROM {0}", _provider.EscapeTableName (pd.TableInfo.TableName))).Append (sql));
        }

        #endregion

        #region operation : IsNew

        /// <summary>
        ///     Check if a poco represents a new row
        /// </summary>
        /// <param name="primaryKeyName">The name of the primary key column</param>
        /// <param name="poco">The object instance whose "newness" is to be tested</param>
        /// <returns>True if the POCO represents a record already in the database</returns>
        /// <remarks>This method simply tests if the POCO's primary key column property has been set to something non-zero.</remarks>
        public bool IsNew (string primaryKeyName, object poco) {
            if (poco == null)
                throw new ArgumentNullException (nameof (poco));

            if (string.IsNullOrEmpty (primaryKeyName))
                throw new ArgumentException ("primaryKeyName");

            return IsNew (primaryKeyName, PocoData.ForObject (poco, primaryKeyName, _defaultMapper), poco);
        }

        protected virtual bool IsNew (string primaryKeyName, PocoData pd, object poco) {
            if (string.IsNullOrEmpty (primaryKeyName) || poco is ExpandoObject)
                throw new InvalidOperationException ("IsNew() and Save() are only supported on tables with identity (inc auto-increment) primary key columns");

            object pk;
            PocoColumn pc;
            PropertyInfo pi;
            if (pd.Columns.TryGetValue (primaryKeyName, out pc)) {
                pk = pc.GetValue (poco);
                pi = pc.PropertyInfo;
            } else {
                pi = poco.GetType ().GetProperty (primaryKeyName);
                if (pi == null)
                    throw new ArgumentException (string.Format ("The object doesn't have a property matching the primary key column name '{0}'", primaryKeyName));
                pk = pi.GetValue (poco, null);
            }

            var type = pk != null ? pk.GetType () : pi.PropertyType;

            if (type.IsGenericType && type.GetGenericTypeDefinition () == typeof (Nullable<>) || !type.IsValueType)
                return pk == null;

            if (type == typeof (string))
                return string.IsNullOrEmpty ((string) pk);
            if (!pi.PropertyType.IsValueType)
                return pk == null;
            if (type == typeof (long))
                return (long) pk == default (long);
            if (type == typeof (int))
                return (int) pk == default (int);
            if (type == typeof (Guid))
                return (Guid) pk == default (Guid);
            if (type == typeof (ulong))
                return (ulong) pk == default (ulong);
            if (type == typeof (uint))
                return (uint) pk == default (uint);
            if (type == typeof (short))
                return (short) pk == default (short);
            if (type == typeof (ushort))
                return (ushort) pk == default (ushort);
            if (type == typeof (decimal))
                return (decimal) pk == default (decimal);

            // Create a default instance and compare
            return pk == Activator.CreateInstance (pk.GetType ());
        }

        /// <summary>
        ///     Check if a poco represents a new row
        /// </summary>
        /// <param name="poco">The object instance whose "newness" is to be tested</param>
        /// <returns>True if the POCO represents a record already in the database</returns>
        /// <remarks>This method simply tests if the POCO's primary key column property has been set to something non-zero.</remarks>
        public bool IsNew (object poco) {
            if (poco == null)
                throw new ArgumentNullException (nameof (poco));

            var pd = PocoData.ForType (poco.GetType (), _defaultMapper);
            return IsNew (pd.TableInfo.PrimaryKey, pd, poco);
        }

        #endregion

        #region operation : Save

        /// <summary>
        ///     Saves a POCO by either performing either an SQL Insert or SQL Update
        /// </summary>
        /// <param name="tableName">The name of the table to be updated</param>
        /// <param name="primaryKeyName">The name of the primary key column</param>
        /// <param name="poco">The POCO object to be saved</param>
        public void Save (string tableName, string primaryKeyName, object poco) {
            if (IsNew (primaryKeyName, poco)) {
                Insert (tableName, primaryKeyName, true, poco);
            } else {
                Update (tableName, primaryKeyName, poco);
            }
        }

        /// <summary>
        ///     Saves a POCO by either performing either an SQL Insert or SQL Update
        /// </summary>
        /// <param name="poco">The POCO object to be saved</param>
        public void Save (object poco) {
            var pd = PocoData.ForType (poco.GetType (), _defaultMapper);
            Save (pd.TableInfo.TableName, pd.TableInfo.PrimaryKey, poco);
        }

        #endregion

        #region operation : Multi-Poco Query/Fetch

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
        public List<TRet> Fetch<T1, T2, TRet> (Func<T1, T2, TRet> cb, string sql, params object[] args) {
            return Query<T1, T2, TRet> (cb, sql, args).ToList ();
        }

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
        public List<TRet> Fetch<T1, T2, T3, TRet> (Func<T1, T2, T3, TRet> cb, string sql, params object[] args) {
            return Query<T1, T2, T3, TRet> (cb, sql, args).ToList ();
        }

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
        public List<TRet> Fetch<T1, T2, T3, T4, TRet> (Func<T1, T2, T3, T4, TRet> cb, string sql, params object[] args) {
            return Query<T1, T2, T3, T4, TRet> (cb, sql, args).ToList ();
        }

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
        public List<TRet> Fetch<T1, T2, T3, T4, T5, TRet> (Func<T1, T2, T3, T4, T5, TRet> cb, string sql, params object[] args) {
            return Query<T1, T2, T3, T4, T5, TRet> (cb, sql, args).ToList ();
        }

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
        public IEnumerable<TRet> Query<T1, T2, TRet> (Func<T1, T2, TRet> cb, string sql, params object[] args) {
            return Query<TRet> (new Type[] { typeof (T1), typeof (T2) }, cb, sql, args);
        }

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
        public IEnumerable<TRet> Query<T1, T2, T3, TRet> (Func<T1, T2, T3, TRet> cb, string sql, params object[] args) {
            return Query<TRet> (new Type[] { typeof (T1), typeof (T2), typeof (T3) }, cb, sql, args);
        }

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
        public IEnumerable<TRet> Query<T1, T2, T3, T4, TRet> (Func<T1, T2, T3, T4, TRet> cb, string sql, params object[] args) {
            return Query<TRet> (new Type[] { typeof (T1), typeof (T2), typeof (T3), typeof (T4) }, cb, sql, args);
        }

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
        public IEnumerable<TRet> Query<T1, T2, T3, T4, T5, TRet> (Func<T1, T2, T3, T4, T5, TRet> cb, string sql, params object[] args) {
            return Query<TRet> (new Type[] { typeof (T1), typeof (T2), typeof (T3), typeof (T4), typeof (T5) }, cb, sql, args);
        }

        /// <summary>
        ///     Perform a multi-poco fetch
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <typeparam name="TRet">The returned list POCO type</typeparam>
        /// <param name="cb">A callback function to connect the POCO instances, or null to automatically guess the relationships</param>
        /// <param name="sql">An SQL builder object representing the query and it's arguments</param>
        /// <returns>A collection of POCO's as a List</returns>
        public List<TRet> Fetch<T1, T2, TRet> (Func<T1, T2, TRet> cb, Sql sql) {
            return Query<T1, T2, TRet> (cb, sql.SQL, sql.Arguments).ToList ();
        }

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
        public List<TRet> Fetch<T1, T2, T3, TRet> (Func<T1, T2, T3, TRet> cb, Sql sql) {
            return Query<T1, T2, T3, TRet> (cb, sql.SQL, sql.Arguments).ToList ();
        }

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
        public List<TRet> Fetch<T1, T2, T3, T4, TRet> (Func<T1, T2, T3, T4, TRet> cb, Sql sql) {
            return Query<T1, T2, T3, T4, TRet> (cb, sql.SQL, sql.Arguments).ToList ();
        }

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
        public List<TRet> Fetch<T1, T2, T3, T4, T5, TRet> (Func<T1, T2, T3, T4, T5, TRet> cb, Sql sql) {
            return Query<T1, T2, T3, T4, T5, TRet> (cb, sql.SQL, sql.Arguments).ToList ();
        }

        /// <summary>
        ///     Perform a multi-poco query
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <typeparam name="TRet">The type of objects in the returned IEnumerable</typeparam>
        /// <param name="cb">A callback function to connect the POCO instances, or null to automatically guess the relationships</param>
        /// <param name="sql">An SQL builder object representing the query and it's arguments</param>
        /// <returns>A collection of POCO's as an IEnumerable</returns>
        public IEnumerable<TRet> Query<T1, T2, TRet> (Func<T1, T2, TRet> cb, Sql sql) {
            return Query<TRet> (new Type[] { typeof (T1), typeof (T2) }, cb, sql.SQL, sql.Arguments);
        }

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
        public IEnumerable<TRet> Query<T1, T2, T3, TRet> (Func<T1, T2, T3, TRet> cb, Sql sql) {
            return Query<TRet> (new Type[] { typeof (T1), typeof (T2), typeof (T3) }, cb, sql.SQL, sql.Arguments);
        }

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
        public IEnumerable<TRet> Query<T1, T2, T3, T4, TRet> (Func<T1, T2, T3, T4, TRet> cb, Sql sql) {
            return Query<TRet> (new Type[] { typeof (T1), typeof (T2), typeof (T3), typeof (T4) }, cb, sql.SQL, sql.Arguments);
        }

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
        public IEnumerable<TRet> Query<T1, T2, T3, T4, T5, TRet> (Func<T1, T2, T3, T4, T5, TRet> cb, Sql sql) {
            return Query<TRet> (new Type[] { typeof (T1), typeof (T2), typeof (T3), typeof (T4), typeof (T5) }, cb, sql.SQL, sql.Arguments);
        }

        /// <summary>
        ///     Perform a multi-poco fetch
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <param name="sql">The SQL query to be executed</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL</param>
        /// <returns>A collection of POCO's as a List</returns>
        public List<T1> Fetch<T1, T2> (string sql, params object[] args) {
            return Query<T1, T2> (sql, args).ToList ();
        }

        /// <summary>
        ///     Perform a multi-poco fetch
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <typeparam name="T3">The third POCO type</typeparam>
        /// <param name="sql">The SQL query to be executed</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL</param>
        /// <returns>A collection of POCO's as a List</returns>
        public List<T1> Fetch<T1, T2, T3> (string sql, params object[] args) {
            return Query<T1, T2, T3> (sql, args).ToList ();
        }

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
        public List<T1> Fetch<T1, T2, T3, T4> (string sql, params object[] args) {
            return Query<T1, T2, T3, T4> (sql, args).ToList ();
        }

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
        public List<T1> Fetch<T1, T2, T3, T4, T5> (string sql, params object[] args) {
            return Query<T1, T2, T3, T4, T5> (sql, args).ToList ();
        }

        /// <summary>
        ///     Perform a multi-poco query
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <param name="sql">The SQL query to be executed</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL</param>
        /// <returns>A collection of POCO's as an IEnumerable</returns>
        public IEnumerable<T1> Query<T1, T2> (string sql, params object[] args) {
            return Query<T1> (new Type[] { typeof (T1), typeof (T2) }, null, sql, args);
        }

        /// <summary>
        ///     Perform a multi-poco query
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <typeparam name="T3">The third POCO type</typeparam>
        /// <param name="sql">The SQL query to be executed</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL</param>
        /// <returns>A collection of POCO's as an IEnumerable</returns>
        public IEnumerable<T1> Query<T1, T2, T3> (string sql, params object[] args) {
            return Query<T1> (new Type[] { typeof (T1), typeof (T2), typeof (T3) }, null, sql, args);
        }

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
        public IEnumerable<T1> Query<T1, T2, T3, T4> (string sql, params object[] args) {
            return Query<T1> (new Type[] { typeof (T1), typeof (T2), typeof (T3), typeof (T4) }, null, sql, args);
        }

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
        public IEnumerable<T1> Query<T1, T2, T3, T4, T5> (string sql, params object[] args) {
            return Query<T1> (new Type[] { typeof (T1), typeof (T2), typeof (T3), typeof (T4), typeof (T5) }, null, sql, args);
        }

        /// <summary>
        ///     Perform a multi-poco fetch
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <param name="sql">An SQL builder object representing the query and it's arguments</param>
        /// <returns>A collection of POCO's as a List</returns>
        public List<T1> Fetch<T1, T2> (Sql sql) {
            return Query<T1, T2> (sql.SQL, sql.Arguments).ToList ();
        }

        /// <summary>
        ///     Perform a multi-poco fetch
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <typeparam name="T3">The third POCO type</typeparam>
        /// <param name="sql">An SQL builder object representing the query and it's arguments</param>
        /// <returns>A collection of POCO's as a List</returns>
        public List<T1> Fetch<T1, T2, T3> (Sql sql) {
            return Query<T1, T2, T3> (sql.SQL, sql.Arguments).ToList ();
        }

        /// <summary>
        ///     Perform a multi-poco fetch
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <typeparam name="T3">The third POCO type</typeparam>
        /// <typeparam name="T4">The fourth POCO type</typeparam>
        /// <param name="sql">An SQL builder object representing the query and it's arguments</param>
        /// <returns>A collection of POCO's as a List</returns>
        public List<T1> Fetch<T1, T2, T3, T4> (Sql sql) {
            return Query<T1, T2, T3, T4> (sql.SQL, sql.Arguments).ToList ();
        }

        /// <summary>
        ///     Perform a multi-poco fetch
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <typeparam name="T3">The third POCO type</typeparam>
        /// <typeparam name="T4">The fourth POCO type</typeparam>
        /// <typeparam name="T5">The fifth POCO type</typeparam>
        /// <param name="sql">An SQL builder object representing the query and it's arguments</param>
        /// <returns>A collection of POCO's as a List</returns>
        public List<T1> Fetch<T1, T2, T3, T4, T5> (Sql sql) {
            return Query<T1, T2, T3, T4, T5> (sql.SQL, sql.Arguments).ToList ();
        }

        /// <summary>
        ///     Perform a multi-poco query
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <param name="sql">An SQL builder object representing the query and it's arguments</param>
        /// <returns>A collection of POCO's as an IEnumerable</returns>
        public IEnumerable<T1> Query<T1, T2> (Sql sql) {
            return Query<T1> (new Type[] { typeof (T1), typeof (T2) }, null, sql.SQL, sql.Arguments);
        }

        /// <summary>
        ///     Perform a multi-poco query
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <typeparam name="T3">The third POCO type</typeparam>
        /// <param name="sql">An SQL builder object representing the query and it's arguments</param>
        /// <returns>A collection of POCO's as an IEnumerable</returns>
        public IEnumerable<T1> Query<T1, T2, T3> (Sql sql) {
            return Query<T1> (new Type[] { typeof (T1), typeof (T2), typeof (T3) }, null, sql.SQL, sql.Arguments);
        }

        /// <summary>
        ///     Perform a multi-poco query
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <typeparam name="T3">The third POCO type</typeparam>
        /// <typeparam name="T4">The fourth POCO type</typeparam>
        /// <param name="sql">An SQL builder object representing the query and it's arguments</param>
        /// <returns>A collection of POCO's as an IEnumerable</returns>
        public IEnumerable<T1> Query<T1, T2, T3, T4> (Sql sql) {
            return Query<T1> (new Type[] { typeof (T1), typeof (T2), typeof (T3), typeof (T4) }, null, sql.SQL, sql.Arguments);
        }

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
        public IEnumerable<T1> Query<T1, T2, T3, T4, T5> (Sql sql) {
            return Query<T1> (new Type[] { typeof (T1), typeof (T2), typeof (T3), typeof (T4), typeof (T5) }, null, sql.SQL, sql.Arguments);
        }

        /// <summary>
        ///     Performs a multi-poco query
        /// </summary>
        /// <typeparam name="TRet">The type of objects in the returned IEnumerable</typeparam>
        /// <param name="types">An array of Types representing the POCO types of the returned result set.</param>
        /// <param name="cb">A callback function to connect the POCO instances, or null to automatically guess the relationships</param>
        /// <param name="sql">The SQL query to be executed</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL</param>
        /// <returns>A collection of POCO's as an IEnumerable</returns>
        public IEnumerable<TRet> Query<TRet> (Type[] types, object cb, string sql, params object[] args) {
            OpenSharedConnection ();
            try {
                using (var cmd = CreateCommand (_sharedConnection, sql, args)) {
                    IDataReader r;
                    try {
                        r = cmd.ExecuteReader ();
                        OnExecutedCommand (cmd);
                    } catch (Exception x) {
                        if (OnException (x))
                            throw;
                        yield break;
                    }

                    var factory = MultiPocoFactory.GetFactory<TRet> (types, _sharedConnection.ConnectionString, sql, r, _defaultMapper);
                    if (cb == null)
                        cb = MultiPocoFactory.GetAutoMapper (types.ToArray ());
                    bool bNeedTerminator = false;
                    using (r) {
                        while (true) {
                            TRet poco;
                            try {
                                if (!r.Read ())
                                    break;
                                poco = factory (r, cb);
                            } catch (Exception x) {
                                if (OnException (x))
                                    throw;
                                yield break;
                            }

                            if (poco != null)
                                yield return poco;
                            else
                                bNeedTerminator = true;
                        }

                        if (bNeedTerminator) {
                            var poco = (TRet) (cb as Delegate).DynamicInvoke (new object[types.Length]);
                            if (poco != null)
                                yield return poco;
                            else
                                yield break;
                        }
                    }
                }
            } finally {
                CloseSharedConnection ();
            }
        }

        #endregion

        #region operation : Multi-Result Set

        /// <summary>
        ///     Perform a multi-results set query
        /// </summary>
        /// <param name="sql">An SQL builder object representing the query and it's arguments</param>
        /// <returns>A GridReader to be queried</returns>
        public IGridReader QueryMultiple (Sql sql) {
            return QueryMultiple (sql.SQL, sql.Arguments);
        }

        /// <summary>
        ///     Perform a multi-results set query
        /// </summary>
        /// <param name="sql">The SQL query to be executed</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL</param>
        /// <returns>A GridReader to be queried</returns>
        public IGridReader QueryMultiple (string sql, params object[] args) {
            OpenSharedConnection ();

            GridReader result = null;
            
           using  var cmd = CreateCommand (_sharedConnection, sql, args);

            try {
                var reader = cmd.ExecuteReader ();
                result = new GridReader (this, cmd, reader, _defaultMapper);
            } catch (Exception x) {
                if (OnException (x))
                    throw;
            }

            return result;
        }

        #endregion

        #region Last Command

        /// <summary>
        ///     Retrieves the SQL of the last executed statement
        /// </summary>
        public string LastSQL => _lastSql;

        /// <summary>
        ///     Retrieves the arguments to the last execute statement
        /// </summary>
        public object[] LastArgs => _lastArgs;

        /// <summary>
        ///     Returns a formatted string describing the last executed SQL statement and it's argument values
        /// </summary>
        public string LastCommand => FormatCommand (_lastSql, _lastArgs);

        #endregion

        #region FormatCommand

        /// <summary>
        ///     Formats the contents of a DB command for display
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public string FormatCommand (IDbCommand cmd) {
            return FormatCommand (cmd.CommandText, (from IDataParameter parameter in cmd.Parameters select parameter.Value).ToArray ());
        }

        /// <summary>
        ///     Formats an SQL query and it's arguments for display
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public string FormatCommand (string sql, object[] args) {
            var sb = new StringBuilder ();
            if (sql == null)
                return "";
            sb.Append (sql);
            if (args != null && args.Length > 0) {
                sb.Append ("\n");
                for (int i = 0; i < args.Length; i++) {
                    sb.AppendFormat ("\t -> {0}{1} [{2}] = \"{3}\"\n", _paramPrefix, i, args[i].GetType ().Name, args[i]);
                }

                sb.Remove (sb.Length - 1, 1);
            }

            return sb.ToString ();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the default mapper.
        /// </summary>
        public IMapper DefaultMapper => _defaultMapper;

        /// <summary>
        ///     When set to true, PetaPoco will automatically create the "SELECT columns" part of any query that looks like it
        ///     needs it
        /// </summary>
        public bool EnableAutoSelect { get; set; }

        /// <summary>
        ///     When set to true, parameters can be named ?myparam and populated from properties of the passed in argument values.
        /// </summary>
        public bool EnableNamedParams { get; set; }

        /// <summary>
        ///     Sets the timeout value for all SQL statements.
        /// </summary>
        public int CommandTimeout { get; set; }

        /// <summary>
        ///     Sets the timeout value for the next (and only next) SQL statement
        /// </summary>
        public int OneTimeCommandTimeout { get; set; }

        /// <summary>
        ///     Gets the loaded database provider. <seealso cref="Provider" />.
        /// </summary>
        /// <returns>
        ///     The loaded database type.
        /// </returns>
        public IProvider Provider => _provider;

        /// <summary>
        ///     Gets the connection string.
        /// </summary>
        /// <returns>
        ///     The connection string.
        /// </returns>
        public string ConnectionString => _connectionString;

        /// <summary>
        ///     Gets or sets the transaction isolation level.
        /// </summary>
        /// <remarks>
        ///     When value is null, the underlying providers default isolation level is used.
        /// </remarks>
        public IsolationLevel? IsolationLevel {
            get => _isolationLevel;
            set {
                if (_transaction != null)
                    throw new InvalidOperationException ("Isolation level can't be changed during a transaction.");

                _isolationLevel = value;
            }
        }

        #endregion

        #region Member Fields

        // Member variables
        private IMapper _defaultMapper;
        private string _connectionString;
        private IProvider _provider;
        private IDbConnection _sharedConnection;
        private IDbTransaction _transaction;
        private int _sharedConnectionDepth;
        private int _transactionDepth;
        private bool _transactionCancelled;
        private string _lastSql;
        private object[] _lastArgs;
        private string _paramPrefix;
        private DbProviderFactory _factory;
        private IsolationLevel? _isolationLevel;

        #endregion

        #region MultiPocoFactory
        internal class MultiPocoFactory {
            // Various cached stuff
            private static readonly Cache<Tuple<Type, ArrayKey<Type>, string, string>, object > MultiPocoFactories = new Cache<Tuple<Type, ArrayKey<Type>, string, string>, object > ();

            private static readonly Cache<ArrayKey<Type>, object> AutoMappers = new Cache<ArrayKey<Type>, object> ();

            // Instance data used by the Multipoco factory delegate - essentially a list of the nested poco factories to call
            private List<Delegate> _delegates;

            public Delegate GetItem (int index) {
                return _delegates[index];
            }

            // Automagically guess the property relationships between various POCOs and create a delegate that will set them up
            public static object GetAutoMapper (Type[] types) {
                // Build a key
                var key = new ArrayKey<Type> (types);

                return AutoMappers.Get (key, () => {
                    // Create a method
                    var m = new DynamicMethod ("petapoco_automapper", types[0], types, true);
                    var il = m.GetILGenerator ();

                    for (var i = 1; i < types.Length; i++) {
                        var handled = false;
                        for (var j = i - 1; j >= 0; j--) {
                            // Find the property
                            var candidates = (from p in types[j].GetProperties () where p.PropertyType == types[i] select p).ToArray ();
                            if (!candidates.Any ())
                                continue;
                            if (candidates.Length > 1)
                                throw new InvalidOperationException (string.Format ("Can't auto join {0} as {1} has more than one property of type {0}", types[i],
                                    types[j]));

                            // Generate code
                            il.Emit (OpCodes.Ldarg_S, j);
                            il.Emit (OpCodes.Ldarg_S, i);
                            il.Emit (OpCodes.Callvirt, candidates.First ().GetSetMethod (true));
                            handled = true;
                        }

                        if (!handled)
                            throw new InvalidOperationException (string.Format ("Can't auto join {0}", types[i]));
                    }

                    il.Emit (OpCodes.Ldarg_0);
                    il.Emit (OpCodes.Ret);

                    // Cache it
                    return m.CreateDelegate (Expression.GetFuncType (types.Concat (types.Take (1)).ToArray ()));
                });
            }

            // Find the split point in a result set for two different pocos and return the poco factory for the first
            private static Delegate FindSplitPoint (Type typeThis, Type typeNext, string connectionString, string sql, IDataReader r, ref int pos, IMapper defaultMapper) {
                // Last?
                if (typeNext == null)
                    return PocoData.ForType (typeThis, defaultMapper).GetFactory (sql, connectionString, pos, r.FieldCount - pos, r, defaultMapper);

                // Get PocoData for the two types
                var pdThis = PocoData.ForType (typeThis, defaultMapper);
                var pdNext = PocoData.ForType (typeNext, defaultMapper);

                // Find split point
                var firstColumn = pos;
                var usedColumns = new Dictionary<string, bool> ();
                for (; pos < r.FieldCount; pos++) {
                    // Split if field name has already been used, or if the field doesn't exist in current poco but does in the next
                    var fieldName = r.GetName (pos);
                    if (usedColumns.ContainsKey (fieldName) || (!pdThis.Columns.ContainsKey (fieldName) && pdNext.Columns.ContainsKey (fieldName))) {
                        return pdThis.GetFactory (sql, connectionString, firstColumn, pos - firstColumn, r, defaultMapper);
                    }
                    usedColumns.Add (fieldName, true);
                }

                throw new InvalidOperationException (string.Format ("Couldn't find split point between {0} and {1}", typeThis, typeNext));
            }

            // Create a multi-poco factory
            private static Func<IDataReader, object, TRet> CreateMultiPocoFactory<TRet> (Type[] types, string connectionString, string sql, IDataReader r, IMapper defaultMapper) {
                var m = new DynamicMethod ("petapoco_multipoco_factory", typeof (TRet), new [] { typeof (MultiPocoFactory), typeof (IDataReader), typeof (object) },
                    typeof (MultiPocoFactory));
                var il = m.GetILGenerator ();

                // Load the callback
                il.Emit (OpCodes.Ldarg_2);

                // Call each delegate
                var dels = new List<Delegate> ();
                var pos = 0;
                for (var i = 0; i < types.Length; i++) {
                    // Add to list of delegates to call
                    var del = FindSplitPoint (types[i], i + 1 < types.Length ? types[i + 1] : null, connectionString, sql, r, ref pos, defaultMapper);
                    dels.Add (del);

                    // Get the delegate
                    il.Emit (OpCodes.Ldarg_0); // callback,this
                    il.Emit (OpCodes.Ldc_I4, i); // callback,this,Index
                    il.Emit (OpCodes.Callvirt, typeof (MultiPocoFactory).GetMethod ("GetItem")); // callback,Delegate
                    il.Emit (OpCodes.Ldarg_1); // callback,delegate, datareader

                    // Call Invoke
                    var tDelInvoke = del.GetType ().GetMethod ("Invoke");
                    il.Emit (OpCodes.Callvirt, tDelInvoke); // Poco left on stack
                }

                // By now we should have the callback and the N pocos all on the stack.  Call the callback and we're done
                il.Emit (OpCodes.Callvirt, Expression.GetFuncType (types.Concat (new [] { typeof (TRet) }).ToArray ()).GetMethod ("Invoke"));
                il.Emit (OpCodes.Ret);

                // Finish up
                return (Func<IDataReader, object, TRet>) m.CreateDelegate (typeof (Func<IDataReader, object, TRet>), new MultiPocoFactory () { _delegates = dels });
            }

            internal static void FlushCaches () {
                MultiPocoFactories.Flush ();
                AutoMappers.Flush ();
            }

            // Get (or create) the multi-poco factory for a query
            public static Func<IDataReader, object, TRet> GetFactory<TRet> (Type[] types, string connectionString, string sql, IDataReader r, IMapper defaultMapper) {
                var key = Tuple.Create (typeof (TRet), new ArrayKey<Type> (types), connectionString, sql);

                return (Func<IDataReader, object, TRet>) MultiPocoFactories.Get (key, () => CreateMultiPocoFactory<TRet> (types, connectionString, sql, r, defaultMapper));
            }
        }
        #endregion

        #region AutoSelectHelper
        internal static class AutoSelectHelper {
            private static Regex rxSelect = new Regex (@"\A\s*(SELECT|EXECUTE|CALL|WITH|SET|DECLARE)\s",
                RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Multiline);

            private static Regex rxFrom = new Regex (@"\A\s*FROM\s",
                RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Multiline);

            public static string AddSelectClause<T> (IProvider provider, string sql, IMapper defaultMapper) {
                try {
                    if (sql.StartsWith (";"))
                        return sql.Substring (1);

                    if (!rxSelect.IsMatch (sql)) {
                        var pd = PocoData.ForType (typeof (T), defaultMapper);
                        var tableName = provider.EscapeTableName (pd.TableInfo.TableName);
                        string cols = pd.Columns.Count != 0 ?
                            string.Join (", ", (from c in pd.QueryColumns select tableName + "." + provider.EscapeSqlIdentifier (c)).ToArray ()) :
                            "NULL";
                        if (!rxFrom.IsMatch (sql))
                            sql = string.Format ("SELECT {0} FROM {1} {2}", cols, tableName, sql);
                        else
                            sql = string.Format ("SELECT {0} {1}", cols, sql);
                    }
                } catch (Exception) {
                    var pd = PocoData.ForType (typeof (T), defaultMapper);
                    var tableName = provider.EscapeTableName (pd.TableInfo.TableName);
                    string cols = pd.Columns.Count != 0 ?
                        string.Join (", ", (from c in pd.QueryColumns select tableName + "." + provider.EscapeSqlIdentifier (c)).ToArray ()) :
                        "NULL";

                    sql = string.Format ("SELECT {0} FROM {1} {2}", cols, tableName, sql);

                }

                return sql;
            }
        }
        #endregion

        #region AnsiString
        /// <summary>
        ///     Wrap strings in an instance of this class to force use of DBType.AnsiString
        /// </summary>
        public class AnsiString {
            /// <summary>
            ///     The string value
            /// </summary>
            public string Value { get; }

            /// <summary>
            ///     Constructs an AnsiString
            /// </summary>
            /// <param name="str">The C# string to be converted to ANSI before being passed to the DB</param>
            public AnsiString (string str) => Value = str;
        }
        #endregion

        #region ConventionMapper
        /// <summary>
        ///     Represents a configurable convention mapper.
        /// </summary>
        /// <remarks>
        ///     By default this mapper replaces <see cref="StandardMapper" /> without change, which means backwards compatibility
        ///     is kept.
        /// </remarks>
        public class ConventionMapper : IMapper {
            /// <summary>
            ///     Gets or sets the get sequence name logic.
            /// </summary>
            public Func<Type, PropertyInfo, string> GetSequenceName { get; set; }

            /// <summary>
            ///     Gets or sets the inflect column name logic.
            /// </summary>
            public Func<IInflector, string, string> InflectColumnName { get; set; }

            /// <summary>
            ///     Gets or sets the inflect table name logic.
            /// </summary>
            public Func<IInflector, string, string> InflectTableName { get; set; }

            /// <summary>
            ///     Gets or sets the is primary key auto increment logic.
            /// </summary>
            public Func<Type, bool> IsPrimaryKeyAutoIncrement { get; set; }

            /// <summary>
            ///     Gets or sets the map column logic.
            /// </summary>
            public Func<ColumnInfo, Type, PropertyInfo, bool> MapColumn { get; set; }

            /// <summary>
            ///     Gets or set the map primary key logic.
            /// </summary>
            public Func<TableInfo, Type, bool> MapPrimaryKey { get; set; }

            /// <summary>
            ///     Gets or sets the map table logic.
            /// </summary>
            public Func<TableInfo, Type, bool> MapTable { get; set; }

            /// <summary>
            ///     Gets or sets the from db convert logic.
            /// </summary>
            public Func<PropertyInfo, Type, Func<object, object>> FromDbConverter { get; set; }

            /// <summary>
            ///     Gets or sets the to db converter logic.
            /// </summary>
            public Func<PropertyInfo, Func<object, object>> ToDbConverter { get; set; }

            /// <summary>
            ///     Constructs a new instance of convention mapper.
            /// </summary>
            public ConventionMapper () {
                GetSequenceName = (t, pi) => null;
                InflectColumnName = (inflect, cn) => cn;
                InflectTableName = (inflect, tn) => tn;
                MapPrimaryKey = (ti, t) => {
                    var primaryKey = t.GetCustomAttributes (typeof (PrimaryKeyAttribute), true).FirstOrDefault () as PrimaryKeyAttribute;

                    if (primaryKey != null) {
                        ti.PrimaryKey = primaryKey.Value;
                        ti.SequenceName = primaryKey.SequenceName;
                        ti.AutoIncrement = primaryKey.AutoIncrement;
                        return true;
                    }

                    var prop = t.GetProperties ().FirstOrDefault (p => {
                        if (p.Name.Equals ("Id", StringComparison.OrdinalIgnoreCase))
                            return true;
                        if (p.Name.Equals (t.Name + "Id", StringComparison.OrdinalIgnoreCase))
                            return true;
                        if (p.Name.Equals (t.Name + "_Id", StringComparison.OrdinalIgnoreCase))
                            return true;
                        return false;
                    });

                    if (prop == null)
                        return false;

                    ti.PrimaryKey = InflectColumnName (Inflector.Instance, prop.Name);
                    ti.AutoIncrement = IsPrimaryKeyAutoIncrement (prop.PropertyType);
                    ti.SequenceName = GetSequenceName (t, prop);
                    return true;
                };
                MapTable = (ti, t) => {
                    var tableName = t.GetCustomAttributes (typeof (TableNameAttribute), true).FirstOrDefault () as TableNameAttribute;
                    ti.TableName = tableName != null ? tableName.Value : InflectTableName (Inflector.Instance, t.Name);
                    MapPrimaryKey (ti, t);
                    return true;
                };
                IsPrimaryKeyAutoIncrement = t => {
                    if (t.IsGenericType && t.GetGenericTypeDefinition () == typeof (Nullable<>))
                        t = t.GetGenericArguments () [0];

                    if (t == typeof (long) || t == typeof (ulong))
                        return true;
                    if (t == typeof (int) || t == typeof (uint))
                        return true;
                    if (t == typeof (short) || t == typeof (ushort))
                        return true;

                    return false;
                };
                MapColumn = (ci, t, pi) => {
                    // Check if declaring poco has [Explicit] attribute
                    var isExplicit = t.GetCustomAttributes (typeof (ExplicitColumnsAttribute), true).Any ();

                    // Check for [Column]/[Ignore] Attributes
                    var column = pi.GetCustomAttributes (typeof (ColumnAttribute), true).FirstOrDefault () as ColumnAttribute;

                    if (isExplicit && column == null)
                        return false;

                    if (pi.GetCustomAttributes (typeof (IgnoreAttribute), true).Any ())
                        return false;

                    // Read attribute
                    if (column != null) {
                        ci.ColumnName = column.Name ?? InflectColumnName (Inflector.Instance, pi.Name);
                        ci.ForceToUtc = column.ForceToUtc;
                        ci.ResultColumn = (column as ResultColumnAttribute) != null;
                        ci.AutoSelectedResultColumn = (column as ResultColumnAttribute)?.IncludeInAutoSelect == IncludeInAutoSelect.Yes;
                        ci.InsertTemplate = column.InsertTemplate;
                        ci.UpdateTemplate = column.UpdateTemplate;
                    } else {
                        ci.ColumnName = InflectColumnName (Inflector.Instance, pi.Name);
                    }

                    return true;
                };
                FromDbConverter = (pi, t) => {
                    if (pi != null) {
                        var valueConverter = pi.GetCustomAttributes (typeof (ValueConverterAttribute), true).FirstOrDefault () as ValueConverterAttribute;
                        if (valueConverter != null)
                            return valueConverter.ConvertFromDb;
                    }

                    return null;
                };
                ToDbConverter = (pi) => {
                    if (pi != null) {
                        var valueConverter = pi.GetCustomAttributes (typeof (ValueConverterAttribute), true).FirstOrDefault () as ValueConverterAttribute;
                        if (valueConverter != null)
                            return valueConverter.ConvertToDb;
                    }

                    return null;
                };
            }

            /// <summary>
            ///     Get information about the table associated with a POCO class
            /// </summary>
            /// <param name="pocoType">The poco type.</param>
            /// <returns>A TableInfo instance</returns>
            /// <remarks>
            ///     This method must return a valid TableInfo.
            ///     To create a TableInfo from a POCO's attributes, use TableInfo.FromPoco
            /// </remarks>
            public TableInfo GetTableInfo (Type pocoType) {
                var ti = new TableInfo ();
                return MapTable (ti, pocoType) ? ti : null;
            }

            /// <summary>
            ///     Get information about the column associated with a property of a POCO
            /// </summary>
            /// <param name="pocoProperty">The PropertyInfo of the property being queried</param>
            /// <returns>A reference to a ColumnInfo instance, or null to ignore this property</returns>
            /// <remarks>
            ///     To create a ColumnInfo from a property's attributes, use PropertyInfo.FromProperty
            /// </remarks>
            public ColumnInfo GetColumnInfo (PropertyInfo pocoProperty) {
                var ci = new ColumnInfo ();
                return MapColumn (ci, pocoProperty.DeclaringType, pocoProperty) ? ci : null;
            }

            /// <summary>
            ///     Supply a function to convert a database value to the correct property value
            /// </summary>
            /// <param name="targetProperty">The target property</param>
            /// <param name="sourceType">The type of data returned by the DB</param>
            /// <returns>A Func that can do the conversion, or null for no conversion</returns>
            public Func<object, object> GetFromDbConverter (PropertyInfo targetProperty, Type sourceType) {
                return FromDbConverter?.Invoke (targetProperty, sourceType);
            }

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
            public Func<object, object> GetToDbConverter (PropertyInfo sourceProperty) {
                return ToDbConverter?.Invoke (sourceProperty);
            }
        }
        #endregion
    }
}