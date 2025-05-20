using Microsoft.AspNetCore.Components;
using MudBlazor;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Condominiums;
using RealtyHub.Core.Responses;
using System.Text.RegularExpressions;

namespace RealtyHub.Web.Components.Condominiums;

/// <summary>
/// Componente responsável pelo formulário de cadastro e edição de condomínios.
/// </summary>
public class CondominiumFormComponent : ComponentBase
{
    #region Parameters

    /// <summary>
    /// Identificador do condomínio. Se for diferente de zero, o formulário entra em modo de edição.
    /// </summary>
    [Parameter]
    public long Id { get; set; }

    #endregion

    #region Properties

    /// <summary>
    /// Indica a operação atual do formulário ("Editar" ou "Cadastrar").
    /// </summary>
    public string Operation => Id != 0
        ? "Editar" : "Cadastrar";

    /// <summary>
    /// Indica se o formulário está em estado de carregamento ou processamento.
    /// </summary>
    public bool IsBusy { get; set; }

    /// <summary>
    /// Modelo de entrada utilizado para o binding dos campos do formulário.
    /// </summary>
    public Condominium InputModel { get; set; } = new();

    /// <summary>
    /// Expressão regular utilizada para remover caracteres não numéricos do CEP.
    /// </summary>
    public string Pattern = @"\D";

    #endregion

    #region Services

    /// <summary>
    /// Serviço para exibição de mensagens e notificações.
    /// </summary>
    [Inject]
    public ISnackbar Snackbar { get; set; } = null!;

    /// <summary>
    /// Serviço de navegação para redirecionamento de páginas.
    /// </summary>
    [Inject]
    public NavigationManager NavigationManager { get; set; } = null!;

    /// <summary>
    /// Handler responsável pelas operações de condomínio.
    /// </summary>
    [Inject]
    public ICondominiumHandler Handler { get; set; } = null!;

    #endregion

    #region Methods

    /// <summary>
    /// Manipula o envio válido do formulário, realizando a criação ou atualização do condomínio.
    /// </summary>
    /// <returns>Task assíncrona representando a operação.</returns>
    public async Task OnValidSubmitAsync()
    {
        IsBusy = true;
        try
        {
            // Remove caracteres não numéricos do CEP
            InputModel.Address.ZipCode = Regex.Replace(InputModel.Address.ZipCode, Pattern, "");

            Response<Condominium?> result;
            if (InputModel.Id > 0)
                result = await Handler.UpdateAsync(InputModel);
            else
                result = await Handler.CreateAsync(InputModel);

            if (!result.IsSuccess)
            {
                Snackbar.Add(result.Message ?? string.Empty, Severity.Error);
                return;
            }

            Snackbar.Add("Condomínio cadastrado com sucesso", Severity.Success);
            NavigationManager.NavigateTo("/condominios");
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

    #region Overrides

    /// <summary>
    /// Inicializa o componente, buscando os dados do condomínio caso esteja em modo de edição.
    /// </summary>
    /// <returns>Task assíncrona representando a operação.</returns>
    protected override async Task OnInitializedAsync()
    {
        IsBusy = true;
        GetCondominiumByIdRequest? request = null;
        try
        {
            request = new GetCondominiumByIdRequest { Id = Id };
        }
        catch
        {
            Snackbar.Add("Parâmetro inválido", Severity.Error);
        }

        if (request is null) return;

        try
        {
            if (Id != 0)
            {
                var result = await Handler.GetByIdAsync(request);
                if (result is { IsSuccess: true, Data: not null })
                    InputModel = result.Data;
                else
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