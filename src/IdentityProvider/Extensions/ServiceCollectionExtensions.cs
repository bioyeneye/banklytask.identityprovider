using System;
using System.Collections.Generic;
using System.Reflection;
using IdentityProvider.BusinessDomain.ServiceModels;
using IdentityProvider.BusinessDomain.Services.Authentication;
using IdentityProvider.DataAccess;
using IdentityProvider.DataAccess.Contexts;
using IdentityProvider.DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityProvider.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDbContextWithIdentity(this IServiceCollection services, IConfiguration configuration)
        {
            var identityOptions = configuration.GetSection(nameof(IdentityProviderOptions)).Get<IdentityProviderOptions>();

            var connectionString = configuration.GetConnectionString(nameof(ApplicationDbContext));
            var migrationsAssembly = typeof(ApplicationDbContext).GetTypeInfo().Assembly.GetName().Name;

            services.AddDbContext<ApplicationDbContext>(builder => builder.UseSqlServer(connectionString, sqlOptions => sqlOptions.MigrationsAssembly(migrationsAssembly)))
                .AddIdentity<ApplicationIdentityUser, IdentityRole>(options =>
                {
                    options.Password.RequireDigit = identityOptions.IdentityPasswordOptions.RequireDigit;
                    options.Password.RequiredLength = identityOptions.IdentityPasswordOptions.RequiredLength;
                    options.Password.RequireNonAlphanumeric = identityOptions.IdentityPasswordOptions.RequireNonAlphanumeric;
                    options.Password.RequireUppercase = identityOptions.IdentityPasswordOptions.RequireUppercase;
                    options.Password.RequireLowercase = identityOptions.IdentityPasswordOptions.RequireLowercase;
                    options.Lockout.AllowedForNewUsers = identityOptions.IdentityLockoutOptions.AllowedForNewUsers;
                    options.Lockout.MaxFailedAccessAttempts = identityOptions.IdentityLockoutOptions.MaxFailedAccessAttempts;
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(identityOptions.IdentityLockoutOptions.DefaultLockoutTime);
                })
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            
            return services;
        }

        public static void AddIdentityServerConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString(nameof(ApplicationDbContext));
            var migrationsAssembly = typeof(ApplicationDbContext).GetTypeInfo().Assembly.GetName().Name;

            IIdentityServerBuilder ids = services.AddIdentityServer()
                .AddCustomUserStore()
                .AddDeveloperSigningCredential();

            // EF client, scope, and persisted grant stores
            ids.AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = (builder =>
                    {
                        builder.UseSqlServer(connectionString, sqlOptions => sqlOptions.MigrationsAssembly(migrationsAssembly));

                    });

                    //this helps with token cleanup
                    /*options.EnableTokenCleanup = true;
                    options.TokenCleanupInterval = 3600; // in seconds*/
                })
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = (builder =>
                    {
                        builder.UseSqlServer(connectionString, sqlOptions => sqlOptions.MigrationsAssembly(migrationsAssembly));
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
            builder.Services.AddTransient<IUserService, UserService>();
            builder.AddProfileService<CustomProfileService>();
            builder.AddResourceOwnerValidator<CustomResourceOwnerPasswordValidator>();
            return builder;
        }
    }
}