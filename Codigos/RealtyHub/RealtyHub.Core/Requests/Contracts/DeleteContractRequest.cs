namespace RealtyHub.Core.Requests.Contracts;

/// <summary>
/// Classe que representa uma requisição para excluir um contrato.
/// </summary>
/// <remarks>
/// Esta classe é usada para encapsular os dados necessários para excluir um contrato específico,
/// incluindo o Id do contrato a ser excluído.
/// </remarks>
public class DeleteContractRequest : Request
{

    /// <summary>
    /// Identificador do contrato a ser excluído.
    /// </summary>
    /// <remarks>
    /// Este campo é obrigatório e deve conter um valor válido para que a operação de exclusão seja realizada.
    /// </remarks>
    /// <value> O Id do contrato a ser excluído </value>
    public long Id { get; set; }
}