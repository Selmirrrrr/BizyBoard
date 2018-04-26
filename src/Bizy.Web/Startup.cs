namespace Bizy.Web
{
    using System;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.SpaServices.AngularCli;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Models;
    using Services;
    using static System.Environment;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration => configuration.RootPath = "ClientApp/dist");

            Configuration.GetValue("WinBizApi:EncryptionKey", "");
            Configuration.GetValue("WinBizApi:Url", "");
            GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            services.AddSingleton(new SettingsService(new WinBizApiSettings(GetEnvironmentVariable("WINBIZ_API_KEY"),
                                                                            GetEnvironmentVariable("WINBIZ_API_COMPANY"),
                                                                            GetEnvironmentVariable("WINBIZ_API_USERNAME"),
                                                                            GetEnvironmentVariable("WINBIZ_API_PASSWORD"),
                                                                            Configuration.GetValue("WinBizApi:EncryptionKey", ""),
                                                                            Configuration.GetValue("WinBizApi:Url", ""))));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    "default",
                    "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment()) spa.UseAngularCliServer("start");
            });
        }
    }
}