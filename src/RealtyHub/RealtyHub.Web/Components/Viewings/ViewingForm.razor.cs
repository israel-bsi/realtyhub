using Microsoft.AspNetCore.Components;
using MudBlazor;
using RealtyHub.Core.Enums;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Viewings;
using RealtyHub.Core.Responses;

namespace RealtyHub.Web.Components.Viewings;

public partial class ViewingFormComponent : ComponentBase
{
    #region Parameters

    [Parameter]
    public long Id { get; set; }

    #endregion

    #region Properties
    public string Operation => Id != 0
        ? "Reagendar" : "Agendar";
    public Viewing InputModel { get; set; } = new();
    public TimeSpan? ViewingTime { get; set; }
    public bool IsBusy { get; set; }
    #endregion

    #region Services

    [Inject]
    public IViewingHandler Handler { get; set; } = null!;

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
            if (InputModel.ViewingDate.HasValue && ViewingTime.HasValue)
            {
                InputModel.ViewingDate = InputModel
                    .ViewingDate.Value.Date.Add(ViewingTime.Value)
                    .ToUniversalTime();
            }

            Response<Viewing?> result;
            if (Id == 0)
                result = await Handler.ScheduleAsync(InputModel);
            else
                result = await Handler.RescheduleAsync(InputModel);

            var resultMessage = result.Message ?? string.Empty;
            Snackbar.Add(resultMessage, result.IsSuccess ? Severity.Success : Severity.Error);
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

    private async Task LoadViewingAsync()
    {
        GetViewingByIdRequest? request = null;
        try
        {
            request = new GetViewingByIdRequest { Id = Id };
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
            InputModel.ViewingDate = response.Data.ViewingDate;
            InputModel.ViewingStatus = response.Data.ViewingStatus;
            InputModel.CustomerId = response.Data.CustomerId;
            InputModel.PropertyId = response.Data.PropertyId;
            InputModel.Property = response.Data.Property;
            InputModel.Customer = response.Data.Customer;
            InputModel.UserId = response.Data.UserId;
            ViewingTime = InputModel.ViewingDate!.Value.TimeOfDay;
        }
        else
        {
            Snackbar.Add(response.Message ?? string.Empty, Severity.Error);
            NavigationManager.NavigateTo("/visitas");
        }
    }
    private void RedirectToCreateViewing()
    {
        InputModel.ViewingStatus = EViewingStatus.Scheduled;
        InputModel.ViewingDate = DateTime.Now;
        NavigationManager.NavigateTo("/visitas/agendar");
    }

    public async Task OnClickDoneViewing()
    {
        Snackbar.Add($"Visita {InputModel.Id} finalizada", Severity.Info);
    }

    public async Task OnClickCancelViewing()
    {
        Snackbar.Add($"Visita {InputModel.Id} cancelada", Severity.Info);
    }

    #endregion

    #region Overrides

    protected override async Task OnInitializedAsync()
    {
        IsBusy = true;
        try
        {
            if (Id != 0)
                await LoadViewingAsync();
            else
                RedirectToCreateViewing();
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