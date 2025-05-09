namespace RealtyHub.Core.Requests.Contracts;

/// <summary>
/// Classe que representa uma requisição para obter um contrato específico pelo seu Id.
/// </summary>
/// <remarks>
/// A classe herda de <c><see cref="Request"/></c>, que contém propriedades comuns para requisições.
/// </remarks>
public class GetContractByIdRequest : Request
{
    /// <summary>
    /// Identificador do contrato a ser recuperado.
    /// </summary>
    /// <value>O Id do contrato a ser recuperado.</value>
    public long Id { get; set; }
}