﻿namespace ClassDiagramRealtyHub.Enums
{

    /// <summary>
    /// Enumerador para os tipos de pagamento.
    /// </summary>
    /// <remarks>
    /// <para>O enum representa os diferentes tipos de pagamento que podem ser utilizados.</para>
    /// <para>Cada valor do enum é associado a um número inteiro e possui um atributo
    /// Display que fornece uma descrição legível para o tipo de pagamento.</para>
    /// </remarks>
    public enum EPaymentType
    {
        /// <summary>
        /// Tipo de pagamento boleto
        /// </summary>
        /// <remarks>
        /// Este valor é utilizado quando o pagamento é feito por meio de boleto bancário.
        /// </remarks>
        /// <value>1</value>
        BankSlip = 1,

        /// <summary>
        /// Tipo de pagamento transferência bancária
        /// </summary>
        /// <remarks>
        /// Este valor é utilizado quando o pagamento é feito por meio de transferência bancária.
        /// </remarks>
        /// <value>2</value>
        BankTranfer = 2,

        /// <summary>
        /// Tipo de pagamento cheque
        /// </summary>
        /// <remarks>
        /// Este valor é utilizado quando o pagamento é feito por meio de cheque.
        /// </remarks>
        /// <value>3</value>
        Check = 3,

        /// <summary>
        /// Tipo de pagamento em dinheiro.
        /// </summary>
        /// <remarks>
        /// Este valor é utilizado quando o pagamento é feito em espécie.
        /// </remarks>
        /// <value>4</value>
        Cash = 4,

        /// <summary>
        /// Tipo de pagamento Pix.
        /// </summary>
        /// <remarks>
        /// Este valor é utilizado quando o pagamento é feito via Pix.
        /// </remarks>
        /// <value>5</value>
        Pix = 5,

        /// <summary>
        /// Tipo de pagamento financiamento.
        /// </summary>
        /// <remarks>
        /// Este valor é utilizado quando o pagamento é feito por meio de financiamento.
        /// </remarks>
        /// <value>6</value>
        Financing = 6,

        /// <summary>
        /// Tipo de pagamento cartão de crédito.
        /// </summary>
        /// <remarks>
        /// Este valor é utilizado quando o pagamento é feito por meio de cartão de crédito.
        /// </remarks>
        /// <value>7</value>
        CreditCard = 7,

        /// <summary>
        /// Tipo de pagamento via FGTS.
        /// </summary>
        /// <remarks>
        /// Este valor é utilizado quando o pagamento é feito utilizando os recursos do FGTS.
        /// </remarks>
        /// <value>8</value>
        Fgts = 8,

        /// <summary>
        /// Tipo de pagamento outros.
        /// </summary>
        /// <remarks>
        /// Este valor é utilizado para pagamentos que não se enquadram nos tipos anteriores.
        /// </remarks>
        /// <value>9</value>
        Others = 9
    }
}