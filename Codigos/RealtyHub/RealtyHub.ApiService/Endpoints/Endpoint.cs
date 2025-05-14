using RealtyHub.ApiService.Common.Api;
using RealtyHub.ApiService.Endpoints.Condominiums;
using RealtyHub.ApiService.Endpoints.Contracts;
using RealtyHub.ApiService.Endpoints.ContractsTemplates;
using RealtyHub.ApiService.Endpoints.Customers;
using RealtyHub.ApiService.Endpoints.Emails;
using RealtyHub.ApiService.Endpoints.Identity;
using RealtyHub.ApiService.Endpoints.Offers;
using RealtyHub.ApiService.Endpoints.Properties;
using RealtyHub.ApiService.Endpoints.Reports;
using RealtyHub.ApiService.Endpoints.Viewings;
using RealtyHub.ApiService.Models;

namespace RealtyHub.ApiService.Endpoints;

/// <summary>
/// Classe responsável por mapear todos os endpoints da aplicação.
/// </summary>
/// <remarks>
/// Esta classe organiza e registra os endpoints agrupados por suas respectivas áreas funcionais,
/// como clientes, imóveis, condomínios, contratos, entre outros.
/// </remarks>
public static class Endpoint
{
    /// <summary>
    /// Mapeia todos os endpoints da aplicação.
    /// </summary>
    /// <param name="app">A instância do aplicativo <see cref="WebApplication"/>.</param>
    public static void MapEndpoints(this WebApplication app)
    {
        var endpoints = app.MapGroup("");

        // Health Check
        endpoints.MapGroup("/")
            .WithTags("Health Check")
            .MapGet("/", () => new { message = "Ok" });

        // Identity Endpoints
        endpoints.MapGroup("v1/identity")
            .WithTags("Identity")
            .MapIdentityApi<User>();

        endpoints.MapGroup("v1/identity")
            .WithTags("Identity")
            .MapEndpoint<LogoutEndpoint>()
            .MapEndpoint<RegisterUserEndpoint>()
            .MapEndpoint<ManageInfoEndpoint>()
            .MapEndpoint<ConfirmEmailEndpoint>()
            .MapEndpoint<ForgotPasswordEndpoint>()
            .MapEndpoint<ResetPasswordEndpoint>();

        // Customer Endpoints
        endpoints.MapGroup("v1/customers")
            .WithTags("Customers")
            .RequireAuthorization()
            .MapEndpoint<CreateCustomerEndpoint>()
            .MapEndpoint<UpdateCustomerEndpoint>()
            .MapEndpoint<DeleteCustomerEndpoint>()
            .MapEndpoint<GetCustomerByIdEndpoint>()
            .MapEndpoint<GetAllCustomersEndpoint>();

        // Property Endpoints
        endpoints.MapGroup("v1/properties")
            .WithTags("Properties")
            .RequireAuthorization()
            .MapEndpoint<CreatePropertyEndpoint>()
            .MapEndpoint<UpdatePropertyEndpoint>()
            .MapEndpoint<DeletePropertyEndpoint>()
            .MapEndpoint<CreatePropertyPhotosEndpoint>()
            .MapEndpoint<DeletePropertyPhotoEndpoint>()
            .MapEndpoint<GetAllViewingsByPropertyEndpoint>()
            .MapEndpoint<UpdatePropertyPhotosEndpoint>();

        endpoints.MapGroup("v1/properties")
            .WithTags("Properties")
            .MapEndpoint<GetPropertyByIdEndpoint>()
            .MapEndpoint<GetAllPropertyPhotosByPropertyEndpoint>()
            .MapEndpoint<GetAllPropertiesEndpoint>();

        // Condominium Endpoints
        endpoints.MapGroup("v1/condominiums")
            .WithTags("Condominiums")
            .RequireAuthorization()
            .MapEndpoint<CreateCondominiumEndpoint>()
            .MapEndpoint<UpdateCondominiumEndpoint>()
            .MapEndpoint<DeleteCondominiumEndpoint>()
            .MapEndpoint<GetCondominiumByIdEndpoint>()
            .MapEndpoint<GetAllCondominiumsEndpoint>();

        // Viewing Endpoints
        endpoints.MapGroup("v1/viewings")
            .WithTags("Viewings")
            .RequireAuthorization()
            .MapEndpoint<ScheduleViewingEndpoint>()
            .MapEndpoint<RescheduleViewingEndpoint>()
            .MapEndpoint<CancelViewingEndpoint>()
            .MapEndpoint<GetViewingByIdEndpoint>()
            .MapEndpoint<GetAllViewingsEndpoint>()
            .MapEndpoint<DoneViewingEndpoint>();

        // Offer Endpoints
        endpoints.MapGroup("v1/offers")
            .WithTags("Offers")
            .RequireAuthorization()
            .MapEndpoint<UpdateOfferEndpoint>()
            .MapEndpoint<AcceptOfferEndpoint>()
            .MapEndpoint<RejectOfferEndpoint>()
            .MapEndpoint<GetOfferByIdEndpoint>()
            .MapEndpoint<GetAllOffersByPropertyEndpoint>()
            .MapEndpoint<GetAllOffersByCustomerEndpoint>()
            .MapEndpoint<GetAllOffersEndpoint>()
            .MapEndpoint<GetOfferAcceptedEndpoint>();

        endpoints.MapGroup("v1/offers")
            .WithTags("Offers")
            .MapEndpoint<CreateOfferEndpoint>();

        // Contract Endpoints
        endpoints.MapGroup("v1/contracts")
            .WithTags("Contracts")
            .RequireAuthorization()
            .MapEndpoint<CreateContractEndpoint>()
            .MapEndpoint<UpdateContractEndpoint>()
            .MapEndpoint<DeleteContractEndpoint>()
            .MapEndpoint<GetContractByIdEndpoint>()
            .MapEndpoint<GetAllContractsEndpoint>();

        // Contract Templates Endpoints
        endpoints.MapGroup("v1/contracts-templates")
            .WithTags("Contract Templates")
            .RequireAuthorization()
            .MapEndpoint<GetAllContractTemplatesEndpoint>();

        // Report Endpoints
        endpoints.MapGroup("v1/reports")
            .WithTags("Reports")
            .RequireAuthorization()
            .MapEndpoint<OfferReportEndpoint>()
            .MapEndpoint<PropertyReportEndpoint>();

        // Email Endpoints
        endpoints.MapGroup("v1/emails")
            .WithTags("Emails")
            .RequireAuthorization()
            .MapEndpoint<SendContractEmailEndpoint>();
    }

    /// <summary>
    /// Método auxiliar para mapear um endpoint genérico.
    /// </summary>
    /// <typeparam name="TEndpoint">O tipo do endpoint que implementa <see cref="IEndpoint"/>.</typeparam>
    /// <param name="app">O construtor de rotas do aplicativo.</param>
    /// <returns>O construtor de rotas atualizado.</returns>
    private static IEndpointRouteBuilder MapEndpoint<TEndpoint>(this IEndpointRouteBuilder app)
        where TEndpoint : IEndpoint
    {
        TEndpoint.Map(app);
        return app;
    }
}