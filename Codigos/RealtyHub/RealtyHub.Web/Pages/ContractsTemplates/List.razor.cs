using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;

namespace RealtyHub.Web.Pages.ContractsTemplates;

/// <summary>
/// Página responsável por exibir e gerenciar a listagem dos modelos de contrato.
/// </summary>
public partial class ListContractTemplatesPage : ComponentBase
{
    #region Properties

    /// <summary>
    /// Lista de modelos de contrato a serem exibidos na página.
    /// </summary>
    public List<ModeloContrato> ContractTemplates { get; set; } = [];

    #endregion

    #region Services

    /// <summary>
    /// Serviço para exibição de mensagens de notificação.
    /// </summary>
    [Inject]
    public ISnackbar Snackbar { get; set; } = null!;

    /// <summary>
    /// Handler responsável pelas operações relacionadas aos modelos de contrato.
    /// </summary>
    [Inject]
    public IContractTemplateHandler Handler { get; set; } = null!;

    /// <summary>
    /// Serviço JavaScript para interações com o navegador.
    /// </summary>
    [Inject]
    public IJSRuntime JsRuntime { get; set; } = null!;

    #endregion

    #region Methods

    /// <summary>
    /// Carrega os dados do servidor de forma paginada para exibição no grid.
    /// </summary>
    /// <param name="state">Estado atual do grid contendo informações de paginação.</param>
    /// <returns>
    /// Um objeto <see cref="GridData{ContractTemplate}"/> contendo os modelos de contrato filtrados e a contagem total.
    /// Caso a operação falhe, retorna um grid vazio e exibe uma mensagem de erro.
    /// </returns>
    public async Task<GridData<ModeloContrato>> LoadServerData(GridState<ModeloContrato> state)
    {
        try
        {
            var response = await Handler.GetAllAsync();
            if (response.IsSuccess)
            {
                ContractTemplates = response.Data ?? [];
                return new GridData<ModeloContrato>
                {
                    Items = ContractTemplates.Where(c => c.MostrarNaHome),
                    TotalItems = ContractTemplates.Count
                };
            }

            Snackbar.Add(response.Message ?? string.Empty, Severity.Error);
            return new GridData<ModeloContrato>();
        }
        catch (Exception e)
        {
            Snackbar.Add(e.Message, Severity.Error);
            return new GridData<ModeloContrato>();
        }
    }

    /// <summary>
    /// Abre o modelo de contrato em PDF em uma nova aba do navegador.
    /// </summary>
    /// <param name="template">O modelo de contrato a ser visualizado.</param>
    /// <returns>Task representando a operação assíncrona.</returns>
    public async Task ShowInNewPage(ModeloContrato template)
    {
        await JsRuntime.InvokeVoidAsync("openContractPdfInNewTab", template.Caminho);
    }

    #endregion
}