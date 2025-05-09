namespace RealtyHub.Core.Requests.Contracts;

/// <summary>
/// Classe que representa uma requisição para excluir um contrato.
/// </summary>
/// <remarks>
/// A classe herda de <c><see cref="Request"/></c>, que contém propriedades comuns para requisições.
/// </remarks>
public class DeleteContractRequest : Request
{

    /// <summary>
    /// Identificador do contrato a ser excluído.
    /// </summary>
    /// <value> O Id do contrato a ser excluído </value>
    public long Id { get; set; }
}