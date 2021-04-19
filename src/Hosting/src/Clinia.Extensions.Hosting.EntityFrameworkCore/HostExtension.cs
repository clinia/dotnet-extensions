using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Hosting
{
    public static class HostExtension
    {
        public static IHost MigrateDbContext<TContext>(
            this IHost host, 
            Action<TContext, IServiceProvider> preHook,
            Action<TContext, IServiceProvider> seeder) where TContext : DbContext
        {
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<TContext>>();
            var context = services.GetService<TContext>();
            
            try
            {
                logger.LogInformation("Migrating database associated with context {DbContextName}", typeof(TContext).Name);
                
                var retries = 10;
                var retry = Policy.Handle<Exception>()
                    .WaitAndRetry(retries, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                        (exception, timeSpan, retry, ctx) =>
                        {
                            logger.LogWarning(exception,
                                "[{prefix}] Exception {ExceptionType} with message {Message} detected on attempt {retry} of {retries}",
                                nameof(TContext), exception.GetType().Name, exception.Message, retry, retries);
                        });
                
                // if the sql server container is not created on run docker compose this
                // migration can't fail for network related exception. The retry options for DbContext only 
                // apply to transient exceptions
                retry.Execute(() =>
                {
                    preHook?.Invoke(context, services);
                    InvokeSeeder(seeder, context, services);
                });
                
                logger.LogInformation("Migrated database associated with context {DbContextName}", typeof(TContext).Name);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while migrating the database used on context {DbContextName}", typeof(TContext).Name);
                throw;
            }

            return host;
        }
        
        private static void InvokeSeeder<TContext>(
            Action<TContext, IServiceProvider> seeder,
            TContext context,
            IServiceProvider services)
            where TContext : DbContext
        {
            context.Database.Migrate();
            seeder?.Invoke(context, services);
        }
    }
}