using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BankOfDotNet.Api.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BankOfDotNet.Api
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
            services.AddDbContext<BankContext>(options =>
                {
                    options.UseSqlite(Configuration.GetConnectionString("DefaultConnection")); 
                });
            
            services.AddControllers();
            
            services.AddSwaggerGen(c =>
            {
                c.OperationFilter<AuthenticationRequirementsOperationFilter>();
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "BankOfDotNet.Api", Version = "v1" });
                c.AddSecurityDefinition("oauth2",new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        Implicit = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri("https://localhost:5201/connect/authorize"),
                            Scopes = new Dictionary<string, string>
                            {
                                { "BankOfDotNetApi", "Api Scope" },
                            },
                            TokenUrl = new Uri("https://localhost:5201/connect/token")
                        }
                    },
                    In = ParameterLocation.Header,
                    BearerFormat = "Bearer ",
                    OpenIdConnectUrl = new Uri("https://localhost:5201/connect/authorize")
                });
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = "https://localhost:5201";
                    options.ApiName = "BankApi";
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "BankOfDotNet.Api v1");
                    c.OAuthClientId("swaggerapiui");
                    c.OAuthAppName("Swagger Api UI");
                });
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }

    public class AuthenticationRequirementsOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Security == null)
                operation.Security = new List<OpenApiSecurityRequirement>();


            var scheme = new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "bearer" } };
            operation.Security.Add(new OpenApiSecurityRequirement
            {
                [scheme] = new List<string>()
            });
        }
    }
}
