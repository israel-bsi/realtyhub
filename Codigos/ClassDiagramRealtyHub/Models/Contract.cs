using System;
using System.Collections.Generic;

namespace ClassDiagramRealtyHub.Models
{
    /// <summary>
    /// Representa um contrato no sistema.
    /// </summary>
    /// <remarks>
    /// Essa classe herda de <c><see cref="Entity"/></c>,
    /// que contém propriedades comuns a todas as entidades do sistema.
    /// </remarks>
    public class Contract : Entity
    {
        /// <summary>
        /// Obtém ou define o ID do contrato.
        /// </summary>
        /// <value>Um valor inteiro representando o ID do contrato.</value>
        public long Id { get; set; }

        /// <summary>
        /// Obtém ou define o ID do vendedor associado ao contrato.
        /// </summary>
        /// <value>Um valor inteiro representando o ID do vendedor.</value>
        public long SellerId { get; set; }

        /// <summary>
        /// Obtém ou define o ID do comprador associado ao contrato.
        /// </summary>
        /// <value>Um valor inteiro representando o ID do comprador.</value>
        public long BuyerId { get; set; }

        /// <summary>
        /// Obtém ou define o ID da oferta associada ao contrato.
        /// </summary>
        /// <value>Um valor inteiro representando o ID da oferta.</value>
        public long OfferId { get; set; }

        /// <summary>
        /// Obtém ou define a data de emissão do contrato.
        /// </summary>
        /// <value>Uma data representando quando o contrato foi emitido.</value>
        public DateTime? IssueDate { get; set; }

        /// <summary>
        /// Obtém ou define a data de vigência do contrato.
        /// </summary>
        /// <value>Uma data representando quando o contrato entra em vigor.</value>
        public DateTime? EffectiveDate { get; set; }

        /// <summary>
        /// Obtém ou define a data de término do contrato.
        /// </summary>
        /// <value>Uma data representando quando o contrato termina.</value>
        public DateTime? TermEndDate { get; set; }

        /// <summary>
        /// Obtém ou define a data de assinatura do contrato.
        /// </summary>
        /// <value>Uma data representando quando o contrato foi assinado.</value>
        public DateTime? SignatureDate { get; set; }

        /// <summary>
        /// Obtém ou define o ID do arquivo associado ao contrato.
        /// </summary>
        /// <value>Uma string representando o ID do arquivo.</value>
        public string FileId { get; set; } = string.Empty;

        /// <summary>
        /// Indica se o contrato está ativo.
        /// </summary>
        /// <value><c>true</c> se o contrato estiver ativo; caso contrário, <c>false</c>.</value>
        public bool IsActive { get; set; }

        /// <summary>
        /// Obtém ou define o ID do usuário associado ao contrato.
        /// </summary>
        /// <value>Uma string representando o ID do usuário.</value>
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// Obtém ou define a oferta associada ao contrato.
        /// </summary>
        /// <value>Um objeto <see cref="Offer"/> representando a oferta. Pode ser nulo.</value>
        public Offer Offer { get; set; }

        /// <summary>
        /// Obtém ou define o cliente vendedor associado ao contrato.
        /// </summary>
        /// <value>Um objeto <see cref="Customer"/> representando o vendedor. Pode ser nulo.</value>
        public Customer Seller { get; set; }

        /// <summary>
        /// Obtém ou define o cliente comprador associado ao contrato.
        /// </summary>
        /// <value>Um objeto <see cref="Customer"/> representando o comprador. Pode ser nulo.</value>
        public Customer Buyer { get; set; }

        /// <summary>
        /// Obtém o caminho completo para o arquivo PDF do contrato.
        /// </summary>
        /// <value>Uma string representando a URL completa onde o PDF do contrato está disponível.</value>
        public string FilePath =>
            $"{Configuration.BackendUrl}/contracts/{FileId}.pdf";

        public bool Create() => true;

        public List<Contract> List() => new List<Contract>();

        public bool Update() => true;

        public bool Deactivate() => true;
    }
}