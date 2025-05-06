namespace RealtyHub.Core.Requests.Customers;

/// <summary>
/// Classe que representa uma requisição para obter um cliente específico pelo seu Id.
/// </summary>
/// <remarks>
/// Esta classe é usada para encapsular os dados necessários para recuperar um cliente específico,
/// incluindo o Id do cliente a ser recuperado.
/// </remarks>
public class GetCustomerByIdRequest : Request
{
    /// <summary>
    /// Identificador do cliente a ser recuperado.
    /// </summary>
    /// <remarks>
    /// Este campo é obrigatório e deve conter um valor válido para que a operação de recuperação seja realizada.
    /// </remarks>
    /// <value>O Id do cliente a ser recuperado.</value>
    public long Id { get; set; }
}