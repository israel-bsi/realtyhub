using System.Collections.Generic;
using ClassDiagramRealtyHub.Enums;

namespace ClassDiagramRealtyHub.Models
{
    /// <summary>
    /// Representa um imóvel no sistema.
    /// </summary>
    /// <remarks>
    /// Esta classe herda de <c><see cref="Entidade"/></c>, 
    /// que contém propriedades comuns a todas as entidades do sistema.
    /// </remarks>
    public class Imovel : Entidade
    {
        /// <summary>
        /// Obtém ou define o ID do imóvel.
        /// </summary>
        /// <value>Um valor inteiro representando o ID do imóvel.</value>
        public long Id { get; set; }

        /// <summary>
        /// Obtém ou define o ID do vendedor associado ao imóvel.
        /// </summary>
        /// <value>Um valor inteiro representando o ID do vendedor.</value>
        public long VendedorId { get; set; }

        /// <summary>
        /// Obtém ou define o ID do condomínio associado ao imóvel.
        /// </summary>
        /// <value>Um valor inteiro representando o ID do condomínio.</value>
        public long CondominioId { get; set; }

        /// <summary>
        /// Obtém ou define o título do imóvel.
        /// </summary>
        /// <value>Uma string contendo o título do imóvel.</value>
        public string Titulo { get; set; } = string.Empty;

        /// <summary>
        /// Obtém ou define a descrição do imóvel.
        /// </summary>
        /// <value>Uma string contendo a descrição do imóvel.</value>
        public string Descricao { get; set; } = string.Empty;

        /// <summary>
        /// Obtém ou define o preço do imóvel.
        /// </summary>
        /// <value>Um valor decimal representando o preço do imóvel.</value>
        public decimal Preco { get; set; }

        /// <summary>
        /// Obtém ou define o tipo do imóvel.
        /// </summary>
        /// <value>Um valor do enum <see cref="ETipoImovel"/> representando o tipo do imóvel. O padrão é <see cref="ETipoImovel.Apartamento"/>.</value>
        public ETipoImovel TipoImovel { get; set; } = ETipoImovel.Apartamento;

        /// <summary>
        /// Obtém ou define a quantidade de quartos do imóvel.
        /// </summary>
        /// <value>Um valor inteiro representando o número de quartos.</value>
        public int Quarto { get; set; }

        /// <summary>
        /// Obtém ou define a quantidade de banheiros do imóvel.
        /// </summary>
        /// <value>Um valor inteiro representando o número de banheiros.</value>
        public int Banheiro { get; set; }

        /// <summary>
        /// Obtém ou define a quantidade de vagas de garagem do imóvel.
        /// </summary>
        /// <value>Um valor inteiro representando o número de vagas na garagem.</value>
        public int Garagem { get; set; }

        /// <summary>
        /// Obtém ou define a área (em metros quadrados) do imóvel.
        /// </summary>
        /// <value>Um valor double representando a área do imóvel.</value>
        public double Area { get; set; }

        /// <summary>
        /// Obtém ou define os detalhes da transação relativos ao imóvel.
        /// </summary>
        /// <value>Uma string contendo informações sobre a transação do imóvel.</value>
        public string DetalhesTransacao { get; set; } = string.Empty;

        /// <summary>
        /// Obtém ou define o endereço do imóvel.
        /// </summary>
        /// <value>Um objeto <see cref="Endereco"/> representando o endereço do imóvel.</value>
        public Endereco Endereco { get; set; }

        /// <summary>
        /// Indica se o imóvel é novo.
        /// </summary>
        /// <value><c>true</c> se o imóvel for novo; caso contrário, <c>false</c>.</value>
        public bool Novo { get; set; }

        /// <summary>
        /// Obtém ou define o número de matrícula no cartório.
        /// </summary>
        /// <value>Uma string representando o número de matrícula.</value>
        public string NumeroRegistro { get; set; } = string.Empty;

        /// <summary>
        /// Obtém ou define o registro do imóvel no cartório.
        /// </summary>
        /// <value>Uma string representando o registro do cartório.</value>
        public string RegistroCartorio { get; set; } = string.Empty;

        /// <summary>
        /// Indica se o imóvel está ativo.
        /// </summary>
        /// <value><c>true</c> se o imóvel estiver ativo; caso contrário, <c>false</c>.</value>
        public bool Ativo { get; set; }

        /// <summary>
        /// Indica se o imóvel deverá ser exibido na página inicial.
        /// </summary>
        /// <value><c>true</c> se o imóvel deve ser exibido na home; caso contrário, <c>false</c>.</value>
        public bool ExibirNaHome { get; set; }

        /// <summary>
        /// Obtém ou define o ID do usuário associado ao imóvel.
        /// </summary>
        /// <value>Uma string representando o ID do usuário.</value>
        public string UsuarioId { get; set; } = string.Empty;

        /// <summary>
        /// Obtém ou define a lista de fotos associadas ao imóvel.
        /// </summary>
        /// <value>Uma lista de objetos <see cref="FotoImovel"/> que representam as fotos do imóvel.</value>
        public List<FotoImovel> FotosImovel { get; set; }

        /// <summary>
        /// Obtém ou define o vendedor associado ao imóvel.
        /// </summary>
        /// <value>Um objeto <see cref="Cliente"/> representando o vendedor. Pode ser nulo.</value>
        public Cliente Vendedor { get; set; }

        /// <summary>
        /// Obtém ou define o condomínio associado ao imóvel.
        /// </summary>
        /// <value>Um objeto <see cref="Condominio"/> representando o condomínio ao qual o imóvel pertence.</value>
        public Condominio Condominio { get; set; }
    }
}