using Microsoft.AspNetCore.Components;
using MudBlazor;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Properties;

namespace RealtyHub.Web.Components.Home;

/// <summary>
/// Página responsável por exibir os detalhes de um imóvel na área pública do site.
/// </summary>
public partial class PropertyDetailsPage : ComponentBase
{
    #region Parameters

    /// <summary>
    /// Identificador do imóvel a ser exibido.
    /// </summary>
    [Parameter]
    public long Id { get; set; }

    #endregion

    #region Properties

    /// <summary>
    /// Modelo do imóvel exibido na página.
    /// </summary>
    public Property InputModel { get; set; } = new();

    /// <summary>
    /// Indica se a página está em estado de carregamento.
    /// </summary>
    public bool IsBusy { get; set; }

    /// <summary>
    /// Chave para forçar atualização do grid de fotos ou informações.
    /// </summary>
    public int GridKey { get; set; }

    #endregion

    #region Services

    /// <summary>
    /// Serviço para exibição de mensagens e notificações.
    /// </summary>
    [Inject]
    public ISnackbar Snackbar { get; set; } = null!;

    /// <summary>
    /// Handler responsável pelas operações de imóveis.
    /// </summary>
    [Inject]
    public IPropertyHandler PropertyHandler { get; set; } = null!;

    #endregion

    #region Overrides

    /// <summary>
    /// Inicializa o componente, buscando os detalhes do imóvel pelo ID informado.
    /// </summary>
    /// <remarks>
    /// Realiza a requisição ao handler para obter os dados do imóvel. Em caso de sucesso, preenche o modelo e atualiza a interface.
    /// Em caso de erro, exibe mensagem via Snackbar.
    /// </remarks>
    protected override async Task OnInitializedAsync()
    {
        IsBusy = true;
        try
        {
            var request = new GetPropertyByIdRequest { Id = Id };
            var result = await PropertyHandler.GetByIdAsync(request);
            if (result is { IsSuccess: true, Data: not null })
            {
                InputModel = result.Data;
                GridKey++;
            }
            else
            {
                Snackbar.Add(result.Message ?? string.Empty, Severity.Error);
            }
        }
        catch (Exception e)
        {
            Snackbar.Add(e.Message, Severity.Error);
        }
        finally
        {
            IsBusy = false;
        }
    }

    #endregion
}