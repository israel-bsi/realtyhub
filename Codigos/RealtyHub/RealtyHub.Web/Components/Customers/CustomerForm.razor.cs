﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using RealtyHub.Core.Enums;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Customers;
using RealtyHub.Core.Responses;
using RealtyHub.Web.Services;
using System.Text.RegularExpressions;

namespace RealtyHub.Web.Components.Customers;

public partial class CustomerFormComponent : ComponentBase
{
    #region Parameters

    [Parameter]
    public long Id { get; set; }

    #endregion

    #region Properties
    
    public EditContext EditContext = null!;
    private ValidationMessageStore? MessageStore;
    public string Operation => Id != 0
        ? "Editar" : "Cadastrar";
    public bool IsBusy { get; set; }
    public Customer InputModel { get; set; } = new();
    public PatternMask DocumentCustomerMask { get; set; } = null!;
    public string DocumentType =>
        InputModel.PersonType == EPersonType.Individual ? "CPF" : "CNPJ";
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
        if(IsIndividualPersonFieldsInvalid()) return;
        IsBusy = true;
        try
        {
            InputModel.Phone = Regex.Replace(InputModel.Phone, Pattern, "");
            InputModel.DocumentNumber = Regex.Replace(InputModel.DocumentNumber, Pattern, "");
            InputModel.Address.ZipCode = Regex.Replace(InputModel.Address.ZipCode, Pattern, "");
            InputModel.RgIssueDate = InputModel.RgIssueDate?.ToUniversalTime();

            Response<Customer?> result;
            if (InputModel.Id > 0)
                result = await Handler.UpdateAsync(InputModel);
            else
                result = await Handler.CreateAsync(InputModel);

            var resultMessage = result.Message ?? string.Empty;
            if (result.IsSuccess)
            {
                Snackbar.Add(resultMessage, Severity.Success);
                NavigationManager.NavigateTo("/clientes");
            }
            else
                Snackbar.Add(resultMessage, Severity.Error);
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
    public void OnBlurDocumentTextField()
        => ValidateDocument();
    public void ValidateDocument()
    {
        if (string.IsNullOrEmpty(InputModel.DocumentNumber)) return;
        MessageStore?.Clear();
        if (InputModel.PersonType is EPersonType.Individual)
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

        MessageStore?.Add(() => InputModel.DocumentNumber, ErrorText);
        EditContext.NotifyValidationStateChanged();
    }
    public void OnPersonTypeChanged(EPersonType newPersonType)
    {
        InputModel.PersonType = newPersonType;
        ChangeDocumentMask(newPersonType);
        ValidateDocument();
    }

    private bool IsIndividualPersonFieldsInvalid()
    {
        if (InputModel.PersonType is not EPersonType.Individual) return false;
        MessageStore?.Clear();
        var isInvalid = false;

        if (string.IsNullOrEmpty(InputModel.Nationality))
        {
            MessageStore?.Add(() => InputModel.Nationality, "Campo obrigatório");
            isInvalid = true;
        }

        if (string.IsNullOrEmpty(InputModel.Occupation))
        {
            MessageStore?.Add(() => InputModel.Occupation, "Campo obrigatório");
            isInvalid = true;
        }

        if (string.IsNullOrEmpty(InputModel.Rg))
        {
            MessageStore?.Add(() => InputModel.Rg, "Campo obrigatório");
            isInvalid = true;
        }

        if (string.IsNullOrEmpty(InputModel.IssuingAuthority))
        {
            MessageStore?.Add(() => InputModel.IssuingAuthority, "Campo obrigatório");
            isInvalid = true;
        }

        if (InputModel.RgIssueDate is null)
        {
            MessageStore?.Add(() => InputModel.RgIssueDate!, "Campo obrigatório");
            isInvalid = true;
        }

        EditContext.NotifyValidationStateChanged();
        MessageStore?.Clear();

        return isInvalid;
    }
    private void ChangeDocumentMask(EPersonType personType)
    {
        DocumentCustomerMask = personType switch
        {
            EPersonType.Individual => Utility.Masks.Cpf,
            EPersonType.Business => Utility.Masks.Cnpj,
            _ => DocumentCustomerMask
        };
        StateHasChanged();
    }
    private async Task LoadCustomerAsync()
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

        var response = await Handler.GetByIdAsync(request);
        if (response is { IsSuccess: true, Data: not null })
        {
            InputModel.Id = response.Data.Id;
            InputModel.Name = response.Data.Name;
            InputModel.Email = response.Data.Email;
            InputModel.Phone = response.Data.Phone;
            InputModel.DocumentNumber = response.Data.DocumentNumber;
            InputModel.PersonType = response.Data.PersonType;
            InputModel.CustomerType = response.Data.CustomerType;
            InputModel.Occupation = response.Data.Occupation;
            InputModel.Nationality = response.Data.Nationality;
            InputModel.MaritalStatus = response.Data.MaritalStatus;
            InputModel.Address.ZipCode = response.Data.Address.ZipCode;
            InputModel.Address.Street = response.Data.Address.Street;
            InputModel.Address.Number = response.Data.Address.Number;
            InputModel.Address.Complement = response.Data.Address.Complement;
            InputModel.Address.Neighborhood = response.Data.Address.Neighborhood;
            InputModel.Address.City = response.Data.Address.City;
            InputModel.Address.State = response.Data.Address.State;
            InputModel.Address.Country = response.Data.Address.Country;
            InputModel.Rg = response.Data.Rg;
            InputModel.IssuingAuthority = response.Data.IssuingAuthority;
            InputModel.RgIssueDate = response.Data.RgIssueDate;
            InputModel.BusinessName = response.Data.BusinessName;
            InputModel.IsActive = response.Data.IsActive;
            InputModel.UserId = response.Data.UserId;
        }
        else
        {
            Snackbar.Add(response.Message ?? string.Empty, Severity.Error);
            NavigationManager.NavigateTo("/clientes");
        }
        ValidateDocument();
    }
    private void RedirectToCreateCustomer()
    {
        InputModel.PersonType = EPersonType.Individual;
        InputModel.MaritalStatus = EMaritalStatus.Single;
        NavigationManager.NavigateTo("/clientes/adicionar");
    }
    private void ExecuteValidations()
    {
        EditContext = new EditContext(InputModel);
        MessageStore = new ValidationMessageStore(EditContext);
        ChangeDocumentMask(InputModel.PersonType);
    }

    #endregion

    #region Overrides

    protected override async Task OnInitializedAsync()
    {
        IsBusy = true;
        try
        {
            if (Id != 0)
                await LoadCustomerAsync();
            else
                RedirectToCreateCustomer();
        }
        catch (Exception e)
        {
            Snackbar.Add(e.Message, Severity.Error);
        }
        finally
        {
            ExecuteValidations();
            IsBusy = false;
        }
    }

    #endregion
}