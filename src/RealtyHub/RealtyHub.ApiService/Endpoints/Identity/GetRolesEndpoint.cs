﻿using System.Security.Claims;
using RealtyHub.ApiService.Common.Api;
using RealtyHub.Core.Models.Account;

namespace RealtyHub.ApiService.Endpoints.Identity;

public class GetRolesEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/roles", Handler)
            .RequireAuthorization();
    }
    private static Task<IResult> Handler(ClaimsPrincipal user)
    {
        if (user.Identity is null || !user.Identity.IsAuthenticated)
            return Task.FromResult(Results.Unauthorized());

        var identity = (ClaimsIdentity)user.Identity;
        var roles = identity
            .FindAll(identity.RoleClaimType)
            .Select(c => new RoleClaim
            {
                Issuer = c.Issuer,
                OriginalIssuer = c.OriginalIssuer,
                Type = c.Type,
                Value = c.Value,
                ValueType = c.ValueType
            });

        return Task.FromResult<IResult>(TypedResults.Json(roles));
    }
}