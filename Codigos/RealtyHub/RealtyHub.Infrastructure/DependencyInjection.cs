using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RealtyHub.Infrastructure.Data;

namespace RealtyHub.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(
            x =>
            {
                x.UseNpgsql(Core.Configuration.ConnectionString)
                    .EnableSensitiveDataLogging()
                    .EnableDetailedErrors();
            });

        return services;
    }
}