using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using MudBlazor;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Condominiums;
using RealtyHub.Core.Responses;
using System.Text.RegularExpressions;

namespace RealtyHub.Web.Components.Condominiums;

public class CondominiumFormComponent : ComponentBase
{
    #region Parameters

    [Parameter]
    public long Id { get; set; }

    #endregion

    #region Properties

    public string Operation => Id != 0
        ? "Editar" : "Cadastrar";
    public bool IsBusy { get; set; }
    public Condominium InputModel { get; set; } = new();
    public string Pattern = @"\D";

    #endregion

    #region Services

    [Inject]
    public ISnackbar Snackbar { get; set; } = null!;

    [Inject]
    public NavigationManager NavigationManager { get; set; } = null!;

    [Inject]
    public ICondominiumHandler Handler { get; set; } = null!;

    #endregion

    #region Methods

    public async Task OnValidSubmitAsync()
    {
        IsBusy = true;
        try
        {
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
            NavigationManager.NavigateTo("/condominiums");
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

    protected override async Task OnInitializedAsync()
    {
        IsBusy = true;
        GetCondominiumByIdRequest? request = null;
        try
        {
            request = new GetCondominiumByIdRequest { Id = Id };
        }
        catch (Exception e)
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
            else
            {
                NavigationManager.NavigateTo("/condominiums/adicionar");
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