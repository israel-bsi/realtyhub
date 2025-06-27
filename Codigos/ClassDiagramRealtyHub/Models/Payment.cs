using System.Globalization;
using System.Text;
using ClassDiagramRealtyHub.Enums;

namespace ClassDiagramRealtyHub.Models
{
    /// <summary>
    /// Representa um pagamento relacionado a uma proposta.
    /// </summary>
    /// <remarks>
    /// Esta classe herda de <c><see cref="Entity"/></c>, 
    /// que contém propriedades comuns a todas as entidades do sistema.
    /// </remarks>
    public class Payment : Entity
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
        public decimal Amount { get; set; }

        /// <summary>
        /// Obtém ou define o tipo de pagamento.
        /// </summary>
        /// <value>Um valor do enum <see cref="EPaymentType"/> representando a forma de pagamento.</value>
        public EPaymentType PaymentType { get; set; } = EPaymentType.Cash;

        /// <summary>
        /// Indica se o pagamento será realizado em parcelas.
        /// </summary>
        /// <value><c>true</c> se o pagamento for parcelado; caso contrário, <c>false</c>.</value>
        public bool Installments { get; set; }

        /// <summary>
        /// Obtém ou define o número de parcelas.
        /// </summary>
        /// <value>
        /// Um valor inteiro representando o número de parcelas, que deverá estar entre 1 e 24.
        /// </value>
        public int InstallmentsCount { get; set; } = 1;

        /// <summary>
        /// Obtém ou define o ID da proposta associada ao pagamento.
        /// </summary>
        /// <value>Um valor inteiro representando o ID da proposta.</value>
        public long OfferId { get; set; }

        /// <summary>
        /// Indica se o pagamento está ativo.
        /// </summary>
        /// <value><c>true</c> se o pagamento estiver ativo; caso contrário, <c>false</c>.</value>
        public bool IsActive { get; set; }

        /// <summary>
        /// Obtém ou define o ID do usuário associado ao pagamento.
        /// </summary>
        /// <value>Uma string representando o ID do usuário.</value>
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// Obtém ou define a proposta associada ao pagamento.
        /// </summary>
        /// <value>Um objeto <see cref="Offer"/> representando a proposta associada.</value>
        public Offer Offer { get; set; }
    }
}