﻿namespace ClassDiagramRealtyHub.Enums
{

    /// <summary>
    /// Enumerador para os tipos de pessoa.
    /// </summary>
    /// <remarks>
    /// <para>O enum representa os diferentes tipos de pessoa que podem ser utilizados na aplicação.</para>
    /// <para>Cada valor do enum é associado a um número inteiro e possui um atributo 
    /// Display que fornece uma descrição legível para o tipo de pessoa.</para>
    /// </remarks>
    public enum EPersonType
    {
        /// <summary>
        /// Tipo de pessoa individual (Pessoa Física).
        /// </summary>
        /// <remarks>
        /// Este valor é utilizado quando o registro refere-se a uma pessoa física.
        /// </remarks>
        /// <value>1</value>
        Individual = 1,

        /// <summary>
        /// Tipo de pessoa jurídica (Pessoa Jurídica).
        /// </summary>
        /// <remarks>
        /// Este valor é utilizado quando o registro refere-se a uma pessoa jurídica.
        /// </remarks>
        /// <value>2</value>
        Business = 2
    }
}