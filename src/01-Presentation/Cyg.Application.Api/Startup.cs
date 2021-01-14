using AspectCore.Extensions.DependencyInjection;
using AutoMapper;
using Coldairarrow.DotNettySocket;
using Cyg.Applicatio;
using Cyg.Applicatio.Repository;
using Cyg.Applicatio.Service;
using Cyg.Applicatio.Service.Impl.Internal;
using Cyg.Extensions.WebApi;
using Cyg.FreeSql;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebApiClient;


namespace Cyg.Application.Api
{
    public class Startup
    {
        private IHostingEnvironment _env;
        private IConfiguration _configuration;
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            var environmentValue = "Ys";// env.IsDevelopment() ? "Ys" : Environment.GetEnvironmentVariable("EnvironmentValue", EnvironmentVariableTarget.Machine);
            if (!environmentValue.HasVal())
            {
                new BusinessException("未配置系统环境变量:EnvironmentValue");
            }
            _env = env;
            _configuration = new ConfigurationBuilder()
                  .SetBasePath(env.ContentRootPath)
                  .AddJsonFile("appsettings.json", true, true)
                  .AddJsonFile($"appsettings.{environmentValue}.json", true, true)
                  .AddEnvironmentVariables()
                  .Build();
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            var setting = new Appsettings();
            ConfigurationBinder.Bind(_configuration.GetSection("Config"), setting);
            services.AddWebApi(_configuration, CygApplication.勘测端Api, "pdd_application", (unitOfWork, sqlQuery) =>
            {

            });
            services.AddSingleton(_configuration);
            services.AddSingleton<IHttpApiFactory<IResourceApiService>, HttpApiFactory<IResourceApiService>>(p =>
            {
                return new HttpApiFactory<IResourceApiService>().ConfigureHttpApiConfig(c =>
                {
                    c.HttpHost = new Uri(setting.ResourceApi);
                });
            });
            services.AddFreeSql(_configuration.GetSection("FreeSql").Get<FreeSqlOptions>());

            services.AddTransient(p =>
            {
                var factory = p.GetRequiredService<IHttpApiFactory<IResourceApiService>>();
                return factory.CreateHttpApi();
            });
            services.ConfigureServices();
            services.AddSingleton<CapHandler>();
            return services.BuildDynamicProxyServiceProvider();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider,IApplicationLifetime applicationLifetime)
        {
            SocketHandler.Map(app);
            serviceProvider.GetService<CapHandler>().MessageHandler();
            app.UseWebApi(_env);
            app.UseStateAutoMapper();
            IWebSocketServer socket = null;
            applicationLifetime.ApplicationStarted.Register(() =>
            {
                socket = app.ApplicationServices.GetRequiredService<ISocketService>()
                .StartAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            });
            applicationLifetime.ApplicationStopping.Register(() =>
            {
                socket?.Close();
            });
        }
    }
}
