using Microsoft.AspNetCore.Components;
using MudBlazor;
using RealtyHub.Core.Models;
using RealtyHub.Web.Components.Home;

namespace RealtyHub.Web.Components;

public partial class CardHomeComponent : ComponentBase
{
    #region Parameters

    [Parameter]
    public string Class { get; set; } = string.Empty;

    [Parameter]
    public Property Property { get; set; } = new();

    #endregion

    #region Services

    [Inject] 
    public ISnackbar Snackbar { get; set; } = null!;

    [Inject]
    public IDialogService DialogService { get; set; } = null!;

    #endregion

    #region Methods

    public string GetSrcPhoto()
    {
        var photo = Property
                        .PropertyPhotos
                        .FirstOrDefault(p => p.IsThumbnail)
                    ?? Property.PropertyPhotos.FirstOrDefault();

        var fullName = $"{photo?.Id}{photo?.Extension}";

        return $"{Configuration.BackendUrl}/photos/{fullName}";
    }

    public async Task OnShowDetails()
    {
        var parameters = new DialogParameters { ["Id"] = Property.Id };
        var options = new DialogOptions
        {
            CloseOnEscapeKey = true,
            MaxWidth = MaxWidth.ExtraLarge,
            FullWidth = true
        };
        await DialogService.ShowAsync<ShowPropertyDetails>(null, parameters, options);
    }

    #endregion
}