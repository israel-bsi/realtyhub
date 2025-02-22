using Microsoft.AspNetCore.Components;
using MudBlazor;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Properties;

namespace RealtyHub.Web.Components.Home;

public partial class PropertyDetailsPage : ComponentBase
{
    #region Parameters

    [Parameter]
    public long Id { get; set; }

    #endregion

    #region Properties

    public Property InputModel { get; set; } = new();
    public bool IsBusy { get; set; }
    public int GridKey { get; set; }

    #endregion

    #region Services

    [Inject]
    public ISnackbar Snackbar { get; set; } = null!;

    [Inject]
    public IPropertyHandler PropertyHandler { get; set; } = null!;

    #endregion

    #region Overrides

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