using Microsoft.AspNetCore.Components;
using MudBlazor;
using RealtyHub.Core.Models;
using RealtyHub.Web.Components.Offers;

namespace RealtyHub.Web.Components.Home;

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

    public async Task OnSendOfferClickedAsync()
    {
        var options = new DialogOptions
        {
            CloseButton = true,
            MaxWidth = MaxWidth.Medium,
            CloseOnEscapeKey = true,
            FullWidth = true
        };

        var parameters = new DialogParameters
        {
            { "PropertyId", Property.Id }
        };

        await DialogService.ShowAsync<OfferDialogCreate>("Enviar proposta", parameters, options);
    }

    #endregion
}