﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using RealtyHub.ApiService.Data;
using RealtyHub.ApiService.Handlers;
using RealtyHub.ApiService.Models;
using RealtyHub.ApiService.Services;
using RealtyHub.Core;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Services;
using RealtyHub.ServiceDefaults;

namespace RealtyHub.ApiService.Common.Api;

public static class BuilderExtension
{
    public static void AddConfiguration(this WebApplicationBuilder builder)
    {
        Configuration.ConnectionString =
            builder
                .Configuration
                .GetConnectionString("DefaultConnection")
            ?? string.Empty;
        Configuration.BackendUrl = builder.Configuration.GetValue<string>("BackendUrl") ?? string.Empty;
        Configuration.FrontendUrl = builder.Configuration.GetValue<string>("FrontendUrl") ?? string.Empty;
    }

    public static void AddDocumentation(this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.OrderActionsBy((apiDesc) => apiDesc.HttpMethod);
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
                    x.UseNpgsql(Configuration.ConnectionString)
                        .EnableSensitiveDataLogging()
                        .EnableDetailedErrors();
                });

        builder.Services
            .AddIdentityCore<User>()
            .AddRoles<IdentityRole<long>>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddApiEndpoints();
    }

    public static void AddCrossOrigin(this WebApplicationBuilder builder)
    {
        builder.Services.AddCors(
            options => options.AddPolicy(
                ApiConfiguration.CorsPolicyName,
                policy => policy
                    .WithOrigins([
                        Configuration.BackendUrl,
                        Configuration.FrontendUrl
                    ])
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
            ));
    }

    public static void AddServices(this WebApplicationBuilder builder)
    {
        // Add service defaults & Aspire client integrations.
        builder.AddServiceDefaults();

        // Add services to the container.
        builder.Services.AddProblemDetails();

        builder.Services.AddTransient<ICustomerHandler, CustomerHandler>();
        builder.Services.AddTransient<IPropertyHandler, PropertyHandler>();
        builder.Services.AddTransient<IViewingHandler, ViewingHandler>();
        builder.Services.AddTransient<IOfferHandler, OfferHandler>();

        builder.Services.AddTransient<IViaCepService, ViaCepService>();
    }
}