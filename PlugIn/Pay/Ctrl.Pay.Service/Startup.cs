using Ctrl.Plugin.Ali;
using Ctrl.Plugin.PayCore;
using Ctrl.Plugin.PayCore.Utils;
using Ctrl.Plugin.Unionpay;
using Ctrl.Plugin.Wx;
using Ctrl.System.Models.Entities;
using Dapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Swagger;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Merchant = Ctrl.Plugin.Unionpay.Merchant;

namespace Ctrl.Pay.Service
{
    public class Startup
    {
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
        public IConfiguration Configuration { get; }
        
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
           
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddWebEncoders(opt =>
            {
                opt.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All);
            });
            services.AddPaySharp(a =>
            {
                var sss = Configuration;
                using (var db = new DbBase(Configuration["ConnectionString"]))
                {
                    var ylstr = db.dbConnection.QueryFirstOrDefault<string>(@"select top 1 pay.ConfigInfo from Sys_Pays pay
                        left join Sys_Dictionary dic on pay.PayType=dic.DictionaryId
                        where dic.Code=@Code ", new { Code = StatusPays.银联支付 });
                    var unionpayMerchant = JsonConvert.DeserializeObject<Merchant>(ylstr);
                    if (!string.IsNullOrWhiteSpace(ylstr)) {
                        a.Add(new UnionpayGateway(unionpayMerchant)
                        {
                            GatewayUrl = "https://gateway.test.95516.com"
                        });
                    }
                 
                }
                using (var db = new DbBase(Configuration["ConnectionString"]))
                {
                    var alistr = db.dbConnection.QueryFirstOrDefault<string>(@"select top 1 pay.ConfigInfo from Sys_Pays pay
                        left join Sys_Dictionary dic on pay.PayType=dic.DictionaryId
                        where dic.Code=@Code ", new { Code = StatusPays.支付宝支付 });
                    if (!string.IsNullOrWhiteSpace(alistr))
                    {
                        var alipayMerchant = JsonConvert.DeserializeObject<Plugin.Ali.Merchant>(alistr);
                        a.Add(new AlipayGateway(alipayMerchant)
                        {
                            GatewayUrl = "https://gateway.test.95516.com"
                        });
                    }
                 
                }
                using (var db = new DbBase(Configuration["ConnectionString"]))
                {
                    var wxstr = db.dbConnection.QueryFirstOrDefault<string>(@"select top 1 pay.ConfigInfo from Sys_Pays pay
                        left join Sys_Dictionary dic on pay.PayType=dic.DictionaryId
                        where dic.Code=@Code ", new { Code = StatusPays.微信支付 });
                    if (!string.IsNullOrWhiteSpace(wxstr))
                    {
                        var wechatpayMerchant = JsonConvert.DeserializeObject<Plugin.Wx.Merchant>(wxstr);
                        a.Add(new WechatpayGateway(wechatpayMerchant));
                    }
                }

            });
            //注册Swagger生成器，定义一个和多个Swagger 文档
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "Ctrl Pays API",
                });
                // 为 Swagger JSON and UI设置xml文档注释路径
                var basePath = Path.GetDirectoryName(typeof(Program).Assembly.Location);//获取应用程序所在目录（绝对，不受工作目录影响，建议采用此方法获取路径）
                var xmlPath = Path.Combine(basePath, "Ctrl.Pay.Service.xml");
                c.IncludeXmlComments(xmlPath);
            });
            #region 允许跨域
            services.AddCors();
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            #region 跨域
            app.UseCors(builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
            #endregion
            
            app.UseHttpsRedirection();
            app.UseMvc();
            app.UsePaySharp();
            //启用中间件服务生成Swagger作为JSON终结点
            app.UseSwagger();
            //启用中间件服务对swagger-ui，指定Swagger JSON终结点
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Pays V1");
                c.RoutePrefix = string.Empty;
            });
        }
    }
}
