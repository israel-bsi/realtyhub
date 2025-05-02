using RealtyHub.Core.Models;
using RealtyHub.Core.Responses;

namespace RealtyHub.Core.Handlers;

/// <summary>
/// Interface que define os métodos para manipulação de modelos de contrato.
/// </summary>
/// <remarks>
/// Esta interface é responsável por definir as operações que podem ser realizadas
/// com os modelos de contrato, como a recuperação de todos os modelos disponíveis.
/// </remarks>
public interface IContractTemplateHandler
{
    /// <summary>
    /// Recupera todos os modelos de contrato disponíveis.
    /// </summary>
    /// <remarks>
    /// Este método é responsável por buscar todos os modelos de contrato
    /// armazenados no sistema e retornar uma lista com os resultados.
    /// </remarks>
    /// <returns>
    /// Retorna um objeto <see cref="Response{TData}"/> contendo uma lista de modelos de contrato.
    /// </returns>
    Task<Response<List<ContractTemplate>?>> GetAllAsync();
}