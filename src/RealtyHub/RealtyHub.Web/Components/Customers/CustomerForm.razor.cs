﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using RealtyHub.Core.Enums;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Requests.Customers;
using RealtyHub.Core.Services;
using RealtyHub.Web.Services;
using System.Text.RegularExpressions;
using RealtyHub.Core.Models;
using RealtyHub.Core.Responses;

namespace RealtyHub.Web.Components.Customers;

public partial class CustomerFormComponent : ComponentBase
{
    #region Parameters

    [Parameter]
    public long Id { get; set; }

    #endregion

    #region Properties

    public string Operation => Id != 0
        ? "Editar" : "Cadastrar";
    public bool IsBusy { get; set; }
    public Customer InputModel { get; set; } = new();

    public readonly PatternMask PhoneMask = new("(##) #########")
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
    public IViaCepService CepService { get; set; } = null!;

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
            InputModel.DocumentType = InputModel.CustomerType == ECustomerType.Individual
                ? EDocumentType.Cpf : EDocumentType.Cnpj;

            Response<Customer?> result;
            if (InputModel.Id > 0)
            {
                result = await Handler.UpdateAsync(InputModel);
            }
            else
            {
                result = await Handler.CreateAsync(InputModel);
            }

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
    public void ValidateDocument(FocusEventArgs focusEventArgs)
    {
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
    }
    public void OnCustomerTypeChanged(ECustomerType newCustomerType)
    {
        InputModel.CustomerType = newCustomerType;
        ChangeDocumentMask(newCustomerType);
        StateHasChanged();
    }
    private void ChangeDocumentMask(ECustomerType customerType)
    {
        DocumentCustomerMask = customerType switch
        {
            ECustomerType.Individual => CpfMask,
            ECustomerType.Business => CnpjMask,
            _ => DocumentCustomerMask
        };
    }

    #endregion

    #region Overrides

    protected override async Task OnInitializedAsync()
    {
        ChangeDocumentMask(InputModel.CustomerType);
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
                    InputModel.Id = response.Data.Id;
                    InputModel.Name = response.Data.Name;
                    InputModel.Email = response.Data.Email;
                    InputModel.Phone = response.Data.Phone;
                    InputModel.DocumentNumber = response.Data.DocumentNumber;
                    InputModel.DocumentType = response.Data.DocumentType;
                    InputModel.CustomerType = response.Data.CustomerType;
                    InputModel.Rg = response.Data.Rg;
                    InputModel.BusinessName = response.Data.BusinessName;
                    InputModel.Address.ZipCode = response.Data.Address.ZipCode;
                    InputModel.Address.Street = response.Data.Address.Street;
                    InputModel.Address.Neighborhood = response.Data.Address.Neighborhood;
                    InputModel.Address.Number = response.Data.Address.Number;
                    InputModel.Address.Complement = response.Data.Address.Complement;
                    InputModel.Address.City = response.Data.Address.City;
                    InputModel.Address.State = response.Data.Address.State;
                    InputModel.Address.Country = response.Data.Address.Country;
                }
                else
                {
                    Snackbar.Add(response.Message, Severity.Error);
                    NavigationManager.NavigateTo("/clientes");
                }
                InputModel.DocumentType = InputModel.CustomerType == ECustomerType.Individual
                    ? EDocumentType.Cpf : EDocumentType.Cnpj;
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
        else
        {
            NavigationManager.NavigateTo("/clientes/adicionar");
        }
    }

    #endregion
}