using System.ComponentModel.DataAnnotations;

namespace RealtyHub.Core.Models;

/// <summary>
/// Representa um condomínio no sistema.
/// </summary>
/// <remarks>
/// Essa classe herda de <c><see cref="Entidade"/></c>
/// que contém propriedades comuns a todas as entidades do sistema.
/// </remarks>
public class Condominio : Entidade
{
    /// <summary>
    /// Obtém ou define o ID do condomínio.
    /// </summary>
    /// <value>Um valor inteiro representando o ID.</value>
    public long Id { get; set; }

    /// <summary>
    /// Obtém ou define o nome do condomínio.
    /// </summary>
    /// <value>Uma string contendo o nome do condomínio.</value>
    [Required(ErrorMessage = "Nome é um campo obrigatório")]
    [MaxLength(120, ErrorMessage = "O Nome deve conter até 120 caracteres")]
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Obtém ou define o endereço do condomínio.
    /// </summary>
    /// <value>Um objeto <see cref="Endereco"/> que representa o endereço.</value>
    [ValidateComplexType]
    public Endereco Endereco { get; set; } = new();

    /// <summary>
    /// Obtém ou define o número de unidades no condomínio.
    /// </summary>
    /// <value>Um valor inteiro representando o número de unidades.</value>
    public int Unidades { get; set; }

    /// <summary>
    /// Obtém ou define o número de andares do condomínio.
    /// </summary>
    /// <value>Um valor inteiro representando o número de andares.</value>
    public int Andares { get; set; }

    /// <summary>
    /// Indica se o condomínio possui elevador.
    /// </summary>
    /// <value><c>true</c> se possui elevador; caso contrário, <c>false</c>.</value>
    public bool PossuiElevador { get; set; }

    /// <summary>
    /// Indica se o condomínio possui piscina.
    /// </summary>
    /// <value><c>true</c> se possui piscina; caso contrário, <c>false</c>.</value>
    public bool PossuiPiscina { get; set; }

    /// <summary>
    /// Indica se o condomínio possui salão de festas.
    /// </summary>
    /// <value><c>true</c> se possui salão de festas; caso contrário, <c>false</c>.</value>
    public bool PossuiSalaoFesta { get; set; }

    /// <summary>
    /// Indica se o condomínio possui playground.
    /// </summary>
    /// <value><c>true</c> se possui playground; caso contrário, <c>false</c>.</value>
    public bool PossuiPlayground { get; set; }

    /// <summary>
    /// Indica se o condomínio possui academia.
    /// </summary>
    /// <value><c>true</c> se possui academia; caso contrário, <c>false</c>.</value>
    public bool PossuiAcademia { get; set; }

    /// <summary>
    /// Obtém ou define o valor do condomínio.
    /// </summary>
    /// <value>Um valor decimal representando o valor do condomínio.</value>
    public decimal TaxaCondominial { get; set; }

    /// <summary>
    /// Obtém ou define o ID do usuário associado ao condomínio.
    /// </summary>
    /// <value>Uma string representando o ID do usuário.</value>
    public string UsuarioId { get; set; } = string.Empty;

    /// <summary>
    /// Indica se o condomínio está ativo.
    /// </summary>
    /// <value><c>true</c> se está ativo; caso contrário, <c>false</c>.</value>
    public bool Ativo { get; set; }

    /// <summary>
    /// Obtém ou define a coleção de propriedades associadas ao condomínio.
    /// </summary>
    /// <value>Uma coleção de objetos <see cref="Property"/> que representam as propriedades associadas.</value>
    public ICollection<Imovel> Imoveis { get; set; } = [];
}