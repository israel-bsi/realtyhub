using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
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

/// <summary>
/// Componente responsável pelo formulário de cadastro e edição de imóveis, incluindo upload, remoção e definição de fotos.
/// </summary>
public partial class PropertyFormComponent : ComponentBase
{
    #region Parameters

    /// <summary>
    /// Identificador do imóvel. Se diferente de zero, o formulário entra em modo de edição.
    /// </summary>
    [Parameter]
    public long Id { get; set; }

    #endregion

    #region Properties

    /// <summary>
    /// Indica a operação atual do formulário ("Editar" ou "Cadastrar").
    /// </summary>
    public string Operation => Id != 0
        ? "Editar" : "Cadastrar";

    /// <summary>
    /// Indica se o formulário está em estado de carregamento ou processamento.
    /// </summary>
    public bool IsBusy { get; set; }

    /// <summary>
    /// Modelo de entrada utilizado para o binding dos campos do formulário.
    /// </summary>
    public Imovel InputModel { get; set; } = new();

    /// <summary>
    /// Lista de fotos do imóvel carregadas do servidor.
    /// </summary>
    public List<FotoImovel> PropertyPhotos { get; set; } = [];

    /// <summary>
    /// Lista de todas as fotos (novas e existentes) exibidas no formulário.
    /// </summary>
    public List<PhotoItem> AllPhotos { get; set; } = [];

    /// <summary>
    /// Índice da foto atualmente selecionada no carrossel.
    /// </summary>
    public int SelectedIndex { get; set; }

    /// <summary>
    /// Chave para forçar atualização do carrossel de fotos.
    /// </summary>
    public int CarouselKey { get; set; }

    /// <summary>
    /// Chave para forçar atualização do botão de fotos no DataGrid.
    /// </summary>
    public int DataGridBtnPhotosKey { get; set; }

    #endregion

    #region Services

    /// <summary>
    /// Serviço para exibição de mensagens e notificações.
    /// </summary>
    [Inject]
    public ISnackbar Snackbar { get; set; } = null!;

    /// <summary>
    /// Handler responsável pelas operações de fotos de imóveis.
    /// </summary>
    [Inject]
    public IPropertyPhotosHandler PropertyPhotosHandler { get; set; } = null!;

    /// <summary>
    /// Handler responsável pelas operações de imóveis.
    /// </summary>
    [Inject]
    public IPropertyHandler PropertyHandler { get; set; } = null!;

    /// <summary>
    /// Serviço de navegação para redirecionamento de páginas.
    /// </summary>
    [Inject]
    public NavigationManager NavigationManager { get; set; } = null!;

    /// <summary>
    /// Serviço de diálogo para exibição de modais.
    /// </summary>
    [Inject]
    public IDialogService DialogService { get; set; } = null!;

    #endregion

    #region Methods

    /// <summary>
    /// Manipula o envio válido do formulário, realizando a criação ou atualização do imóvel e o gerenciamento das fotos.
    /// </summary>
    /// <remarks>
    /// Cria ou atualiza o imóvel via handler, gerencia o upload, atualização e remoção de fotos no servidor,
    /// exibe mensagens de sucesso ou erro via Snackbar e redireciona para a listagem de imóveis em caso de sucesso.
    /// </remarks>
    public async Task OnValidSubmitAsync()
    {
        IsBusy = true;
        try
        {
            Response<Imovel?> result;

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

    /// <summary>
    /// Remove todas as fotos do imóvel após confirmação do usuário.
    /// </summary>
    /// <remarks>
    /// Exibe um diálogo de confirmação antes de limpar a lista de fotos exibidas no formulário.
    /// </remarks>
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

    /// <summary>
    /// Remove a foto atualmente selecionada do imóvel após confirmação do usuário.
    /// </summary>
    /// <remarks>
    /// Exibe um diálogo de confirmação antes de remover a foto selecionada da lista.
    /// </remarks>
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

    /// <summary>
    /// Abre um diálogo para exibir a imagem em tamanho ampliado.
    /// </summary>
    /// <param name="photoUrl">URL da foto a ser exibida.</param>
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

    /// <summary>
    /// Manipula o upload de arquivos de imagem, convertendo-os para base64 e adicionando à lista de fotos.
    /// </summary>
    /// <param name="files">Lista de arquivos selecionados pelo usuário.</param>
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

    /// <summary>
    /// Carrega as fotos do imóvel a partir do servidor e popula a lista de fotos exibidas.
    /// </summary>
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
                        IsThumbnail = photo.Miniatura,
                        DisplayUrl = $"{Configuration.BackendUrl}/photos/{photo.Id}{photo.Extensao}",
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

    /// <summary>
    /// Atualiza a foto principal (thumbnail) do imóvel no servidor e na interface.
    /// </summary>
    /// <param name="item">Foto a ser definida como principal.</param>
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
                        Miniatura = true,
                        ImovelId = InputModel.Id,
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

    /// <summary>
    /// Envia as novas fotos adicionadas no formulário para o servidor.
    /// </summary>
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

    /// <summary>
    /// Remove do servidor as fotos que foram excluídas no formulário.
    /// </summary>
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

    /// <summary>
    /// Carrega os dados do imóvel para edição a partir do ID informado.
    /// </summary>
    /// <remarks>
    /// Busca os dados do imóvel via handler, preenche o modelo de entrada e carrega as fotos do servidor.
    /// Em caso de erro, exibe mensagem e redireciona para a listagem de imóveis.
    /// </remarks>
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

    /// <summary>
    /// Redireciona para a tela de cadastro de novo imóvel, preenchendo valores padrão.
    /// </summary>
    private void RedirectToCreateProperty()
    {
        InputModel.TipoImovel = ETipoImovel.Casa;
        NavigationManager.NavigateTo("/imoveis/adicionar");
    }

    /// <summary>
    /// Abre o diálogo para seleção do vendedor do imóvel.
    /// </summary>
    public async Task OpenSellerDialog()
    {
        var parameters = new DialogParameters
        {
            { "OnCustomerSelected", EventCallback.Factory
                .Create<Cliente>(this, SelectedSeller) }
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

        if (result is { Canceled: false, Data: Cliente selectedCustomer })
            SelectedSeller(selectedCustomer);
    }

    /// <summary>
    /// Manipula a seleção do vendedor no diálogo, atualizando o modelo do imóvel.
    /// </summary>
    /// <param name="seller">Vendedor selecionado.</param>
    private void SelectedSeller(Cliente seller)
    {
        InputModel.Vendedor = seller;
        InputModel.VendedorId = seller.Id;
        StateHasChanged();
    }

    /// <summary>
    /// Abre o diálogo para seleção do condomínio do imóvel.
    /// </summary>
    public async Task OpenCondominiumDialog()
    {
        var parameters = new DialogParameters
        {
            { "OnCondominiumSelected", EventCallback.Factory
                .Create<Condominio>(this, SelectedCondominium) }
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

        if (result is { Canceled: false, Data: Condominio selectedCondominium })
            SelectedCondominium(selectedCondominium);
    }

    /// <summary>
    /// Manipula a seleção do condomínio no diálogo, atualizando o modelo do imóvel.
    /// </summary>
    /// <param name="condominio">Condomínio selecionado.</param>
    private void SelectedCondominium(Condominio condominio)
    {
        InputModel.Condominio = condominio;
        InputModel.CondominioId = condominio.Id;
        StateHasChanged();
    }

    #endregion

    #region Overrides

    /// <summary>
    /// Inicializa o componente, carregando os dados do imóvel para edição ou preparando para cadastro.
    /// </summary>
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