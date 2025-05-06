using System.ComponentModel.DataAnnotations;

namespace RealtyHub.Core.Requests.Customers;

/// <summary>
/// Classe que representa uma requisição para excluir um cliente.
/// </summary>
/// <remarks>
/// Esta classe é usada para encapsular os dados necessários para excluir um cliente específico.
/// </remarks>
public class DeleteCustomerRequest : Request
{
    /// <summary>
    /// Identificador do cliente a ser excluído.
    /// </summary>
    /// <remarks>
    /// Este campo é obrigatório e deve conter um valor válido para que a operação de exclusão seja realizada.
    /// </remarks>
    /// <value>O Id do cliente a ser excluído.</value>    
    public long Id { get; set; }
}