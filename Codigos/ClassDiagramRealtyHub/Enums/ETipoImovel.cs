namespace ClassDiagramRealtyHub.Enums
{
    /// <summary>
    /// Enumerador para os tipos de propriedade.
    /// </summary>
    /// <remarks>
    /// <para>O enum representa os diferentes tipos de propriedade disponíveis na aplicação.</para>
    /// <para>Cada valor do enum é associado a um número inteiro e possui um atributo 
    /// Display que fornece uma descrição legível para o tipo de propriedade.</para>
    /// </remarks>
    public enum ETipoImovel
    {
        /// <summary>
        /// Tipo de propriedade Casa.
        /// </summary>
        /// <remarks>
        /// Este valor é utilizado para designar uma residência do tipo casa.
        /// </remarks>
        /// <value>1</value>
        Casa = 1,

        /// <summary>
        /// Tipo de propriedade Apartamento.
        /// </summary>
        /// <remarks>
        /// Este valor é utilizado para designar uma residência do tipo apartamento.
        /// </remarks>
        /// <value>2</value>
        Apartamento = 2,

        /// <summary>
        /// Tipo de propriedade Ponto Comercial.
        /// </summary>
        /// <remarks>
        /// Este valor é utilizado para designar imóveis de uso comercial.
        /// </remarks>
        /// <value>3</value>
        Comercial = 3,

        /// <summary>
        /// Tipo de propriedade Terreno.
        /// </summary>
        /// <remarks>
        /// Este valor é utilizado para designar um terreno.
        /// </remarks>
        /// <value>4</value>
        Terreno = 4,

        /// <summary>
        /// Tipo de propriedade Kitnet.
        /// </summary>
        /// <remarks>
        /// Este valor é utilizado para designar um imóvel compacto, como uma kitnet.
        /// </remarks>
        /// <value>5</value>
        Kitnet = 5,

        /// <summary>
        /// Tipo de propriedade Fazenda.
        /// </summary>
        /// <remarks>
        /// Este valor é utilizado para designar uma propriedade rural, como uma fazenda.
        /// </remarks>
        /// <value>6</value>
        Fazenda = 6,

        /// <summary>
        /// Tipo de propriedade Outros.
        /// </summary>
        /// <remarks>
        /// Este valor é utilizado para designar outros tipos de propriedade que não se enquadram nas categorias anteriores.
        /// </remarks>
        /// <value>7</value>
        Outros = 7
    }
}