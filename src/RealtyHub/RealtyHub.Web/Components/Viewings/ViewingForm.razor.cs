using Microsoft.AspNetCore.Components;
using RealtyHub.Core.Models;

namespace RealtyHub.Web.Components.Viewings;

public partial class ViewingFormComponent : ComponentBase
{
    #region Parameters

    public long Id { get; set; }

    #endregion

    #region Properties

    public Viewing InputModel { get; set; } = new();

    #endregion

    #region Services



    #endregion

    #region Methods



    #endregion

    #region Overrides

    protected override Task OnInitializedAsync()
    {
        return base.OnInitializedAsync();
    }

    #endregion
}