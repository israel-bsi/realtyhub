namespace ClassDiagramRealtyHub.Enums
{

    /// <summary>
    /// Enumerador para os tipos de cliente.
    /// </summary>
    /// <remarks>
    /// <para>O enum representa os diferentes tipos de cliente que podem existir.</para>
    /// <para>Cada valor do enum é associado a um número inteiro e possui um atributo
    /// Display que fornece uma descrição legível para o tipo de cliente.</para>
    /// </remarks>
    public enum ECustomerType
    {
        /// <summary>
        /// Tipo de cliente vendedor
        /// </summary>
        /// <remarks>
        /// Este valor é utilizado quando o cliente é um vendedor, ou seja, está vendendo um imóvel.
        /// </remarks>
        /// <value>1</value>
        Seller = 1,

        /// <summary>
        /// Tipo de cliente comprador
        /// </summary>
        /// <remarks>
        /// Este valor é utilizado quando o cliente é um comprador, ou seja, está comprando um imóvel.
        /// </remarks>
        /// <value>2</value>
        Buyer = 2,

        /// <summary>
        /// Tipo de cliente vendedor e comprador
        /// </summary>
        /// <remarks>
        /// Este valor é utilizado quando o cliente é tanto vendedor quanto comprador, ou seja, está vendendo e comprando um imóvel.
        /// </remarks>
        /// <value>3</value>
        BuyerSeller = 3
    }
}