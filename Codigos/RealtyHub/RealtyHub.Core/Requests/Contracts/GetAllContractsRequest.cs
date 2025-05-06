namespace RealtyHub.Core.Requests.Contracts;

/// <summary>
/// Classe que representa uma requisição para obter todos os contratos com paginação.
/// </summary>
/// <remarks>
/// Esta classe é usada para encapsular os dados necessários para recuperar uma lista de contratos,
/// incluindo informações de paginação, como número da página e tamanho da página.
/// </remarks>
public class GetAllContractsRequest : PagedRequest { }