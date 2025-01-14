﻿using RealtyHub.Core.Models.Account;
using RealtyHub.Core.Requests.Account;
using RealtyHub.Core.Responses;

namespace RealtyHub.Core.Handlers;

public interface IAccountHandler
{
    Task<Response<string>> LoginAsync(LoginRequest request);
    Task<Response<string>> RegisterAsync(RegisterRequest request);
    Task<Response<string>> ConfirmEmail(UserConfirmEmail userConfirm);
    Task LogoutAsync();
}