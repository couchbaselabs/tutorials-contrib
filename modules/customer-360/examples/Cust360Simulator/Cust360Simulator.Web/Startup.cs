using System;
using System.Collections.Generic;
using Couchbase.Configuration.Client;
using Couchbase.Extensions.DependencyInjection;
using Cust360Simulator.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MySql.Data.MySqlClient;
using Npgsql;

namespace Cust360Simulator.Web
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
            services.AddMvc()
                .AddNewtonsoftJson();

            services.AddSingleton<MySqlConnection>(x =>
            {
                var mysqlConnectionString = "server=localhost;port=3306;database=inventory;user=root;password=debezium";
                return new MySqlConnection(mysqlConnectionString);
            });
            services.AddSingleton<NpgsqlConnection>(x =>
            {
                var postgresConnectionString = "User ID=postgres;Password=password;Host=localhost;Port=5432;";
                return new NpgsqlConnection(postgresConnectionString);
            });
            services.AddCouchbase(x =>
            {
                x.Servers = new List<Uri> {new Uri("http://localhost:8091")};
                x.Username = "Administrator";
                x.Password = "password";
                x.ConnectionPool = new ConnectionPoolDefinition
                {
                    MaxSize = 10
                };
            });

            services.AddTransient<HomeDeliveryRepository>();
            services.AddTransient<LoyaltyRepository>();
            services.AddTransient<LoyaltyCsvExportService>();
            services.AddTransient<LoyaltyCsvImportService>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "Customer 360 API", Version = "v1"});
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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

            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Customer 360 API v1"); });

            app.UseHttpsRedirection();

            app.UseRouting(routes =>
            {
                routes.MapControllers();
            });

            app.UseAuthorization();
        }
    }
}
