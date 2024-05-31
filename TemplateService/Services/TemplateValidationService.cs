using TemplateService.Helpers;
using Trumpee.MassTransit.Messages.Notifications;

namespace TemplateService.Services;

/// <summary>
/// Implementation of the template validation service.
/// </summary>
public class TemplateValidationService : ITemplateValidationService
{
    /// <summary>
    /// Validates the specified content.
    /// </summary>
    /// <param name="content">The content to validate.</param>
    /// <returns>A ValueTask containing the validation result.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the content is null.</exception>
    public ValueTask<ValidationResult> Validate(Content content)
    {
        ArgumentNullException.ThrowIfNull(content, nameof(content));

        var subjectVars = NotificationContentHelper.GetUniqueVariableNames(content.Subject);
        var bodyVars = NotificationContentHelper.GetUniqueVariableNames(content.Body);

        var expectedVariables = subjectVars.Union(bodyVars).ToList();

        var validationResult = ValidateTemplateInternal(content, expectedVariables);
        return ValueTask.FromResult(validationResult);
    }

    private static ValidationResult ValidateTemplateInternal(Content content, List<string> expectedVariables)
    {
        var warnings = new List<string>();

        if (expectedVariables.Count <= 0)
        {
            return new ValidationResult(true, null, NormalizeWarnings(warnings));
        }

        var variablesValidationResult = ValidateVariablesInternal(content, expectedVariables, warnings);
        if (!variablesValidationResult.IsValid)
        {
            return variablesValidationResult;
        }

        return new ValidationResult(true, null, NormalizeWarnings(warnings));
    }

    private static ValidationResult ValidateVariablesInternal(
        Content content, List<string> expectedVariables, List<string> warnings)
    {
        if (content.Variables == null)
        {
            return new ValidationResult(false, "Variables collection is null.", null);
        }

        var actualVariables = content.Variables!.Select(x => x.Key).ToHashSet();

        var missingVariables = expectedVariables.Except(actualVariables).ToList();
        if (missingVariables.Count != 0)
        {
            return new ValidationResult(false,
                $"Missing variables: {string.Join(", ", missingVariables)}", null);
        }

        var extraVariables = actualVariables.Except(expectedVariables).ToList();
        if (extraVariables.Count != 0)
        {
            warnings.Add($"Unexpected variables: {string.Join(", ", extraVariables)}");
        }

        return new ValidationResult(true, null, NormalizeWarnings(warnings));
    }

    private static List<string>? NormalizeWarnings(List<string> warnings)
        => warnings.Count != 0 ? warnings : null;
}
