namespace TemplateService.Services;

/// <summary>
/// Represents the result of a template validation.
/// </summary>
/// <param name="IsValid">Indicates whether the template is valid.</param>
/// <param name="ErrorMessage">Contains an error message if the template is invalid.</param>
/// <param name="Warnings">Contains warnings if there are any non-critical issues.</param>
public record ValidationResult(bool IsValid, string? ErrorMessage, List<string>? Warnings);