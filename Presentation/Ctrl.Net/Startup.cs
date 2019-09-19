using Autofac;
using Autofac.Extensions.DependencyInjection;
using Ctrl.Core.AutoMapper;
using Ctrl.Core.Core.Auth;
using Ctrl.Core.Core.Config;
using Ctrl.Core.Core.Http;
using Ctrl.Core.Core.Reflection;
using Ctrl.Core.Tag.Controls.Button;
using Ctrl.Core.Tag.Controls.Buttons;
using Ctrl.Core.Web.Attributes;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using NLog.Web;
using System;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Unicode;

namespace Ctrl.Net
{

    public class Startup
    {
        public ContainerBuilder Builder { get; set; }
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                         .SetBasePath(env.ContentRootPath)
                         .AddJsonFile(Path.Combine("Configs", "appsettings.json"), optional: true,
                                      reloadOnChange: true) //Settings for the application
                         .AddJsonFile(Path.Combine("Configs", $"appsettings.{env.EnvironmentName}.json"),
                                      optional: true, reloadOnChange: true);
            Configuration = builder.Build();
        }
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessors();
 
            //缓存
            services.AddMemoryCache();
            //services.AddMvc();
            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(OperationLogAttribute));
                options.Filters.Add(typeof(ExceptionFilterAttribute));
                options.Filters.Add(typeof(WebPermissionFilter));
                
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2).AddJsonOptions(json =>
            {
                //统一设置JsonResult中的日期格式
                json.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            });
            services.AddWebEncoders(opt =>
            {
                opt.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All);
            });
            //注册Cookie认证服务
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme,o=> {
                o.LoginPath = new PathString("/sysManage/Account/Login");          
                o.AccessDeniedPath = new PathString("/sysManage/Home/NotFounds");     
                o.SlidingExpiration = true;
            });
            //services.AddMvc().AddJsonOptions(options => { options.SerializerSettings.ContractResolver = new LowerCasePropertyNamesContractResolver(); });
            #region IOC注册区域
            Builder = new ContainerBuilder();//实例化autofac
            Builder.Populate(services);
            var assemblys = AssemblyHelper.LoadCompileAssemblies();
            Builder.RegisterAssemblyTypes(assemblys.ToArray()).Where(t => t.Name.EndsWith("Logic")).AsImplementedInterfaces().InstancePerLifetimeScope();
            Builder.RegisterAssemblyTypes(assemblys.ToArray()).Where(t => t.Name.EndsWith("Repository")).AsImplementedInterfaces().InstancePerLifetimeScope();
            var ApplicationContainer = Builder.Build();
            //services.AddMvc();
            #endregion

          //  services.AddScoped<IButtonTagHelperBase, CtrlButtonTagHelper>();
            return new AutofacServiceProvider(ApplicationContainer);//第三方IOC接管 core内置DI容器 
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
             app.UseStaticHttpContext();
             new AuthenticationExtension(env);
     
            app.UseAuthentication();

            #region Nlog
            //添加Nlog组件到.net core中
            loggerFactory.AddNLog();
            //添加nlog的中间件
#pragma warning disable CS0618 // 类型或成员已过时
            app.AddNLogWeb();
#pragma warning restore CS0618 // 类型或成员已过时

            //指定NLog的配置文件
            env.ConfigureNLog(Path.Combine("Configs", "nlog.config"));
            LogManager.Configuration.Variables["connectionString"] = AppSetting.Load().ConnectionStrings.FirstOrDefault().ConnectionString;
            #endregion

            var Provider = new FileExtensionContentTypeProvider();
            Provider.Mappings[".less"] = "text/css";
            app.UseStaticFiles(new StaticFileOptions()
            {
                ContentTypeProvider = Provider
            });

           AutoMapperUtil.InitializeMap();//初始化automapper

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                name: "areaname",
                template: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                routes.MapRoute(
                  name: "default",
                  template: "{controller=Home}/{action=Index}/{id?}");
                routes.MapAreaRoute(
                    name: "sysManage",
                    areaName: "sysManage",
                    template: "sysManage/{controller=Home}/{action=Index}"
                    );

            });
  
        }
    }
}
