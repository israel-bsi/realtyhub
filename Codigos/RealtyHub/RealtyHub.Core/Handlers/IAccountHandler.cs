using RealtyHub.Core.Models.Account;
using RealtyHub.Core.Requests.Account;
using RealtyHub.Core.Responses;

namespace RealtyHub.Core.Handlers;
/// <summary>
/// Interface que define os métodos para manipulação de contas de usuário.
/// </summary>
/// <remarks>
/// Esta interface é responsável por definir as operações que podem ser realizadas
/// com as contas de usuário, como login, registro, confirmação de e-mail,
/// recuperação de senha, etc.
/// </remarks>
public interface IAccountHandler
{
    /// <summary>
    /// Realiza o login de um usuário no sistema.
    /// </summary>
    /// <remarks>
    /// Este método é responsável por autenticar um usuário com base nas credenciais fornecidas.
    /// </remarks>
    /// <param name="request">Instância de <see cref="LoginRequest"/> contendo as credenciais do usuário.</param>
    /// <returns>
    /// Retorna um objeto <see cref="Response{TData}"/> indicando o resultado da operação.
    /// O objeto TData é do tipo <see cref="string"/> e pode ser nulo se a operação falhar.
    /// </returns>
    Task<Response<string>> LoginAsync(LoginRequest request);

    /// <summary>
    /// Realiza o registro de um novo usuário no sistema.
    /// </summary>
    /// <remarks>
    /// Este método é responsável por criar uma nova conta de usuário com base nas informações fornecidas.
    /// </remarks>
    /// <param name="request"> Instância de <see cref="RegisterRequest"/> contendo as informações do novo usuário.</param>
    /// <returns>
    /// Retorna um objeto <see cref="Response{TData}"/> indicando o resultado da operação.
    /// O objeto TData é do tipo <see cref="string"/> e pode ser nulo se a operação falhar.
    /// </returns>
    Task<Response<string>> RegisterAsync(RegisterRequest request);

    /// <summary>
    /// Confirma o e-mail de um usuário após o registro.
    /// </summary>
    /// <remarks>
    /// Este método é responsável por validar o e-mail do usuário com base no token fornecido.
    /// </remarks>
    /// <param name="request">Instância de <see cref="ConfirmEmailRequest"/> contendo o token de confirmação e Id do usuário.</param>
    /// <returns>
    /// Retorna um objeto <see cref="Response{TData}"/> indicando o resultado da operação.
    /// O objeto TData é do tipo <see cref="string"/> e pode ser nulo se a operação falhar.
    /// </returns>
    Task<Response<string>> ConfirmEmailAsync(ConfirmEmailRequest request);

    /// <summary>
    /// Recupera a senha de um usuário.
    /// </summary>
    /// <remarks>
    /// Este método é responsável por enviar um e-mail de recuperação de senha para o usuário.
    /// </remarks>
    /// <param name="request">>Instância de <see cref="ForgotPasswordRequest"/> contendo o e-mail do usuário.</param>
    /// <returns>
    /// Retorna um objeto <see cref="Response{TData}"/> indicando o resultado da operação.
    /// O objeto TData é do tipo <see cref="string"/> e pode ser nulo se a operação falhar.
    /// </returns>
    Task<Response<string>> ForgotPasswordAsync(ForgotPasswordRequest request);

    /// <summary>
    /// Redefine a senha de um usuário.
    /// /summary>
    /// <remarks>
    /// Este método é responsável por atualizar a senha do usuário com base no token fornecido.
    /// </remarks>
    /// <param name="request">>Instância de <see cref="ResetPasswordRequest"/> contendo o token e a nova senha do usuário.</param>
    /// <returns>
    /// Retorna um objeto <see cref="Response{TData}"/> indicando o resultado da operação.
    /// O objeto TData é do tipo <see cref="string"/> e pode ser nulo se a operação falhar.
    /// </returns>
    Task<Response<string>> ResetPasswordAsync(ResetPasswordRequest request);

    /// <summary>
    /// Realiza o logout de um usuário do sistema.
    /// </summary>
    /// <remarks>
    /// Este método é responsável por encerrar a sessão do usuário no sistema.
    /// </remarks>
    Task LogoutAsync();
}