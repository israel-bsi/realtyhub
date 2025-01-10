using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using RealtyHub.Core.Enums;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Requests.Customers;
using RealtyHub.Core.Services;

namespace RealtyHub.Web.Pages.Customers;

public partial class CreateCustomerPage : ComponentBase
{
    #region Properties

    public bool IsBusy { get; set; }
    public CreateCustomerRequest InputModel { get; set; } = new();

    public readonly PatternMask PhoneMask = new("(##) #####-####")
    {
        MaskChars = [new MaskChar('#', @"[0-9]")]
    };

    public readonly PatternMask ZipCodeMask = new("#####-###")
    {
        MaskChars = [new MaskChar('#', @"[0-9]")]
    };

    public readonly PatternMask CpfMask = new("###.###.###-##")
    {
        MaskChars = [new MaskChar('#', @"[0-9]")]
    };

    public readonly PatternMask CnpjMask = new("##.###.###/####-##")
    {
        MaskChars = [new MaskChar('#', @"[0-9]")]
    };

    public PatternMask DocumentCustomerMask => 
        InputModel.CustomerType == ECustomerType.Individual ? CpfMask : CnpjMask;
    public string DocumentType => 
        InputModel.CustomerType == ECustomerType.Individual ? "CPF" : "CNPJ";

    public string Pattern = @"\D";

    #endregion

    #region Services

    [Inject]
    public ICustomerHandler Handler { get; set; } = null!;

    [Inject] 
    public IViaCepService CepService { get; set; } = null!;

    [Inject]
    public NavigationManager NavigationManager { get; set; } = null!;

    [Inject]
    public ISnackbar Snackbar { get; set; } = null!;

    #endregion

    #region Methods

    public async Task OnValidSubmitAsync()
    {
        IsBusy = true;
        try
        {
            InputModel.Phone = Regex.Replace(InputModel.Phone, Pattern, "");
            InputModel.DocumentNumber = Regex.Replace(InputModel.DocumentNumber, Pattern, "");
            InputModel.Address.ZipCode = Regex.Replace(InputModel.Address.ZipCode, Pattern, "");
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
    public async Task SearchAddressAsync(FocusEventArgs focusEventArgs)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(InputModel.Address.ZipCode))
                return;

            var result = await CepService.SearchAddressAsync(InputModel.Address.ZipCode);
            if (result.Data is null || !result.IsSuccess) return;

            InputModel.Address.Street = result.Data.Street;
            InputModel.Address.Neighborhood = result.Data.Neighborhood;
            InputModel.Address.City = result.Data.City;
            InputModel.Address.State = result.Data.State;
            InputModel.Address.Complement = result.Data.Complement;
            StateHasChanged();
        }
        catch (Exception e)
        {
            Snackbar.Add(e.Message, Severity.Error);
        }
    }

    #endregion
}