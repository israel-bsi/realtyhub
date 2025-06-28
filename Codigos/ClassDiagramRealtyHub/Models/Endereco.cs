namespace ClassDiagramRealtyHub.Models
{
    /// <summary>
    /// Representa um endereço no sistema.
    /// </summary>
    public class Endereco
    {
        /// <summary>
        /// Logradouro do endereço.
        /// </summary>
        /// <value>O logradouro do endereço.</value>
        public string Logradouro { get; set; } = string.Empty;

        /// <summary>
        /// Bairro do endereço.
        /// </summary>
        /// <value>O bairro do endereço.</value>
        public string Bairro { get; set; } = string.Empty;

        /// <summary>
        /// Número do endereço.
        /// </summary>
        /// <value>O número do endereço.</value>
        public string Numero { get; set; } = string.Empty;

        /// <summary>
        /// Cidade do endereço.
        /// </summary>
        /// <value>O nome da cidade do endereço.</value>
        public string Cidade { get; set; } = string.Empty;

        /// <summary>
        /// Estado do endereço.
        /// </summary>
        /// <value>O nome do estado do endereço.</value>  
        public string Estado { get; set; } = string.Empty;

        /// <summary>
        /// País do endereço.
        /// </summary>
        /// <value>O nome do país do endereço.</value>
        public string Pais { get; set; } = string.Empty;

        /// <summary>
        /// Código postal (CEP) do endereço.
        /// </summary>
        /// <value>O código postal do endereço.</value>
        public string Cep { get; set; } = string.Empty;

        /// <summary>
        /// Complemento do endereço.
        /// </summary>
        /// <value>Informações adicionais sobre o endereço.</value>
        public string Complemento { get; set; } = string.Empty;
    }
}