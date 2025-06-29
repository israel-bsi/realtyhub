﻿namespace ClassDiagramRealtyHub.Enums
{


    /// <summary>
    /// Enumerador para os status das ofertas.
    /// </summary>
    /// <remarks>
    /// <para>O enum representa os diferentes status que uma oferta pode ter.</para>
    /// <para>Cada valor do enum é associado a um número inteiro e possui um atributo
    /// Display que fornece uma descrição legível para o status da oferta.</para>
    /// </remarks>
    public enum EOfferStatus
    {
        /// <summary>
        /// Status da oferta em análise.
        /// </summary>
        /// <remarks>
        /// Este valor é utilizado quando a oferta está em análise.
        /// </remarks>
        /// <value>1</value>
        Analysis = 1,

        /// <summary>
        /// Status da oferta aceita.
        /// </summary>
        /// <remarks>
        /// Este valor é utilizado quando a oferta foi aceita.
        /// </remarks>
        /// <value>2</value>
        Accepted = 2,

        /// <summary>
        /// Status da oferta rejeitada.
        /// </summary>
        /// <remarks>
        /// Este valor é utilizado quando a oferta foi rejeitada.
        /// </remarks>
        /// <value>3</value>
        Rejected = 3
    }
}