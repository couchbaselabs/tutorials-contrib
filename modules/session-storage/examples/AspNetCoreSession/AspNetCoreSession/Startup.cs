using System;
using System.Collections.Generic;
using Couchbase.Extensions.Caching;
using Couchbase.Extensions.DependencyInjection;
using Couchbase.Extensions.Session;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCoreSession
{
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
            services.AddMvc();

            // sets up Couchbase to be used by other services
            services.AddCouchbase(opt =>
            {
                opt.Servers = new List<Uri> { new Uri("http://localhost:8091") };
                opt.Username = "Administrator";
                opt.Password = "password";
            });

            // adds Couchbase as a distributed cache (which the session storage will use)
            services.AddDistributedCouchbaseCache("sessionstore", opt => { });

            // add couchbase as the session state provider
            services.AddCouchbaseSession(opt =>
            {
                opt.IdleTimeout = new TimeSpan(0, 0, 20, 0);
                opt.Cookie = new CookieBuilder {Name = ".MyApp.Cookie"};
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime appLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseCookiePolicy();

            // tell ASP.NET that we want to use session
            app.UseSession();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            // close/cleanup any Couchbase cluster connections if the application is being stopped
            appLifetime.ApplicationStopped.Register(() =>
            {
                app.ApplicationServices.GetRequiredService<ICouchbaseLifetimeService>().Close();
            });
        }
    }
}
