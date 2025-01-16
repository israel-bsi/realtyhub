using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using RealtyHub.Core.Enums;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Properties;
using RealtyHub.Core.Requests.PropertiesImages;
using RealtyHub.Core.Responses;

namespace RealtyHub.Web.Components.Properties;

public partial class PropertyFormComponent : ComponentBase
{
    #region Parameters

    [Parameter]
    public long Id { get; set; }

    #endregion

    #region Properties
    public string Operation => Id != 0
        ? "Editar" : "Cadastrar";
    public bool IsBusy { get; set; }
    public Property InputModel { get; set; } = new();
    public List<FileData> SelectedFileBytes { get; set; } = new();

    #endregion

    #region Services

    [Inject]
    public ISnackbar Snackbar { get; set; } = null!;

    [Inject]
    public IPropertyImageHandler PropertyImageHandler { get; set; } = null!;

    [Inject]
    public IPropertyHandler PropertyHandler { get; set; } = null!;

    [Inject]
    public NavigationManager NavigationManager { get; set; } = null!;

    #endregion

    #region Methods

    public async Task OnValidSubmitAsync()
    {
        IsBusy = true;
        try
        {
            Response<Property?> result;

            if (InputModel.Id > 0)
                result = await PropertyHandler.UpdateAsync(InputModel);
            else
                result = await PropertyHandler.CreateAsync(InputModel);

            var resultMessage = result.Message ?? string.Empty;

            if (result.IsSuccess)
            {
                var request = new CreatePropertyImageRequest
                {
                    PropertyId = result.Data?.Id ?? 0,
                    FileBytes = SelectedFileBytes
                };
                var resultImages = await PropertyImageHandler.CreateAsync(request);
                if (!resultImages.IsSuccess)
                    Snackbar.Add(resultImages.Message ?? string.Empty, Severity.Error);

                Snackbar.Add(resultMessage, Severity.Success);
                NavigationManager.NavigateTo("/imoveis");
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

    private async Task LoadPropertyAsync()
    {
        GetPropertyByIdRequest? request = null;

        try
        {
            request = new GetPropertyByIdRequest { Id = Id };
        }
        catch
        {
            Snackbar.Add("Parâmetro inválido", Severity.Error);
        }

        if (request is null) return;

        var response = await PropertyHandler.GetByIdAsync(request);
        if (response is { IsSuccess: true, Data: not null })
        {
            InputModel.Id = response.Data.Id;
            InputModel.Title = response.Data.Title;
            InputModel.Description = response.Data.Description;
            InputModel.Price = response.Data.Price;
            InputModel.PropertyType = response.Data.PropertyType;
            InputModel.Bedroom = response.Data.Bedroom;
            InputModel.Bathroom = response.Data.Bathroom;
            InputModel.Garage = response.Data.Garage;
            InputModel.Area = response.Data.Area;
            InputModel.TransactionsDetails = response.Data.TransactionsDetails;
            InputModel.Address.ZipCode = response.Data.Address.ZipCode;
            InputModel.Address.Street = response.Data.Address.Street;
            InputModel.Address.Number = response.Data.Address.Number;
            InputModel.Address.Complement = response.Data.Address.Complement;
            InputModel.Address.Neighborhood = response.Data.Address.Neighborhood;
            InputModel.Address.City = response.Data.Address.City;
            InputModel.Address.State = response.Data.Address.State;
            InputModel.Address.Country = response.Data.Address.Country;
            InputModel.IsNew = response.Data.IsNew;
            InputModel.PropertyImages = response.Data.PropertyImages;
            InputModel.IsActive = response.Data.IsActive;
            InputModel.UserId = response.Data.UserId;
        }
        else
        {
            Snackbar.Add(response.Message ?? string.Empty, Severity.Error);
            NavigationManager.NavigateTo("/imoveis");
        }
    }

    private void RedirectToCreateProperty()
    {
        InputModel.PropertyType = EPropertyType.Casa;
        NavigationManager.NavigateTo("/imoveis/adicionar");
    }

    public async Task OnFilesChange(IReadOnlyList<IBrowserFile>? files)
    {
        if (files is null) return;
        foreach (var file in files)
        {
            using var ms = new MemoryStream();
            await file.OpenReadStream(long.MaxValue).CopyToAsync(ms);

            SelectedFileBytes.Add(new FileData
            {
                Content = ms.ToArray(),
                ContentType = file.ContentType,
                Name = file.Name
            });
        }
    }
    #endregion

    #region Overrides

    protected override async Task OnInitializedAsync()
    {
        IsBusy = true;
        try
        {
            if (Id != 0)
                await LoadPropertyAsync();
            else
                RedirectToCreateProperty();
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

