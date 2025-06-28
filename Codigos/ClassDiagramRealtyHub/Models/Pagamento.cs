using System.Globalization;
using System.Text;
using ClassDiagramRealtyHub.Enums;

namespace ClassDiagramRealtyHub.Models
{
    /// <summary>
    /// Representa um pagamento relacionado a uma proposta.
    /// </summary>
    /// <remarks>
    /// Esta classe herda de <c><see cref="Entidade"/></c>, 
    /// que contém propriedades comuns a todas as entidades do sistema.
    /// </remarks>
    public class Pagamento : Entidade
    {
        /// <summary>
        /// Obtém ou define o ID do pagamento.
        /// </summary>
        /// <value>Um valor inteiro representando o ID do pagamento.</value>
        public long Id { get; set; }

        /// <summary>
        /// Obtém ou define o valor do pagamento.
        /// </summary>
        /// <value>Um valor decimal representando o montante do pagamento.</value>
        public decimal Valor { get; set; }

        /// <summary>
        /// Obtém ou define o tipo de pagamento.
        /// </summary>
        /// <value>Um valor do enum <see cref="ETipoPagamento"/> representando a forma de pagamento.</value>
        public ETipoPagamento TipoPagamento { get; set; } = ETipoPagamento.Dinheiro;

        /// <summary>
        /// Indica se o pagamento será realizado em parcelas.
        /// </summary>
        /// <value><c>true</c> se o pagamento for parcelado; caso contrário, <c>false</c>.</value>
        public bool Parcelado { get; set; }

        /// <summary>
        /// Obtém ou define o número de parcelas.
        /// </summary>
        /// <value>
        /// Um valor inteiro representando o número de parcelas, que deverá estar entre 1 e 24.
        /// </value>
        public int QuantidadeParcelas { get; set; } = 1;

        /// <summary>
        /// Obtém ou define o ID da proposta associada ao pagamento.
        /// </summary>
        /// <value>Um valor inteiro representando o ID da proposta.</value>
        public long PropostaId { get; set; }

        /// <summary>
        /// Indica se o pagamento está ativo.
        /// </summary>
        /// <value><c>true</c> se o pagamento estiver ativo; caso contrário, <c>false</c>.</value>
        public bool Ativo { get; set; }

        /// <summary>
        /// Obtém ou define o ID do usuário associado ao pagamento.
        /// </summary>
        /// <value>Uma string representando o ID do usuário.</value>
        public string UsuarioId { get; set; } = string.Empty;

        /// <summary>
        /// Obtém ou define a proposta associada ao pagamento.
        /// </summary>
        /// <value>Um objeto <see cref="Proposta"/> representando a proposta associada.</value>
        public Proposta Proposta { get; set; }
    }
}