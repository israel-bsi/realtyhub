using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using RealtyHub.Core.Enums;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Requests.Customers;
using RealtyHub.Web.Services;
using System.Text.RegularExpressions;
using RealtyHub.Core.Models;
using RealtyHub.Core.Responses;
using Microsoft.AspNetCore.Components.Forms;

namespace RealtyHub.Web.Components.Customers;

public partial class CustomerFormComponent : ComponentBase
{
    #region Parameters

    [Parameter]
    public long Id { get; set; }

    #endregion

    #region Properties
    public EditContext EditContext = null!;
    private ValidationMessageStore MessageStore = null!;
    public string Operation => Id != 0
        ? "Editar" : "Cadastrar";
    public bool IsBusy { get; set; }
    public Customer InputModel { get; set; } = new();
    public PatternMask DocumentCustomerMask { get; set; } = null!;
    public string DocumentType =>
        InputModel.CustomerType == ECustomerType.Individual ? "CPF" : "CNPJ";
    public string Pattern = @"\D";
    public string? ErrorText { get; set; }
    public bool Error => !string.IsNullOrEmpty(ErrorText);

    #endregion

    #region Services

    [Inject]
    public ICustomerHandler Handler { get; set; } = null!;


    [Inject]
    public NavigationManager NavigationManager { get; set; } = null!;

    [Inject]
    public ISnackbar Snackbar { get; set; } = null!;

    [Inject]
    public DocumentValidator DocumentValidator { get; set; } = null!;

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

            Response<Customer?> result;
            if (InputModel.Id > 0)
                result = await Handler.UpdateAsync(InputModel);
            else
                result = await Handler.CreateAsync(InputModel);

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
    public void OnBlurDocumentTextField(FocusEventArgs focusEventArgs)
        => ValidateDocument();
    public void ValidateDocument()
    {
        MessageStore.Clear();
        if (InputModel.CustomerType is ECustomerType.Individual)
        {
            ErrorText =
                !DocumentValidator.IsValidCpf(InputModel.DocumentNumber)
                    ? "CPF inválido" : null;
        }
        else
        {
            ErrorText =
                !DocumentValidator.IsValidCnpj(InputModel.DocumentNumber)
                    ? "CNPJ inválido" : null;
        }

        if (string.IsNullOrEmpty(ErrorText)) return;

        MessageStore.Add(() => InputModel.DocumentNumber, ErrorText);
        EditContext.NotifyValidationStateChanged();
    }
    public void OnCustomerTypeChanged(ECustomerType newCustomerType)
    {
        InputModel.CustomerType = newCustomerType;
        ChangeDocumentMask(newCustomerType);
        ValidateDocument();
    }
    private void ChangeDocumentMask(ECustomerType customerType)
    {
        DocumentCustomerMask = customerType switch
        {
            ECustomerType.Individual => Utility.Masks.Cnpj,
            ECustomerType.Business => Utility.Masks.Cnpj,
            _ => DocumentCustomerMask
        };
        StateHasChanged();
    }

    #endregion

    #region Overrides

    protected override async Task OnInitializedAsync()
    {
        EditContext = new EditContext(InputModel);
        MessageStore = new ValidationMessageStore(EditContext);
        if (Id != 0)
        {
            GetCustomerByIdRequest? request = null;

            try
            {
                request = new GetCustomerByIdRequest { Id = Id };
            }
            catch
            {
                Snackbar.Add("Parâmetro inválido", Severity.Error);
            }

            if (request is null) return;

            IsBusy = true;
            try
            {
                var response = await Handler.GetByIdAsync(request);
                if (response is { IsSuccess: true, Data: not null })
                {
                    InputModel = response.Data;
                }
                else
                {
                    Snackbar.Add(response.Message, Severity.Error);
                    NavigationManager.NavigateTo("/clientes");
                }
            }
            catch (Exception e)
            {
                Snackbar.Add(e.Message, Severity.Error);
            }
            finally
            {
                IsBusy = false;
                ChangeDocumentMask(InputModel.CustomerType);
                ValidateDocument();
            }
        }
        else
        {
            InputModel.CustomerType = ECustomerType.Individual;
            NavigationManager.NavigateTo("/clientes/adicionar");
        }
    }

    #endregion
}