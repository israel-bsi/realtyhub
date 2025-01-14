﻿using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RealtyHub.Core.Requests.Account;

public class RegisterRequest : Request
{
    [Required(ErrorMessage = "Creci é obrigatório")]
    public string Creci { get; set; } = string.Empty;
    [Required(ErrorMessage = "Nome é obrigatório")]
    public string GivenName { get; set; } = string.Empty;
    [Required(ErrorMessage = "E-mail é obrigatório")]
    [EmailAddress(ErrorMessage = "E-mail inválido")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Senha inválida")]
    [MinLength(6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres")]
    public string Password { get; set; } = string.Empty;
    [Required(ErrorMessage = "Senha inválida")]
    [MinLength(6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres")]
    [JsonIgnore]
    public string ConfirmPassword { get; set; } = string.Empty;
}