using System;

namespace Ctrl.Core.PetaPoco
{
    public  class DbBase:IDisposable
    {
        private readonly string _providerName = "System.Data.SqlClient";
        public Database DB { get; set; }


        public Database Db(string connectionStringName, string ProviderName)
        { 
            DB = new Database(connectionStringName,ProviderName);
            return DB;
        }



        public void Dispose()
        {
            if (DB != null)
            {
                try
                {
                    DB?.Dispose();
                    DB = null;
                }
                catch 
                {
                }
            }
        }
    }
}
