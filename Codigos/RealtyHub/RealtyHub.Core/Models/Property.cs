﻿using RealtyHub.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace RealtyHub.Core.Models;

public class Property : Entity
{
    public long Id { get; set; }
    public long SellerId { get; set; }
    public long CondominiumId { get; set; }
    [Required(ErrorMessage = "Título é um campo obrigatório")]
    [MaxLength(120, ErrorMessage = "O Título deve conter até 120 caracteres")]
    public string Title { get; set; } = string.Empty;
    [Required(ErrorMessage = "Descrição é um campo obrigatório")]
    [MaxLength(255, ErrorMessage = "A descrição deve conter até 255 caracteres")]
    public string Description { get; set; } = string.Empty;
    [Required(ErrorMessage = "Preço é um campo obrigatório")]
    public decimal Price { get; set; }
    [Required(ErrorMessage = "Tipo de propriedade é um campo obrigatório")]
    public EPropertyType PropertyType { get; set; } = EPropertyType.Apartment;
    [Required(ErrorMessage = "Quartos é um campo obrigatório")]
    public int Bedroom { get; set; }
    [Required(ErrorMessage = "Banheiros é um campo obrigatório")]
    public int Bathroom { get; set; }
    [Required(ErrorMessage = "Garagem é um campo obrigatório")]
    public int Garage { get; set; }
    [Required(ErrorMessage = "Área é um campo obrigatório")]
    public double Area { get; set; }
    [Required(ErrorMessage = "Detalhes de transações é um campo obrigatório")]
    public string TransactionsDetails { get; set; } = string.Empty;
    [ValidateComplexType]
    public Address Address { get; set; } = new();
    [Required(ErrorMessage = "Propriedade nova é um campo obrigatório")]
    public bool IsNew { get; set; }
    [Required(ErrorMessage = "Matriculá no Cartório é um campo obrigatório")]
    public string RegistryNumber { get; set; } = string.Empty;
    [Required(ErrorMessage = "Registro do Cartório é um campo obrigatório")]
    public string RegistryRecord { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool ShowInHome { get; set; }
    public string UserId { get; set; } = string.Empty;
    public List<PropertyPhoto> PropertyPhotos { get; set; } = [];
    public Customer? Seller { get; set; }
    public Condominium Condominium { get; set; } = new();
}