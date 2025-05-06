using System.ComponentModel.DataAnnotations;

namespace RealtyHub.Core.Requests.Condominiums;

/// <summary>
/// Classe que representa uma requisição para obter um condomínio específico pelo Id.
/// </summary>
/// <remarks>
/// Esta classe é usada para encapsular os dados necessários para recuperar um condomínio específico,
/// incluindo o Id do condomínio a ser recuperado.
/// </remarks>
public class GetCondominiumByIdRequest : Request
{
    /// <summary>
    /// Identificador do condomínio a ser recuperado.
    /// </summary>
    /// <remarks>
    /// Este campo é obrigatório e deve conter um valor válido para que a operação de recuperação seja realizada.
    /// </remarks>
    /// <value>O Id do condomínio a ser recuperado.</value>    
    public long Id { get; set; }
}