using System;
using System.Collections.Generic;
using System.Reflection;
using banklytask.identityprovider.Models;
using banklytask.identityprovider.Services.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace banklytask.identityprovider.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDbContextWithIdentity(this IServiceCollection services, IConfiguration configuration)
        {

            var connectionString = configuration.GetConnectionString(nameof(ApplicationDbContext));
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            services.AddDbContext<ApplicationDbContext>(builder => builder.UseSqlite(connectionString, sqlOptions => sqlOptions.MigrationsAssembly(migrationsAssembly)))
                .AddIdentity<ApplicationIdentityUser, IdentityRole>(options =>
                {
                    options.Password.RequireDigit = true;
                    options.Password.RequiredLength = 8;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = true;
                    options.Password.RequireLowercase = false;
                    options.Lockout.AllowedForNewUsers = true;
                    options.Lockout.MaxFailedAccessAttempts = 3;
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(3);
                })
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            
            return services;
        }

        public static void AddIdentityServerConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString(nameof(ApplicationDbContext));
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            IIdentityServerBuilder ids = services.AddIdentityServer()
                .AddCustomUserStore()
                .AddDeveloperSigningCredential();

            // EF client, scope, and persisted grant stores
            ids.AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = (builder =>
                    {
                        builder.UseSqlite(connectionString, sqlOptions => sqlOptions.MigrationsAssembly(migrationsAssembly));

                    });

                    //this helps with token cleanup
                    /*options.EnableTokenCleanup = true;
                    options.TokenCleanupInterval = 3600; // in seconds*/
                })
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = (builder =>
                    {
                        builder.UseSqlite(connectionString, sqlOptions => sqlOptions.MigrationsAssembly(migrationsAssembly));
                    });
                });

            // ASP.NET Identity integration
            //ids.AddAspNetIdentity<ApplicationIdentityUser>();
        }

        public static IServiceCollection AddCustomPolicy(this IServiceCollection services)
        {
            services.AddAuthorization(options =>  
            {  
                options.AddPolicy("OnlyAdministrator", policy => 
                    policy.RequireClaim("Role", new List<string>
                    {
                        "Administrator"
                    }));  
            });

            return services;
        }
        
        public static IIdentityServerBuilder AddCustomUserStore(this IIdentityServerBuilder builder)
        {
            builder.Services.AddTransient<IUserRepository, UserRepository>();
            builder.AddProfileService<CustomProfileService>();
            builder.AddResourceOwnerValidator<CustomResourceOwnerPasswordValidator>();
            return builder;
        }
    }
}