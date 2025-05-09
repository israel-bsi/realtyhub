﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using RealtyHub.Core.Enums;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Properties;
using RealtyHub.Core.Requests.PropertiesPhotos;
using RealtyHub.Core.Responses;
using RealtyHub.Web.Components.Common;
using RealtyHub.Web.Components.Condominiums;
using RealtyHub.Web.Components.Customers;

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
    public List<PropertyPhoto> PropertyPhotos { get; set; } = [];
    public List<PhotoItem> AllPhotos { get; set; } = [];
    public int SelectedIndex { get; set; }
    public int CarouselKey { get; set; }
    public int DataGridBtnPhotosKey { get; set; }
    #endregion

    #region Services

    [Inject]
    public ISnackbar Snackbar { get; set; } = null!;

    [Inject]
    public IPropertyPhotosHandler PropertyPhotosHandler { get; set; } = null!;

    [Inject]
    public IPropertyHandler PropertyHandler { get; set; } = null!;

    [Inject]
    public NavigationManager NavigationManager { get; set; } = null!;

    [Inject]
    public IDialogService DialogService { get; set; } = null!;

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

            if (!result.IsSuccess)
            {
                Snackbar.Add(result.Message ?? string.Empty, Severity.Error);
                return;
            }

            InputModel.Id = result.Data?.Id ?? 0;

            await DeleteRemovedServerPhotos();
            await UpdateServerPhotos();

            Snackbar.Add(result.Message ?? string.Empty, Severity.Success);
            NavigationManager.NavigateTo("/imoveis");
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
    public async Task RemoveAllPhotos()
    {
        var parameters = new DialogParameters
        {
            { "ContentText", "Deseja realmente excluir todas as fotos do imóvel?" },
            { "ButtonColor", Color.Error }
        };

        var dialog = await DialogService.ShowAsync<DialogConfirm>("Confirmação", parameters);
        var result = await dialog.Result;
        if (result is { Canceled: true }) return;

        CarouselKey++;
        AllPhotos.Clear();
        StateHasChanged();
    }
    public async Task RemovePhoto()
    {
        var parameters = new DialogParameters
        {
            { "ContentText", "Deseja realmente excluir a foto selecionada?" },
            { "ButtonColor", Color.Error }
        };

        var dialog = await DialogService.ShowAsync<DialogConfirm>("Confirmação", parameters);
        var result = await dialog.Result;
        if (result is { Canceled: true }) return;

        if (SelectedIndex >= 0 && SelectedIndex < AllPhotos.Count)
        {
            AllPhotos.RemoveAt(SelectedIndex);

            SelectedIndex = AllPhotos.Count > 0
                ? Math.Min(SelectedIndex, AllPhotos.Count - 1)
                : 0;
        }

        CarouselKey++;
        StateHasChanged();
    }
    public async Task OpenImageDialog(string photoUrl)
    {
        var parameters = new DialogParameters { ["ImageUrl"] = photoUrl };
        var options = new DialogOptions
        {
            CloseOnEscapeKey = true,
            MaxWidth = MaxWidth.Large,
            FullWidth = true
        };
        await DialogService.ShowAsync<ImageDialog>(null, parameters, options);
    }
    public async Task OnFilesChange(IReadOnlyList<IBrowserFile>? files)
    {
        if (files is null) return;
        foreach (var file in files)
        {
            using var ms = new MemoryStream();
            await file.OpenReadStream(long.MaxValue).CopyToAsync(ms);

            var newBytes = ms.ToArray();
            var base64 = Convert.ToBase64String(newBytes);
            var dataUri = $"data:{file.ContentType};base64,{base64}";

            AllPhotos.Add(new PhotoItem
            {
                DisplayUrl = dataUri,
                Content = newBytes,
                ContentType = file.ContentType,
                OriginalFileName = file.Name
            });
        }

        CarouselKey++;
        DataGridBtnPhotosKey++;
        StateHasChanged();
    }
    public async Task LoadPhotosFromServerAsync()
    {
        try
        {
            var request = new GetAllPropertyPhotosByPropertyRequest { PropertyId = Id };
            var response = await PropertyPhotosHandler.GetAllByPropertyAsync(request);
            if (response is { IsSuccess: true, Data: not null })
            {
                PropertyPhotos = response.Data;
                foreach (var photo in PropertyPhotos)
                {
                    AllPhotos.Add(new PhotoItem
                    {
                        Id = photo.Id,
                        IsThumbnail = photo.IsThumbnail,
                        DisplayUrl = $"{Configuration.BackendUrl}/photos/{photo.Id}{photo.Extension}",
                    });
                }
            }
            else
                Snackbar.Add(response.Message ?? string.Empty, Severity.Error);
        }
        catch (Exception e)
        {
            Snackbar.Add(e.Message, Severity.Error);
        }
    }
    public async Task UpdateThumbnailsAsync(PhotoItem item)
    {
        if (string.IsNullOrEmpty(item.Id))
        {
            foreach (var photoItem in AllPhotos)
            {
                photoItem.IsThumbnail = photoItem.Id == item.Id;
            }
            return;
        }
        IsBusy = true;
        try
        {
            var request = new UpdatePropertyPhotosRequest
            {
                PropertyId = InputModel.Id,
                Photos =
                [
                    new()
                    {
                        Id = item.Id,
                        IsThumbnail = true,
                        PropertyId = InputModel.Id,
                    }
                ]
            };

            var result = await PropertyPhotosHandler.UpdateAsync(request);
            if (!result.IsSuccess)
            {
                Snackbar.Add(result.Message ?? string.Empty, Severity.Error);
                return;
            }

            Snackbar.Add("Foto definida como principal", Severity.Success);

            foreach (var photoItem in AllPhotos)
            {
                photoItem.IsThumbnail = photoItem.Id == item.Id;
            }

            CarouselKey++;
            StateHasChanged();
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
    private async Task UpdateServerPhotos()
    {
        var newPhotos = AllPhotos.Where(p => string.IsNullOrEmpty(p.Id)).ToList();
        if (newPhotos.Count > 0)
        {
            var request = new CreatePropertyPhotosRequest
            {
                PropertyId = InputModel.Id,
                FileBytes = newPhotos.Select(p => new FileData
                {
                    Id = p.Id,
                    Content = p.Content!,
                    ContentType = p.ContentType!,
                    Name = p.OriginalFileName!,
                    IsThumbnail = p.IsThumbnail
                }).ToList()
            };

            var resultPhotos = await PropertyPhotosHandler.CreateAsync(request);
            if (!resultPhotos.IsSuccess)
                Snackbar.Add(resultPhotos.Message ?? string.Empty, Severity.Error);
        }
    }
    private async Task DeleteRemovedServerPhotos()
    {
        var oldServerPhotosIds = PropertyPhotos
            .Select(p => p.Id)
            .ToList();

        var currentServerPhotosIds = AllPhotos
            .Where(p => !string.IsNullOrEmpty(p.Id))
            .Select(p => p.Id)
            .ToList();

        var removedIds = oldServerPhotosIds.Except(currentServerPhotosIds).ToList();

        foreach (var photoId in removedIds)
        {
            var deleteReq = new DeletePropertyPhotoRequest { Id = photoId, PropertyId = InputModel.Id };
            var resp = await PropertyPhotosHandler.DeleteAsync(deleteReq);
            if (!resp.IsSuccess)
            {
                Snackbar.Add(resp.Message ?? $"Erro ao excluir foto {photoId}", Severity.Error);
            }
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
            InputModel = response.Data;
            await LoadPhotosFromServerAsync();
        }
        else
        {
            Snackbar.Add(response.Message ?? string.Empty, Severity.Error);
            NavigationManager.NavigateTo("/imoveis");
        }
    }
    private void RedirectToCreateProperty()
    {
        InputModel.PropertyType = EPropertyType.House;
        NavigationManager.NavigateTo("/imoveis/adicionar");
    }
    public async Task OpenSellerDialog()
    {
        var parameters = new DialogParameters
        {
            { "OnCustomerSelected", EventCallback.Factory
                .Create<Customer>(this, SelectedSeller) }
        };
        var options = new DialogOptions
        {
            CloseButton = true,
            MaxWidth = MaxWidth.Large,
            FullWidth = true
        };
        var dialog = await DialogService
            .ShowAsync<CustomerDialog>("Informe o vendedor", parameters, options);
        var result = await dialog.Result;

        if (result is { Canceled: false, Data: Customer selectedCustomer })
            SelectedSeller(selectedCustomer);
    }
    private void SelectedSeller(Customer seller)
    {
        InputModel.Seller = seller;
        InputModel.SellerId = seller.Id;
        StateHasChanged();
    }

    public async Task OpenCondominiumDialog()
    {
        var parameters = new DialogParameters
        {
            { "OnCondominiumSelected", EventCallback.Factory
                .Create<Condominium>(this, SelectedCondominium) }
        };
        var options = new DialogOptions
        {
            CloseButton = true,
            MaxWidth = MaxWidth.Large,
            FullWidth = true
        };
        var dialog = await DialogService
            .ShowAsync<CondominiumDialog>("Informe o condomínio", parameters, options);
        var result = await dialog.Result;

        if (result is { Canceled: false, Data: Condominium selectedCondominium })
            SelectedCondominium(selectedCondominium);
    }

    private void SelectedCondominium(Condominium condominium)
    {
        InputModel.Condominium = condominium;
        InputModel.CondominiumId = condominium.Id;
        StateHasChanged();
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