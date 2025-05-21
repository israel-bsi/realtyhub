using Microsoft.AspNetCore.Components.WebAssembly.Http;

namespace RealtyHub.Web.Security;

/// <summary>
/// Handler responsável por configurar as credenciais do navegador e adicionar cabeçalhos específicos
/// para requisições HTTP, garantindo que os cookies sejam incluídos e o atributo "X-Requested-With"
/// seja definido para "XMLHttpRequest".
/// </summary>
public class CookieHandler : DelegatingHandler
{
    /// <summary>
    /// Envia a requisição HTTP com as configurações de cookies e cabeçalhos adicionais.
    /// </summary>
    /// <param name="request">A requisição HTTP a ser enviada.</param>
    /// <param name="cancellationToken">Token para cancelamento assíncrono da operação.</param>
    /// <returns>Task que representa a operação assíncrona e contém a resposta HTTP.</returns>
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
        request.Headers.Add("X-Requested-With", "XMLHttpRequest");
        return base.SendAsync(request, cancellationToken);
    }
}