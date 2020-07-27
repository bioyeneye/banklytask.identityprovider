using System;
using System.Linq;
using IdentityProvider.DataAccess.Contexts;
using IdentityProvider.DataAccess.Entities;
using IdentityProvider.DataAccess.Entities.Enums;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IdentityProvider.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseConfigureSecurityHeaders(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (!env.IsDevelopment())
            {
                app.UseHsts(options => options.MaxAge(days: 365).IncludeSubdomains());
            }
            
            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("X-Frame-Options", "DENY");
                await next();
            });

            app.UseHttpsRedirection();

            /*app.UseCsp(options =>
            {
                options.DefaultSources(directive => directive.Self());
                options.BlockAllMixedContent().FormActions(s => s.Self()).FrameAncestors(s => s.Self());
                options.ImageSources(directive => directive.Self().CustomSources("*"));
                options.ScriptSources(directive => directive.Self().UnsafeInline());
                options.StyleSources(directive => directive.Self().UnsafeInline());
            });*/

            app.UseXContentTypeOptions();
            app.UseReferrerPolicy(options => options.NoReferrer());
            app.UseXXssProtection(options => options.EnabledWithBlockMode());
            app.UseXfo(options => options.Deny());
            
            app.Use((context, next) =>
            {
                if (context.Request.IsHttps)
                {
                    context.Response.Headers.Append("Expect-CT", $"max-age=0; report-uri=\"https://tranicars.com/report-ct\"");
                }
                return next.Invoke();
            });
            
            var forwardOptions = new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
                RequireHeaderSymmetry = false
            };

            forwardOptions.KnownNetworks.Clear();
            forwardOptions.KnownProxies.Clear();
            app.UseForwardedHeaders(forwardOptions);

            return app;
        }
        
         public static void InitializeDbTestData(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();
                serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>().Database.Migrate();
                serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Database.Migrate();

                var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

                if (!context.Clients.Any())
                {
                    foreach (var client in Clients.Get())
                    {
                        context.Clients.Add(client.ToEntity());
                    }

                    context.SaveChanges();
                }

                if (!context.IdentityResources.Any())
                {
                    foreach (var resource in Resources.GetIdentityResources())
                    {
                        context.IdentityResources.Add(resource.ToEntity());
                    }

                    context.SaveChanges();
                }

                if (!context.ApiScopes.Any())
                {
                    foreach (var scope in Resources.GetApiScopes())
                    {
                        context.ApiScopes.Add(scope.ToEntity());
                    }

                    context.SaveChanges();
                }

                if (!context.ApiResources.Any())
                {
                    foreach (var resource in Resources.GetApiResources())
                    {
                        context.ApiResources.Add(resource.ToEntity());
                    }

                    context.SaveChanges();
                }

                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<ApplicationIdentityUser>>();
                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                
                if (!roleManager.Roles.Any())
                {
                    var roles = Enum.GetNames(typeof(RolesConstants.Enum));
                    foreach (var role in roles)
                    {
                        if (!roleManager.Roles.Any(c=> c.Name == role))
                        {
                            roleManager.CreateAsync(new IdentityRole() { Name = role }).Wait();
                        }
                    }
                }
                
                if (!userManager.Users.Any())
                {
                    foreach (var testUser in Users.Get())
                    {
                        var identityUser = new ApplicationIdentityUser
                        {
                            UserName = testUser.Username,
                            Id = testUser.SubjectId,
                            Email = testUser.Username,
                            EmailConfirmed = true,
                            FirstName = testUser.FirstName,
                            LastName = testUser.LastName,
                            CreatedTime = DateTime.UtcNow
                        };

                        userManager.CreateAsync(identityUser, testUser.Password).Wait();
                        userManager.AddClaimsAsync(identityUser, testUser.Claims.ToList()).Wait();
                        userManager.AddToRoleAsync(identityUser, testUser.RoleName).Wait();
                    }
                }
            }
        }
    }
}