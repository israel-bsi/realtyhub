namespace RealtyHub.Core.Requests.Contracts;

/// <summary>
/// Classe que representa uma requisição para obter um contrato específico pelo seu Id.
/// </summary>
/// <remarks>
/// Esta classe é usada para encapsular os dados necessários para recuperar um contrato específico,
/// incluindo o Id do contrato a ser recuperado.
/// </remarks>
public class GetContractByIdRequest : Request
{
    /// <summary>
    /// Identificador do contrato a ser recuperado.
    /// </summary>
    /// <remarks>
    /// Este campo é obrigatório e deve conter um valor válido para que a operação de recuperação seja realizada.
    /// </remarks>
    /// <value>O Id do contrato a ser recuperado.</value>
    public long Id { get; set; }
}