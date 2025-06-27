using System.Collections.Generic;

namespace ClassDiagramRealtyHub.Models
{
    /// <summary>
    /// Representa um condomínio no sistema.
    /// </summary>
    /// <remarks>
    /// Essa classe herda de <c><see cref="Entity"/></c>
    /// que contém propriedades comuns a todas as entidades do sistema.
    /// </remarks>
    public class Condominium : Entity
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
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Obtém ou define o endereço do condomínio.
        /// </summary>
        /// <value>Um objeto <see cref="Address"/> que representa o endereço.</value>
        public Address Address { get; set; } = new Address();

        /// <summary>
        /// Obtém ou define o número de unidades no condomínio.
        /// </summary>
        /// <value>Um valor inteiro representando o número de unidades.</value>
        public int Units { get; set; }

        /// <summary>
        /// Obtém ou define o número de andares do condomínio.
        /// </summary>
        /// <value>Um valor inteiro representando o número de andares.</value>
        public int Floors { get; set; }

        /// <summary>
        /// Indica se o condomínio possui elevador.
        /// </summary>
        /// <value><c>true</c> se possui elevador; caso contrário, <c>false</c>.</value>
        public bool HasElevator { get; set; }

        /// <summary>
        /// Indica se o condomínio possui piscina.
        /// </summary>
        /// <value><c>true</c> se possui piscina; caso contrário, <c>false</c>.</value>
        public bool HasSwimmingPool { get; set; }

        /// <summary>
        /// Indica se o condomínio possui salão de festas.
        /// </summary>
        /// <value><c>true</c> se possui salão de festas; caso contrário, <c>false</c>.</value>
        public bool HasPartyRoom { get; set; }

        /// <summary>
        /// Indica se o condomínio possui playground.
        /// </summary>
        /// <value><c>true</c> se possui playground; caso contrário, <c>false</c>.</value>
        public bool HasPlayground { get; set; }

        /// <summary>
        /// Indica se o condomínio possui academia.
        /// </summary>
        /// <value><c>true</c> se possui academia; caso contrário, <c>false</c>.</value>
        public bool HasFitnessRoom { get; set; }

        /// <summary>
        /// Obtém ou define o valor do condomínio.
        /// </summary>
        /// <value>Um valor decimal representando o valor do condomínio.</value>
        public decimal CondominiumValue { get; set; }

        /// <summary>
        /// Obtém ou define o ID do usuário associado ao condomínio.
        /// </summary>
        /// <value>Uma string representando o ID do usuário.</value>
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// Indica se o condomínio está ativo.
        /// </summary>
        /// <value><c>true</c> se está ativo; caso contrário, <c>false</c>.</value>
        public bool IsActive { get; set; }

        /// <summary>
        /// Obtém ou define a coleção de propriedades associadas ao condomínio.
        /// </summary>
        /// <value>Uma coleção de objetos <see cref="Property"/> que representam as propriedades associadas.</value>
        public ICollection<Property> Properties { get; set; } = new List<Property>();

        public bool Create() => true;

        public List<Condominium> List() => new List<Condominium>();

        public bool Update() => true;

        public bool Deactivate() => true;
    }
}