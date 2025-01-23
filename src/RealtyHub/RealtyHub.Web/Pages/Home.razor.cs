using Microsoft.AspNetCore.Components;
using MudBlazor;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Properties;

namespace RealtyHub.Web.Pages;

public partial class HomePage : ComponentBase
{
    #region Properties

    public List<Property> Properties { get; set; } = [];
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; } = 1;
    public int PageSize { get; set; } = 8;
    public bool IsBusy { get; set; }

    #endregion

    #region Services

    [Inject]
    public IPropertyHandler PropertyHandler { get; set; } = null!;

    [Inject]
    public ISnackbar Snackbar { get; set; } = null!;

    #endregion

    #region Override

    protected override async Task OnInitializedAsync()
    {
        IsBusy = true;
        try
        {
            await LoadDataAsync(CurrentPage);
        }
        catch
        {
            Snackbar.Add("Erro ao exibir os imóveis", Severity.Error);
        }
        finally
        {
            await Task.Delay(1000);
            IsBusy = false;
        }
    }

    #endregion

    #region Methods
    public async Task OnPageChanged(int newPage)
    {
        CurrentPage = newPage;
        await LoadDataAsync(newPage);
    }

    private async Task LoadDataAsync(int pageNumber)
    {
        var request = new GetAllPropertiesRequest
        {
            PageNumber = pageNumber,
            PageSize = PageSize
        };
        var response = await PropertyHandler.GetAllAsync(request);
        if (response is { IsSuccess: true, Data: not null })
        {
            Properties = response.Data.Where(p => p.ShowInHome).ToList();
            TotalPages = response.TotalPages;
            return;
        }

        Snackbar.Add("Não foi possível exibir os imóveis", Severity.Error);
    }

    #endregion
}