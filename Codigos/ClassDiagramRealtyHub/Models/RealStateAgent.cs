using System.Collections.Generic;

namespace ClassDiagramRealtyHub.Models
{
    /// <summary>
    /// Representa um usuário no sistema.
    /// </summary>
    public class RealStateAgent
    {
        /// <summary>
        /// E-mail do usuário.
        /// </summary>
        /// <value>O endereço de e-mail do usuário.</value>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Nome do usuário.
        /// </summary>
        /// <value>O nome completo do usuário.</value>
        public string GivenName { get; set; } = string.Empty;

        /// <summary>
        /// Número de registro profissional (Creci) do usuário.
        /// </summary>
        /// <value>O número de registro profissional do usuário.</value>
        public string Creci { get; set; } = string.Empty;

        /// <summary>
        /// Dicionário de claims do usuário.
        /// </summary>
        /// <remarks>
        /// As claims são pares chave-valor que representam informações adicionais
        /// sobre o usuário, como permissões e roles.
        /// </remarks>
        /// <value>
        /// Um dicionário onde a chave é o tipo da claim e o valor é o valor da claim.
        /// </value>
        public Dictionary<string, string> Claims { get; set; } = new Dictionary<string, string>();

        public bool Register(string email, string password, string givenName, string creci)
        {
            return true;
        }

        public bool Login(string email, string password)
        {
            return true;
        }
    }
}