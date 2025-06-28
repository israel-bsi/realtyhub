namespace ClassDiagramRealtyHub.Enums
{
    /// <summary>
    /// Enumerador para os tipos de contrato.
    /// </summary>
    /// <remarks>
    /// <para>O enum representa os diferentes tipos de contrato que podem existir.</para>
    /// <para>Cada valor do enum é associado a um número inteiro e possui um atributo 
    /// Display que fornece uma descrição legível para o tipo de contrato.</para>
    /// </remarks>
    public enum ETipoContrato
    {

        /// <summary>
        /// Tipo de contrato não definido.
        /// </summary>
        /// <remarks>
        /// Este valor é utilizado quando o tipo de contrato não é especificado.
        /// </remarks>
        /// <value>0</value>
        Nenhum,

        /// <summary>
        /// Tipo de contrato entre pessoa jurídica e pessoa jurídica.
        /// </summary>
        /// <remarks>
        /// Este valor é utilizado quando o contrato é entre duas pessoas jurídicas.
        /// </remarks>
        /// <value>1</value>
        PJPJ = 1,

        /// <summary>
        /// Tipo de contrato entre pessoa física e pessoa física.
        /// </summary>
        /// <remarks>
        /// Este valor é utilizado quando o contrato é entre duas pessoas físicas.
        /// </remarks>
        /// <value>2</value>
        PFPF = 2,

        /// <summary>
        /// Tipo de contrato entre pessoa física e pessoa jurídica.
        /// </summary>
        /// <remarks>
        /// Este valor é utilizado quando o contrato é entre uma pessoa física e uma pessoa jurídica.
        /// </remarks>
        /// <value>3</value>
        PFPJ = 3,

        /// <summary>
        /// Tipo de contrato entre pessoa jurídica e pessoa física.
        /// </summary>
        /// <remarks>
        /// Este valor é utilizado quando o contrato é entre uma pessoa jurídica e uma pessoa física.
        /// </remarks>
        /// <value>4</value>
        PJPF = 4
    }
}