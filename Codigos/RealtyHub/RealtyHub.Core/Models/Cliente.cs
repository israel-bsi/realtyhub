using RealtyHub.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace RealtyHub.Core.Models;

/// <summary>
/// Representa um cliente no sistema.
/// </summary>
/// <remarks>
/// Esta classe herda de <c><see cref="Entidade"/></c>, 
/// que contém propriedades comuns a todas as entidades do sistema.
/// </remarks>
public class Cliente : Entidade
{
    /// <summary>
    /// Obtém ou define o ID do cliente.
    /// </summary>
    /// <value>Um valor inteiro representando o ID.</value>
    public long Id { get; set; }

    /// <summary>
    /// Obtém ou define o nome do cliente.
    /// </summary>
    /// <value>Uma string contendo o nome do cliente.</value>
    [Required(ErrorMessage = "Nome é um campo obrigatório")]
    [MaxLength(80, ErrorMessage = "O nome deve conter até 80 caracteres")]
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Obtém ou define o e-mail do cliente.
    /// </summary>
    /// <value>Uma string representando o e-mail do cliente.</value>
    [Required(ErrorMessage = "Email é um campo obrigatório")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    [MaxLength(50, ErrorMessage = "O e-mail deve conter até 80 caracteres")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Obtém ou define o telefone do cliente.
    /// </summary>
    /// <value>Uma string contendo o telefone do cliente.</value>
    [Required(ErrorMessage = "Telefone é um campo obrigatório")]
    [Phone(ErrorMessage = "Telefone inválido")]
    [MaxLength(30, ErrorMessage = "O telefone deve conter até 80 caracteres")]
    public string Telefone { get; set; } = string.Empty;

    /// <summary>
    /// Obtém ou define o número do documento do cliente.
    /// </summary>
    /// <value>Uma string representando o número do documento.</value>
    [Required(ErrorMessage = "Documento é um campo obrigatório")]
    [MaxLength(20, ErrorMessage = "O documento deve conter até 20 caracteres")]
    public string NumeroDocumento { get; set; } = string.Empty;

    /// <summary>
    /// Obtém ou define a ocupação do cliente.
    /// </summary>
    /// <value>Uma string contendo a ocupação.</value>
    public string Ocupacao { get; set; } = string.Empty;

    /// <summary>
    /// Obtém ou define a nacionalidade do cliente.
    /// </summary>
    /// <value>Uma string representando a nacionalidade.</value>
    public string Nacionalidade { get; set; } = string.Empty;

    /// <summary>
    /// Obtém ou define o tipo de pessoa do cliente.
    /// </summary>
    /// <value>Um valor do enum <see cref="ETipoPessoa"/> indicando o tipo de pessoa.</value>
    public ETipoPessoa TipoPessoa { get; set; }

    /// <summary>
    /// Obtém ou define o tipo de cliente.
    /// </summary>
    /// <value>Um valor do enum <see cref="ETipoCliente"/> indicando o tipo de cliente. O padrão é <see cref="ETipoCliente.Comprador"/>.</value>
    public ETipoCliente TipoCliente { get; set; } = ETipoCliente.Comprador;

    /// <summary>
    /// Obtém ou define o endereço do cliente.
    /// </summary>
    /// <value>Um objeto <see cref="Endereco"/> representando o endereço. A validação complexa é aplicada para validar suas propriedades.</value>
    [ValidateComplexType]
    public Endereco Endereco { get; set; } = new();

    /// <summary>
    /// Obtém ou define o RG do cliente.
    /// </summary>
    /// <value>Uma string representando o RG.</value>
    public string Rg { get; set; } = string.Empty;

    /// <summary>
    /// Obtém ou define a autoridade emissora do RG.
    /// </summary>
    /// <value>Uma string contendo a autoridade emissora.</value>
    public string AutoridadeEmissora { get; set; } = string.Empty;

    /// <summary>
    /// Obtém ou define a data de emissão do RG.
    /// </summary>
    /// <value>Uma data representando quando o RG foi emitido.</value>
    public DateTime? DataEmissaoRg { get; set; }

    /// <summary>
    /// Obtém ou define o nome fantasia da empresa, se aplicável.
    /// </summary>
    /// <value>Uma string representando o nome empresarial.</value>
    public string NomeFantasia { get; set; } = string.Empty;

    /// <summary>
    /// Obtém ou define o estado civil do cliente.
    /// </summary>
    /// <value>Um valor do enum <see cref="ETipoStatusCivil"/> representando o estado civil. O padrão é <see cref="ETipoStatusCivil.Solteiro"/>.</value>
    public ETipoStatusCivil TipoStatusCivil { get; set; } = ETipoStatusCivil.Solteiro;

    /// <summary>
    /// Indica se o cliente está ativo.
    /// </summary>
    /// <value><c>true</c> se o cliente estiver ativo; caso contrário, <c>false</c>.</value>
    public bool Ativo { get; set; }

    /// <summary>
    /// Obtém ou define o ID do usuário associado ao cliente.
    /// </summary>
    /// <value>Uma string representando o ID do usuário.</value>
    public string UsuarioId { get; set; } = string.Empty;

    /// <summary>
    /// Obtém ou define a lista de propriedades associadas ao cliente.
    /// </summary>
    /// <value>Uma lista de objetos <see cref="Imovel"/> representando as propriedades associadas.</value>
    public List<Imovel> Imoveis { get; set; } = [];
}