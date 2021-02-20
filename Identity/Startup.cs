using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Identity.DynamicUsers;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using IdentityServer4.Validation;
using IdentityServer4.Services;

namespace Identity
{
    public class Startup
    {

        public Startup(IConfiguration configuration)
        {          
            StaticConfig = configuration;
        }

        public static IConfiguration StaticConfig { get; private set; }
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddMvcCore();
          

            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(StaticConfig.GetConnectionString("ConnStr")));
          
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddIdentityServer(options =>
            {
                options.Events.RaiseSuccessEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseErrorEvents = true;
            })
            .AddDeveloperSigningCredential()
            .AddInMemoryIdentityResources(Config.GetIdentityResources())
            .AddInMemoryApiResources(Config.GetApiResources())
            .AddInMemoryApiScopes(Config.Scopes())
            .AddInMemoryClients(Config.GetClients())
            // .AddCustomUserStore()
            .AddProfileService<CustomProfileService>();

            //Inject the classes we just created
          
            services.AddTransient<IResourceOwnerPasswordValidator, CustomResourceOwnerPasswordValidator>();
            services.AddTransient<IProfileService, CustomProfileService>();          
                
               
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseIdentityServer();
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });

          
        }
    }
}
