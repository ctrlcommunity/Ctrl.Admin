#if NETCOREAPP
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.IO;


namespace Ctrl.Core.Core.Config
{
    public class AppSetting
    {
        public List<ConnectionStringSetting> ConnectionStrings { get; set; } = new List<ConnectionStringSetting>();

        public List<BaiDuStringSetting> BaiDuStrings { get; set; } = new List<BaiDuStringSetting>();
        public static AppSetting Load()
        {
            var builder = new ConfigurationBuilder();
                //.SetBasePath(Directory.GetCurrentDirectory())
                //.AddJsonFile(Path.Combine("Configs", "appsettings.json"))
                //;
            var config = builder.Build();

            var app = new AppSetting();
          //  config.GetSection("App").Bind(app);
            return app;
        }

        public class ConnectionStringSetting
        {
            public string Name { get; set; }
            public string ConnectionString { get; set; }
            public string ProviderName { get; set; }
        }
        public class BaiDuStringSetting {
            public string APPID { get; set; }
            public string APIKEY { get; set; }
            public string SECRETKEY { get; set; }

        }
    }
}
#endif