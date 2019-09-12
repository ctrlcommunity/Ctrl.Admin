using System;
using System.Data.Common;
using System.Data.SqlClient;

namespace Ctrl.Pay.Service
{
    public class DbBase : IDisposable
    {
        public DbConnection dbConnection;
        private readonly DbProviderFactory _dbFactory;
        public DbBase(string connectionStringName)
        {
            _dbFactory= SqlClientFactory.Instance;
            dbConnection = _dbFactory.CreateConnection();
            dbConnection.ConnectionString = connectionStringName;
            dbConnection.Open();
        }
        public void Dispose()
        {
            if (dbConnection != null)
            {
                try
                {
                    dbConnection?.Dispose();
                }
                catch { }
            }
        }
    }
}
