namespace RealtyHub.ApiService.Common.Api;

/// <summary>
/// Interface que define um endpoint para o aplicativo ASP.NET Core.
/// </summary>
/// <remarks>
/// Esta interface deve ser implementada por classes que definem
/// endpoints específicos do aplicativo, permitindo o mapeamento
/// de rotas e a configuração de serviços.
/// </remarks>
public interface IEndpoint
{
    /// <summary>
    /// Método responsável por mapear os endpoints do aplicativo.
    /// </summary>
    /// <param name="app">O construtor de rotas do aplicativo.</param>
    /// <remarks>
    /// Este método deve ser implementado para definir os endpoints
    /// específicos do aplicativo, incluindo métodos HTTP, rotas,
    /// parâmetros e respostas.
    /// </remarks>
    static abstract void Map(IEndpointRouteBuilder app);
}