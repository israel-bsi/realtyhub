namespace RealtyHub.Core.Requests.Condominiums;

/// <summary>
/// Classe que representa uma requisição para obter um condomínio específico pelo Id.
/// </summary>
/// <remarks>
/// A classe herda de <c><see cref="Request"/></c>, que contém propriedades comuns para requisições.
/// </remarks>
public class GetCondominiumByIdRequest : Request
{
    /// <summary>
    /// Identificador do condomínio a ser recuperado.
    /// </summary>    
    /// <value>O Id do condomínio a ser recuperado.</value>    
    public long Id { get; set; }
}