using Microsoft.AspNetCore.Components;
using MudBlazor;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Requests.Customers;

namespace RealtyHub.Web.Pages.Customer;

public partial class CreateCustomerPage : ComponentBase
{
    #region Properties

    public bool IsBusy { get; set; }
    public CreateCustomerRequest InputModel { get; set; } = new();

    public PatternMask PhoneMask = new("(##) #####-####")
    {
        MaskChars = [new MaskChar('#', @"[0-9]")]
    };

    public PatternMask ZipCodeMask = new("#####-###")
    {
        MaskChars = [new MaskChar('#', @"[0-9]")]
    };

    #endregion

    #region Services

    [Inject]
    public ICustomerHandler Handler { get; set; } = null!;

    [Inject]
    public NavigationManager NavigationManager { get; set; } = null!;

    [Inject]
    public ISnackbar Snackbar { get; set; } = null!;

    #endregion

    #region Methods

    public async Task OnValidSubmit()
    {
        IsBusy = true;
        try
        {
            var result = await Handler.CreateAsync(InputModel);
            if (result.IsSuccess)
            {
                Snackbar.Add(result.Message, Severity.Success);
                NavigationManager.NavigateTo("/clientes");
            }
            else
                Snackbar.Add(result.Message, Severity.Error);
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