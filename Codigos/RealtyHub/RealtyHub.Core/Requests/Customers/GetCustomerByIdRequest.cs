namespace RealtyHub.Core.Requests.Customers;

/// <summary>
/// Classe que representa uma requisição para obter um cliente específico pelo seu Id.
/// </summary>
/// <remarks>
/// A classe herda de <c><see cref="Request"/></c>, que contém propriedades comuns para requisições.
/// </remarks>
public class GetCustomerByIdRequest : Request
{
    /// <summary>
    /// Identificador do cliente a ser recuperado.
    /// </summary>
    /// <value>O Id do cliente a ser recuperado.</value>
    public long Id { get; set; }
}