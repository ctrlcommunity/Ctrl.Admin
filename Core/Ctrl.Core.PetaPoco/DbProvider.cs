#if NETCOREAPP
using Ctrl.Core.Core.Config;

using System;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;


namespace Ctrl.Core.PetaPoco
{
    public abstract class DbProvider: IDisposable
    {
        protected abstract IDatabase Database { get; }

        protected abstract string ScriptResourceName { get; }

        public virtual void Dispose()
        {
        }

        public virtual Database Databases() {
            SqlConnection connection = new SqlConnection("null");
            return new Database(connection);
        }
        public virtual IDatabase Execute()
        {
            var db = Database;
            using (var s = GetType().Assembly.GetManifestResourceStream(ScriptResourceName))
            {
                using (var r = new StreamReader(s, Encoding.UTF8))
                {
                    ExecuteBuildScript(db, r.ReadToEnd());
                }
            }
            return db;
        }

        public virtual void ExecuteBuildScript(IDatabase database, string script)
        {
            database.Execute(script);
        }

        protected virtual IDatabaseBuildConfiguration BuildFromConnectionName(string name)
        {
#if NETCOREAPP
            var appSettings = AppSetting.Load();
            return DatabaseConfiguration.Build()
                .UsingConnectionString(appSettings.ConnectionStrings.First(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase)).ConnectionString)
                .UsingProviderName(appSettings.ConnectionStrings.First(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase)).ProviderName);
#elif !NETSTANDARD&&!NETCOREAPP
           return DatabaseConfiguration.Build().UsingConnectionStringName(name);
#endif
        }

        protected virtual IDatabase LoadFromConnectionName(string name)
        {
            return BuildFromConnectionName(name).Create();
        }
    }
}
#endif