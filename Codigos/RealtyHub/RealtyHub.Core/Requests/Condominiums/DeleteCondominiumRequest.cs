using System.ComponentModel.DataAnnotations;

namespace RealtyHub.Core.Requests.Condominiums;

/// <summary>
/// Classe que representa uma requisição para excluir um condomínio.
/// </summary>
/// <remarks>
/// Esta classe é usada para encapsular os dados necessários para excluir um condomínio específico.
/// </remarks>
public class DeleteCondominiumRequest : Request
{
    /// <summary>
    /// Identificador do condomínio a ser excluído.
    /// </summary>
    /// <remarks>
    /// Este campo é obrigatório e deve conter um valor válido para que a operação de exclusão seja realizada.
    /// </remarks>
    /// <value>O Id do condomínio a ser excluído.</value>
    public long Id { get; set; }
}