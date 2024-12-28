using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using RealtyHub.ApiService.Data;

namespace RealtyHub.Tests;

public class RealtyHubApiTests : WebApplicationFactory<Program>
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        var root = new InMemoryDatabaseRoot();

        builder.ConfigureServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<AppDbContext>));

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryDbForTesting", root);
            });
        });

        return base.CreateHost(builder);
    }
}