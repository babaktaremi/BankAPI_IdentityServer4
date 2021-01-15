using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BankOfDotNet.IdentityServer
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();


            var migrationAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                //.AddInMemoryApiResources(Config.GetAllApiResources())
                //.AddInMemoryIdentityResources(Config.GetIdentityResources())
                //.AddInMemoryApiScopes(Config.Scopes)
                .AddTestUsers(Config.GetUsers())
                .AddInMemoryClients(Config.GetAllClients())
                //client Resources
                .AddConfigurationStore(options =>
                    {
                        options.ConfigureDbContext = builder =>
                            builder.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), sql => sql.MigrationsAssembly(migrationAssembly));
                        //Operational Stores
                    }).AddOperationalStore(options =>
                    {
                        options.ConfigureDbContext = builder =>
                            builder.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), sql => sql.MigrationsAssembly(migrationAssembly));
                    });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //InitDatabase(app);


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseRouting();

            app.UseIdentityServer();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private void InitDatabase(IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope();

            //scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

            var context = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

            //context.Database.Migrate();

            if (context.Clients.Any())
            {
                foreach (var allClient in Config.GetAllClients().Where(c=>c.ClientId.Equals("swaggerapiui")))
                {
                    context.Clients.Add(allClient.ToEntity());
                }

                context.SaveChanges();
            }

            if (!context.IdentityResources.Any())
            {
                foreach (var allClient in Config.GetIdentityResources())
                {
                    context.IdentityResources.Add(allClient.ToEntity());
                }

                context.SaveChanges();
            }


            if (!context.ApiResources.Any())
            {
                foreach (var allClient in Config.GetAllApiResources())
                {
                    context.ApiResources.Add(allClient.ToEntity());
                }

                context.SaveChanges();
            }

            if (!context.ApiScopes.Any())
            {
                foreach (var allClient in Config.Scopes)
                {
                    context.ApiScopes.Add(allClient.ToEntity());
                }

                context.SaveChanges();
            }
        }
    }
}
