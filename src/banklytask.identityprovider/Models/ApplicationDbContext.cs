using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace banklytask.identityprovider.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationIdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
                
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ApplicationIdentityUser>(b =>
            {
                b.Property(e=> e.FirstName)
                    .IsRequired();

                b.Property(e=> e.LastName)
                    .IsRequired();

                b.Property(e => e.CreatedTime)
                    .HasColumnType("datetime")
                    .IsRequired()
                    .HasDefaultValue();
                
                b.Property(e => e.UpdatedTime)
                    .HasColumnType("datetime")
                    .HasDefaultValue();
            });
            
            base.OnModelCreating(builder);
        }
    }
    
    
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            // Get environment
            string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            // Build config
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            // Get connection string
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            var connectionString = config.GetConnectionString(nameof(ApplicationDbContext));
            var migrationAss = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            optionsBuilder.UseSqlite(connectionString, b => b.MigrationsAssembly(migrationAss));
            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}