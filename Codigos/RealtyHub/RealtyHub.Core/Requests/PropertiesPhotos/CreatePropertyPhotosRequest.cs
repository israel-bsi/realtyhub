using Microsoft.AspNetCore.Http;
using RealtyHub.Core.Models;

namespace RealtyHub.Core.Requests.PropertiesPhotos;

/// <summary>
/// Representa uma requisição para criar fotos de um imóvel.
/// </summary>
/// <remarks>
/// A classe herda de <c><see cref="Request"/></c>, que contém propriedades comuns para requisições.
/// </remarks>
public class CreatePropertyPhotosRequest : Request
{
    /// <summary>
    /// Obtém ou define o identificador único do imóvel.
    /// </summary>
    /// <value>O Id do imóvel cujas fotos serão criadas.</value>
    public long PropertyId { get; set; }

    /// <summary>
    /// Obtém ou define a requisição HTTP associada.
    /// </summary>
    /// <value>Instância do <c><see cref="HttpRequest"/></c> que contém os detalhes da requisição HTTP, podendo ser nula.</value>
    public HttpRequest? HttpRequest { get; set; }

    /// <summary>
    /// Obtém ou define a lista de arquivos em formato de bytes.
    /// </summary>
    /// <value>Uma lista de <c><see cref="FileData"/></c> representando os arquivos enviados, ou nula se nenhum arquivo for enviado.</value>
    public List<FileData>? FileBytes { get; set; }
}