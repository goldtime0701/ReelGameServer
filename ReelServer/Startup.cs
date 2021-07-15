using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.IO;

namespace ReelServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            new Thread(CDataBase.ThreadExcuteQueryUser).Start();
            new Thread(CDataBase.ThreadExcuteQueryGear).Start();
            new Thread(CDataBase.ThreadExcuteQueryNational).Start();
            new Thread(CDataBase.ThreadExcuteQueryOther).Start();

            CGlobal.OnStartGameServer();
            CAdminServer.Start();
            CUserServer.Start();
            CGlobal.OnStartManagerThread();

            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
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
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
