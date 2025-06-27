using System;
using System.Collections.Generic;
using ClassDiagramRealtyHub.Enums;

namespace ClassDiagramRealtyHub.Models
{
    /// <summary>
    /// Representa um cliente no sistema.
    /// </summary>
    /// <remarks>
    /// Esta classe herda de <c><see cref="Entity"/></c>, 
    /// que contém propriedades comuns a todas as entidades do sistema.
    /// </remarks>
    public class Customer : Entity
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
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Obtém ou define o e-mail do cliente.
        /// </summary>
        /// <value>Uma string representando o e-mail do cliente.</value>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Obtém ou define o telefone do cliente.
        /// </summary>
        /// <value>Uma string contendo o telefone do cliente.</value>
        public string Phone { get; set; } = string.Empty;

        /// <summary>
        /// Obtém ou define o número do documento do cliente.
        /// </summary>
        /// <value>Uma string representando o número do documento.</value>
        public string DocumentNumber { get; set; } = string.Empty;

        /// <summary>
        /// Obtém ou define a ocupação do cliente.
        /// </summary>
        /// <value>Uma string contendo a ocupação.</value>
        public string Occupation { get; set; } = string.Empty;

        /// <summary>
        /// Obtém ou define a nacionalidade do cliente.
        /// </summary>
        /// <value>Uma string representando a nacionalidade.</value>
        public string Nationality { get; set; } = string.Empty;

        /// <summary>
        /// Obtém ou define o tipo de pessoa do cliente.
        /// </summary>
        /// <value>Um valor do enum <see cref="EPersonType"/> indicando o tipo de pessoa.</value>
        public EPersonType PersonType { get; set; }

        /// <summary>
        /// Obtém ou define o tipo de cliente.
        /// </summary>
        /// <value>Um valor do enum <see cref="ECustomerType"/> indicando o tipo de cliente. O padrão é <see cref="ECustomerType.Buyer"/>.</value>
        public ECustomerType CustomerType { get; set; } = ECustomerType.Buyer;

        /// <summary>
        /// Obtém ou define o endereço do cliente.
        /// </summary>
        /// <value>Um objeto <see cref="Address"/> representando o endereço. A validação complexa é aplicada para validar suas propriedades.</value>
        public Address Address { get; set; } = new Address();

        /// <summary>
        /// Obtém ou define o RG do cliente.
        /// </summary>
        /// <value>Uma string representando o RG.</value>
        public string Rg { get; set; } = string.Empty;

        /// <summary>
        /// Obtém ou define a autoridade emissora do RG.
        /// </summary>
        /// <value>Uma string contendo a autoridade emissora.</value>
        public string IssuingAuthority { get; set; } = string.Empty;

        /// <summary>
        /// Obtém ou define a data de emissão do RG.
        /// </summary>
        /// <value>Uma data representando quando o RG foi emitido.</value>
        public DateTime? RgIssueDate { get; set; }

        /// <summary>
        /// Obtém ou define o nome empresarial, caso o cliente possua empresa associada.
        /// </summary>
        /// <value>Uma string representando o nome empresarial.</value>
        public string BusinessName { get; set; } = string.Empty;

        /// <summary>
        /// Obtém ou define o estado civil do cliente.
        /// </summary>
        /// <value>Um valor do enum <see cref="EMaritalStatus"/> representando o estado civil. O padrão é <see cref="EMaritalStatus.Single"/>.</value>
        public EMaritalStatus MaritalStatus { get; set; } = EMaritalStatus.Single;

        /// <summary>
        /// Indica se o cliente está ativo.
        /// </summary>
        /// <value><c>true</c> se o cliente estiver ativo; caso contrário, <c>false</c>.</value>
        public bool IsActive { get; set; }

        /// <summary>
        /// Obtém ou define o ID do usuário associado ao cliente.
        /// </summary>
        /// <value>Uma string representando o ID do usuário.</value>
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// Obtém ou define a lista de propriedades associadas ao cliente.
        /// </summary>
        /// <value>Uma lista de objetos <see cref="Property"/> representando as propriedades associadas.</value>
        public List<Property> Properties { get; set; } = new List<Property>();


        public bool Create() => true;

        public List<Customer> List() => new List<Customer>();

        public bool Update() => true;

        public bool Deactivate() => true;
    }
}