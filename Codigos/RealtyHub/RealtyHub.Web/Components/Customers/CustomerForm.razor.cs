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

/// <summary>
/// Componente responsável pelo formulário de cadastro e edição de clientes.
/// </summary>
public partial class CustomerFormComponent : ComponentBase
{
    #region Parameters

    /// <summary>
    /// Parâmetro que representa o ID do cliente a ser editado.
    /// </summary>
    [Parameter]
    public long Id { get; set; }

    #endregion

    #region Properties

    /// <summary>
    /// Contexto de edição do formulário.
    /// </summary>
    public EditContext EditContext = null!;

    /// <summary>
    /// Armazena mensagens de validação do formulário.
    /// </summary>
    private ValidationMessageStore? MessageStore;

    /// <summary>
    /// Texto do botão de ação do formulário.
    /// </summary>
    public string Operation => Id != 0
        ? "Editar" : "Cadastrar";

    /// <summary>
    /// Indica se o formulário está em estado de carregamento ou processamento.
    /// </summary>
    public bool IsBusy { get; set; }

    /// <summary>
    /// Instancia de <c><see cref="Customer"/></c> que representa o modelo de entrada do formulário.
    /// </summary>
    public Customer InputModel { get; set; } = new();

    /// <summary>
    /// Máscara de entrada para o campo de documento do cliente.
    /// </summary>
    /// <remarks>
    /// A máscara é definida com base no tipo de pessoa (física ou jurídica).
    /// </remarks>
    public PatternMask DocumentCustomerMask { get; set; } = null!;

    /// <summary>
    /// Tipo de documento do cliente, que pode ser CPF ou CNPJ.
    /// </summary>
    public string DocumentType =>
        InputModel.PersonType == EPersonType.Individual ? "CPF" : "CNPJ";

    /// <summary>
    /// Expressão regular utilizada para validar campos de texto.
    /// </summary>
    public string Pattern = @"\D";

    /// <summary>
    /// Texto de erro a ser exibido quando o documento do cliente é inválido.
    /// </summary>
    public string? ErrorText { get; set; }

    /// <summary>
    /// Indica se há erro de validação no campo de documento.
    /// </summary>
    public bool Error => !string.IsNullOrEmpty(ErrorText);

    #endregion

    #region Services

    /// <summary>
    /// Handler responsável por gerenciar operações relacionadas a clientes.
    /// </summary>
    [Inject]
    public ICustomerHandler Handler { get; set; } = null!;

    /// <summary>
    /// Serviço de navegação para redirecionamento de páginas.
    /// </summary>
    [Inject]
    public NavigationManager NavigationManager { get; set; } = null!;

    /// <summary>
    /// Serviço para exibição de mensagens e notificações.
    /// </summary>
    [Inject]
    public ISnackbar Snackbar { get; set; } = null!;

    /// <summary>
    /// Serviço para validação de documentos (CPF/CNPJ).
    /// </summary>
    [Inject]
    public DocumentValidator DocumentValidator { get; set; } = null!;

    #endregion

    #region Methods

    /// <summary>
    /// Método chamado quando o formulário é enviado e os dados são válidos.
    /// </summary>
    /// <remarks>
    /// Esse método valida os campos do formulário, formata os dados de entrada e chama o handler para criar ou atualizar o cliente.
    /// Em caso de sucesso, redireciona para a lista de clientes.
    /// Caso contrário, exibe uma mensagem de erro.
    /// </remarks>
    /// <returns>Uma tarefa assíncrona.</returns>
    public async Task OnValidSubmitAsync()
    {
        if (IsIndividualPersonFieldsInvalid()) return;
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

    /// <summary>
    /// Método chamado quando o campo de documento perde o foco.
    /// </summary>
    /// <remarks>
    /// Esse método valida o documento do cliente e exibe mensagens de erro, se necessário.
    /// </remarks>
    public void OnBlurDocumentTextField()
        => ValidateDocument();

    /// <summary>
    /// Método chamado quando o campo de documento é alterado.
    /// </summary>
    /// <remarks>
    /// Esse método valida o documento do cliente e exibe mensagens de erro, se necessário.
    /// </remarks>
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

    /// <summary>
    /// Método chamado quando o tipo de pessoa é alterado.
    /// </summary>
    /// <remarks>
    /// Esse método altera a máscara do documento e valida o documento do cliente.
    /// </remarks>
    public void OnPersonTypeChanged(EPersonType newPersonType)
    {
        InputModel.PersonType = newPersonType;
        ChangeDocumentMask(newPersonType);
        ValidateDocument();
    }

    /// <summary>
    /// Valida os campos obrigatórios para pessoas físicas.
    /// </summary>
    /// <remarks>
    /// Esse método verifica se os campos obrigatórios estão preenchidos e exibe mensagens de erro, se necessário.
    /// </remarks>
    /// <returns><c>true</c> caso os campos sejam inválidos, caso contrario retorna <c>false</c> .</returns>
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

    /// <summary>
    /// Altera a máscara do documento com base no tipo de pessoa.
    /// </summary>
    /// <param name="personType"> Enumerador <see cref="EPersonType"/> que representa o tipo de pessoa.</param>
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

    /// <summary>
    /// Carrega os dados do cliente a partir do ID fornecido.
    /// </summary>
    /// <remarks>
    /// Esse método faz uma requisição para obter os dados do cliente e preenche o modelo de entrada.
    /// Caso ocorra algum erro, exibe uma mensagem de erro e redireciona para a lista de clientes.
    /// </remarks>
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

    /// <summary>
    /// Redireciona para a página de criação de cliente.
    /// </summary>
    private void RedirectToCreateCustomer()
    {
        InputModel.PersonType = EPersonType.Individual;
        InputModel.MaritalStatus = EMaritalStatus.Single;
        NavigationManager.NavigateTo("/clientes/adicionar");
    }

    /// <summary>
    /// Executa as validações iniciais do formulário.
    /// </summary>
    private void ExecuteValidations()
    {
        EditContext = new EditContext(InputModel);
        MessageStore = new ValidationMessageStore(EditContext);
        ChangeDocumentMask(InputModel.PersonType);
    }

    #endregion

    #region Overrides

    /// <summary>
    /// Método chamado quando o componente é inicializado.
    /// </summary>
    /// <remarks>
    /// Esse método inicializa o contexto de edição, executa as validações iniciais e carrega os dados do cliente, se necessário.
    /// Caso ocorra algum erro, exibe uma mensagem de erro.
    /// </remarks>
    /// <returns>Uma tarefa assíncrona.</returns>
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