using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RealtyHub.ApiService.Data;
using RealtyHub.ApiService.Handlers;
using RealtyHub.ApiService.Models;
using RealtyHub.ApiService.Services;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Services;

namespace RealtyHub.ApiService.Common.Api;

public static class BuilderExtension
{
    public static void AddConfiguration(this WebApplicationBuilder builder)
    {
        Core.Configuration.ConnectionString =
            builder
                .Configuration
                .GetConnectionString("DefaultConnection")
            ?? string.Empty;

        Core.Configuration.BackendUrl = builder.Configuration
            .GetValue<string>("BackendUrl") ?? string.Empty;

        Core.Configuration.FrontendUrl = builder.Configuration
            .GetValue<string>("FrontendUrl") ?? string.Empty;
        
        Configuration.EmailSettings.EmailPassword =
            builder.Configuration.GetValue<string>("EmailPassword") ?? string.Empty;

        var root = Path.Combine(Directory.GetCurrentDirectory(), "Sources", "Contracts");
        if (!Directory.Exists(root))
            Directory.CreateDirectory(root);
    }
    public static void AddDocumentation(this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.OrderActionsBy((apiDesc) => apiDesc.GroupName);
            options.CustomSchemaIds(n => n.FullName);
        });
    }
    public static void AddSecurity(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddAuthentication(IdentityConstants.ApplicationScheme)
            .AddIdentityCookies();

        builder.Services.AddAuthorization();
    }
    public static void AddDataContexts(this WebApplicationBuilder builder)
    {
        builder
            .Services
            .AddDbContext<AppDbContext>(
                x =>
                {
                    x.UseNpgsql(Core.Configuration.ConnectionString)
                        .EnableSensitiveDataLogging()
                        .EnableDetailedErrors();
                });

        builder.Services
            .AddIdentityCore<User>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.SignIn.RequireConfirmedEmail = true;
                options.SignIn.RequireConfirmedPhoneNumber = false;
                options.User.RequireUniqueEmail = true;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredUniqueChars = 1;
            })
            .AddRoles<IdentityRole<long>>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddApiEndpoints();
    }
    public static void AddCrossOrigin(this WebApplicationBuilder builder)
    {
        builder.Services.AddCors(
            options => options.AddPolicy(
                Configuration.CorsPolicyName,
                policy => policy
                    .WithOrigins([
                        Core.Configuration.BackendUrl,
                        Core.Configuration.FrontendUrl
                    ])
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
            ));
    }
    public static void AddServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddProblemDetails();
        builder.Services.AddTransient<ICustomerHandler, CustomerHandler>();
        builder.Services.AddTransient<IPropertyHandler, PropertyHandler>();
        builder.Services.AddTransient<IViewingHandler, ViewingHandler>();
        builder.Services.AddTransient<IOfferHandler, OfferHandler>();
        builder.Services.AddTransient<IContractHandler, ContractHandler>();
        builder.Services.AddTransient<IEmailService, EmailService>();
        builder.Services.AddTransient<IPropertyPhotosHandler, PropertyPhotosHandler>();
    }
}