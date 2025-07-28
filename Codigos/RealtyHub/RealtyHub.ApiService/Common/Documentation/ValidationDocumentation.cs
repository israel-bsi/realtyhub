using RealtyHub.ApiService.Common.Validation;

namespace RealtyHub.ApiService.Common.Documentation;

/// <summary>
/// Documentação sobre como aplicar validação de Data Annotations em Minimal APIs.
/// </summary>
/// <remarks>
/// Esta documentação explica como usar o DataAnnotationValidator para validar
/// automaticamente os Data Annotations definidos nos modelos.
/// </remarks>
public static class ValidationDocumentation
{
    /// <summary>
    /// Exemplo de como usar o validador em um endpoint Minimal API.
    /// </summary>
    /// <remarks>
    /// Este exemplo mostra a implementação básica de validação:
    /// 
    /// ```csharp
    /// private static async Task&lt;IResult&gt; HandlerAsync(Customer request)
    /// {
    ///     // 1. Validação APENAS dos Data Annotations
    ///     var validationResult = DataAnnotationValidator.ValidateRecursively(request);
    ///     if (validationResult != null)
    ///     {
    ///         return validationResult; // Retorna BadRequest com erros
    ///     }
    /// 
    ///     // 2. Processamento normal
    ///     // ... lógica do endpoint ...
    /// }
    /// ```
    /// </remarks>
    public static void ExampleUsage() { }

    /// <summary>
    /// Data Annotations validados automaticamente no modelo Customer:
    /// </summary>
    /// <remarks>
    /// 
    /// **Customer.cs:**
    /// - `Name`: [Required], [MaxLength(80)]
    /// - `Email`: [Required], [EmailAddress], [MaxLength(50)]
    /// - `Phone`: [Required], [Phone], [MaxLength(30)]
    /// - `DocumentNumber`: [Required], [MaxLength(20)]
    /// - `Address`: [ValidateComplexType] (valida recursivamente)
    /// 
    /// **Address.cs (validado via ValidateComplexType):**
    /// - `Street`: [Required]
    /// - `Neighborhood`: [Required]
    /// - `Number`: [Required]
    /// - `City`: [Required]
    /// - `State`: [Required]
    /// - `Country`: [Required]
    /// - `ZipCode`: [Required]
    /// 
    /// **Importante:** O validador aplica APENAS os Data Annotations definidos nos modelos.
    /// Não adiciona regras extras de validação.
    /// </remarks>
    public static void ValidationRules() { }

    /// <summary>
    /// Endpoints que implementam validação de Data Annotations:
    /// </summary>
    /// <remarks>
    /// 
    /// **CreateCustomerEndpoint:**
    /// - POST /v1/customers
    /// - Valida Customer + Address recursivamente
    /// - Retorna 400 BadRequest com erros estruturados se inválido
    /// 
    /// **UpdateCustomerEndpoint:**
    /// - PUT /v1/customers/{id}
    /// - Valida Customer + Address recursivamente
    /// - Verifica consistência do ID
    /// - Retorna 400 BadRequest com erros estruturados se inválido
    /// 
    /// **Formato de Resposta de Erro:**
    /// ```json
    /// {
    ///   "Message": "Dados inválidos",
    ///   "Errors": {
    ///     "Name": ["Nome é um campo obrigatório"],
    ///     "Email": ["Email inválido"],
    ///     "Address.Street": ["Logradouro é um campo obrigatório"]
    ///   },
    ///   "IsSuccess": false,
    ///   "Code": 400
    /// }
    /// ```
    /// </remarks>
    public static void ImplementedEndpoints() { }
}