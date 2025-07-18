using System.ComponentModel.DataAnnotations;

namespace RealtyHub.ApiService.Common.Validation;

/// <summary>
/// Validador simples para aplicar Data Annotations em Minimal APIs.
/// Foca apenas nas validações definidas nos atributos do modelo.
/// </summary>
public static class DataAnnotationValidator
{
    /// <summary>
    /// Valida um objeto usando apenas os Data Annotations definidos no modelo.
    /// </summary>
    /// <typeparam name="T">Tipo do objeto a ser validado.</typeparam>
    /// <param name="model">Objeto a ser validado.</param>
    /// <returns>
    /// - null se o modelo for válido
    /// - IResult com BadRequest se houver erros de validação
    /// </returns>
    public static IResult? ValidateDataAnnotations<T>(T model) where T : class
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(model);

        // Valida o objeto usando Data Annotations
        bool isValid = Validator.TryValidateObject(
            model,
            validationContext,
            validationResults,
            validateAllProperties: true
        );

        if (isValid)
        {
            return null; // Modelo válido
        }

        // Coleta as mensagens de erro
        var errors = validationResults
            .Select(vr => vr.ErrorMessage ?? "Erro de validação")
            .ToList();

        var combinedMessage = string.Join("; ", errors);

        // Retorna BadRequest com as mensagens de erro
        return Results.BadRequest(new
        {
            Message = combinedMessage,
            IsSuccess = false,
            Code = 400
        });
    }

    /// <summary>
    /// Valida um objeto recursivamente incluindo propriedades complexas.
    /// Aplica apenas Data Annotations - não adiciona regras extras.
    /// </summary>
    /// <typeparam name="T">Tipo do objeto a ser validado.</typeparam>
    /// <param name="model">Objeto a ser validado.</param>
    /// <returns>
    /// - null se o modelo for válido
    /// - IResult com BadRequest estruturado se houver erros
    /// </returns>
    public static IResult? ValidateRecursively<T>(T model) where T : class
    {
        var allErrors = new Dictionary<string, List<string>>();

        // Valida o objeto principal
        var primaryErrors = GetValidationErrors(model);
        foreach (var error in primaryErrors)
        {
            allErrors[error.Key] = error.Value;
        }

        // Valida propriedades complexas que tenham [ValidateComplexType]
        var properties = typeof(T).GetProperties();
        foreach (var property in properties)
        {
            var validateComplexTypeAttr = property.GetCustomAttributes(typeof(ValidateComplexTypeAttribute), false);
            if (validateComplexTypeAttr.Any())
            {
                var value = property.GetValue(model);
                if (value != null)
                {
                    var propertyErrors = GetValidationErrors(value, property.Name);
                    foreach (var error in propertyErrors)
                    {
                        allErrors[error.Key] = error.Value;
                    }
                }
            }
        }

        if (!allErrors.Any())
        {
            return null; // Modelo válido
        }

        // Retorna resposta estruturada com erros
        return Results.BadRequest(new
        {
            Message = "Dados inválidos",
            Errors = allErrors,
            IsSuccess = false,
            Code = 400
        });
    }

    /// <summary>
    /// Obtém erros de validação de um objeto.
    /// </summary>
    /// <param name="model">Objeto a ser validado.</param>
    /// <param name="propertyPrefix">Prefixo para os nomes das propriedades (usado em validação recursiva).</param>
    /// <returns>Dicionário com nomes dos campos e suas mensagens de erro.</returns>
    private static Dictionary<string, List<string>> GetValidationErrors(object model, string propertyPrefix = "")
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(model);

        Validator.TryValidateObject(
            model,
            validationContext,
            validationResults,
            validateAllProperties: true
        );

        var errors = new Dictionary<string, List<string>>();

        foreach (var validationResult in validationResults)
        {
            var memberNames = validationResult.MemberNames.Any()
                ? validationResult.MemberNames
                : new[] { "General" };

            foreach (var memberName in memberNames)
            {
                var fullPropertyName = string.IsNullOrEmpty(propertyPrefix)
                    ? memberName
                    : $"{propertyPrefix}.{memberName}";

                if (!errors.ContainsKey(fullPropertyName))
                {
                    errors[fullPropertyName] = new List<string>();
                }

                errors[fullPropertyName].Add(validationResult.ErrorMessage ?? "Erro de validação");
            }
        }

        return errors;
    }
}