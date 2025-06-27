namespace ClassDiagramRealtyHub.Enums
{

    /// <summary>
    /// Enumerador para os estados civis dos clientes.
    /// </summary>
    /// <remarks>
    /// <para>O enum representa os diferentes estados civis que um cliente pode ter.</para>
    /// <para>Cada valor do enum é associado a um número inteiro e possui um atributo
    /// Display que fornece uma descrição legível para o estado civil.</para>
    /// </remarks>
    public enum EMaritalStatus
    {
        /// <summary>
        /// Estado civil solteiro(a).
        /// </summary>
        /// <remarks>
        /// Este valor é utilizado quando o cliente é solteiro(a).
        /// </remarks>
        /// <value>1</value>
        Single = 1,

        /// <summary>
        /// Estado civil casado(a).
        /// </summary>
        /// <remarks>
        /// Este valor é utilizado quando o cliente é casado(a).
        /// </remarks>
        /// <value>2</value>
        Married = 2,

        /// <summary>
        /// Estado civil divorciado(a).
        /// </summary>
        /// <remarks>
        /// Este valor é utilizado quando o cliente é divorciado(a).
        /// </remarks>
        /// <value>3</value>
        Divorced = 3,

        /// <summary>
        /// Estado civil viúvo(a).
        /// </summary>
        /// <remarks>
        /// Este valor é utilizado quando o cliente é viúvo(a).
        /// </remarks>
        /// <value>4</value>
        Widowed = 4,

        /// <summary>
        /// Estado civil noivo(a).
        /// </summary>
        /// <remarks>
        /// Este valor é utilizado quando o cliente é noivo(a).
        /// </remarks>
        /// <value>5</value>
        Engaged = 5
    }
}