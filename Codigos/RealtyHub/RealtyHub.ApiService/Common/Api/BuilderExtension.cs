using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RealtyHub.ApiService.Data;
using RealtyHub.ApiService.Handlers;
using RealtyHub.ApiService.Models;
using RealtyHub.ApiService.Services;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Services;

namespace RealtyHub.ApiService.Common.Api;

/// <summary>
/// Classe estática que agrupa métodos de extensão para configurar
/// documentações, diretórios, autenticação, autorização e serviços
/// da aplicação.
/// </summary>
public static class BuilderExtension
{
    /// <summary>
    /// Configura diretórios necessários para a aplicação.
    /// </summary>
    /// <remarks>
    /// Este método cria diretórios para armazenar contratos,
    /// fotos, modelos de contratos, relatórios e logos.
    /// </remarks>
    /// <param name="builder">Instância do construtor da aplicação.</param>
    public static void AddDirectories(this WebApplicationBuilder builder)
    {
        var basePath = builder.Environment.ContentRootPath;

        Configuration.ContractsPath = Path.Combine(basePath, "Sources", "Contracts");
        if (!Directory.Exists(Configuration.ContractsPath))
            Directory.CreateDirectory(Configuration.ContractsPath);

        Configuration.PhotosPath = Path.Combine(basePath, "Sources", "Photos");
        if (!Directory.Exists(Configuration.PhotosPath))
            Directory.CreateDirectory(Configuration.PhotosPath);

        Configuration.ContractTemplatesPath = Path.Combine(basePath, "Sources", "ContractTemplates");
        if (!Directory.Exists(Configuration.ContractTemplatesPath))
            Directory.CreateDirectory(Configuration.ContractTemplatesPath);

        Configuration.ReportsPath = Path.Combine(basePath, "Sources", "Reports");
        if (!Directory.Exists(Configuration.ReportsPath))
            Directory.CreateDirectory(Configuration.ReportsPath);

        Configuration.LogosPath = Path.Combine(basePath, "Sources", "Logos");
        if (!Directory.Exists(Configuration.LogosPath))
            Directory.CreateDirectory(Configuration.LogosPath);
    }

    /// <summary>
    /// Configura as variáveis de ambiente e caminhos de conexão.
    /// </summary>
    /// <remarks>
    /// Este método lê as variáveis de ambiente e configura
    /// as strings de conexão necessárias para a aplicação.
    /// </remarks>
    /// <param name="builder">Instância do construtor da aplicação.</param>
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
    }

    /// <summary>
    /// Configura e ativa a documentação da API com Swagger.
    /// </summary>
    /// <remarks>
    /// Este método adiciona o Swagger à aplicação, permitindo
    /// visualizar a documentação da API e testar os endpoints.
    /// </remarks>
    /// <param name="builder">Instância do construtor da aplicação.</param>
    public static void AddDocumentation(this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.OrderActionsBy((apiDesc) => apiDesc.GroupName);
            options.CustomSchemaIds(n => n.FullName);
        });
    }

    /// <summary>
    /// Adiciona e configura a autenticação e autorização da aplicação.
    /// </summary>
    /// <remarks>
    /// Este método configura a autenticação com cookies e JWT,
    /// permitindo o uso de autenticação baseada em token.
    /// </remarks>
    /// <param name="builder">Instância do construtor da aplicação.</param>
    public static void AddSecurity(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddAuthentication(IdentityConstants.ApplicationScheme)
            .AddIdentityCookies();

        builder.Services.AddAuthorization();
    }

    /// <summary>
    /// Configura os contextos de dados, habilitando o Entity Framework e Identity.
    /// </summary>
    /// <remarks>
    /// Este método adiciona o DbContext da aplicação e configura
    /// o Identity para gerenciar usuários e roles.
    /// </remarks>
    /// <param name="builder">Instância do construtor da aplicação.</param>
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

    /// <summary>
    /// Configura o CORS para permitir chamadas de origens específicas.
    /// </summary>
    /// <remarks>
    /// Este método adiciona uma política de CORS que permite
    /// chamadas de origens específicas, como o frontend e o backend.
    /// </remarks>
    /// <param name="builder">Instância do construtor da aplicação.</param>
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

    /// <summary>
    /// Registra serviços adicionais necessários para a aplicação.
    /// </summary>
    /// <remarks>
    /// Este método adiciona serviços como manipuladores, serviços de email e templates de contratos.
    /// </remarks>
    /// <param name="builder">Instância do construtor da aplicação.</param>
    public static void AddServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddProblemDetails();
        builder.Services.AddTransient<ICustomerHandler, CustomerHandler>();
        builder.Services.AddTransient<IPropertyHandler, PropertyHandler>();
        builder.Services.AddTransient<ICondominiumHandler, CondominiumHandler>();
        builder.Services.AddTransient<IViewingHandler, ViewingHandler>();
        builder.Services.AddTransient<IOfferHandler, OfferHandler>();
        builder.Services.AddTransient<IContractHandler, ContractHandler>();
        builder.Services.AddTransient<IEmailService, EmailService>();
        builder.Services.AddTransient<IPropertyPhotosHandler, PropertyPhotosHandler>();
        builder.Services.AddTransient<IContractTemplateHandler, ContractTemplatesHandler>();
    }
}