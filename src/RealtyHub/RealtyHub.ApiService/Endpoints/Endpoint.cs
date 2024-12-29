using RealtyHub.ApiService.Common.Api;
using RealtyHub.ApiService.Endpoints.Customers;
using RealtyHub.ApiService.Endpoints.Identity;
using RealtyHub.ApiService.Endpoints.Properties;
using RealtyHub.ApiService.Endpoints.Viewings;
using RealtyHub.ApiService.Models;

namespace RealtyHub.ApiService.Endpoints;

public static class Endpoint
{
    public static void MapEndpoints(this WebApplication app)
    {
        var endpoints = app
            .MapGroup("");

        endpoints.MapGroup("/")
            .WithTags("Health Check")
            .MapGet("/", () => new { message = "Ok" });

        endpoints.MapGroup("v1/identity")
            .WithTags("Identity")
            .MapIdentityApi<User>();

        endpoints.MapGroup("v1/identity")
            .WithTags("Identity")
            .MapEndpoint<LogoutEndpoint>()
            .MapEndpoint<GetRolesEndpoint>();

        endpoints.MapGroup("v1/customers")
            .WithTags("Customers")
            .RequireAuthorization()
            .MapEndpoint<CreateCustomerEndpoint>()
            .MapEndpoint<UpdateCustomerEndpoint>()
            .MapEndpoint<DeleteCustomerEndpoint>()
            .MapEndpoint<GetCustomerByIdEndpoint>()
            .MapEndpoint<GetAllCustomersEndpoint>();

        endpoints.MapGroup("v1/properties")
            .WithTags("Properties")
            .RequireAuthorization()
            .MapEndpoint<CreatePropertyEndpoint>()
            .MapEndpoint<UpdatePropertyEndpoint>()
            .MapEndpoint<DeletePropertyEndpoint>()
            .MapEndpoint<GetPropertyByIdEndpoint>()
            .MapEndpoint<GetAllPropertiesEndpoint>();

        endpoints.MapGroup("v1/viewings")
            .WithTags("Viewings")
            .RequireAuthorization()
            .MapEndpoint<CreateViewingEndpoint>()
            .MapEndpoint<UpdateViewingEndpoint>()
            .MapEndpoint<DeleteViewingEndpoint>()
            .MapEndpoint<GetViewingByIdEndpoint>()
            .MapEndpoint<GetAllViewingsEndpoint>();
    }

    private static IEndpointRouteBuilder MapEndpoint<TEndpoint>(this IEndpointRouteBuilder app)
        where TEndpoint : IEndpoint
    {
        TEndpoint.Map(app);
        return app;
    }
}