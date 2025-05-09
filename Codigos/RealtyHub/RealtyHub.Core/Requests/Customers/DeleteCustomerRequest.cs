namespace RealtyHub.Core.Requests.Customers;

/// <summary>
/// Classe que representa uma requisição para excluir um cliente.
/// </summary>
/// <remarks>
/// A classe herda de <c><see cref="Request"/></c>, que contém propriedades comuns para requisições.
/// </remarks>
public class DeleteCustomerRequest : Request
{
    /// <summary>
    /// Identificador do cliente a ser excluído.
    /// </summary>
    /// <value>O Id do cliente a ser excluído.</value>    
    public long Id { get; set; }
}