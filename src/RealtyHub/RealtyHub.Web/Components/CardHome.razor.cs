using Microsoft.AspNetCore.Components;
using MudBlazor;
using RealtyHub.Core.Models;

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

    #endregion
}