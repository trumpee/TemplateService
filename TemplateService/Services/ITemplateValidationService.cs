using Trumpee.MassTransit.Messages.Notifications;

namespace TemplateService.Services;

/// <summary>
/// Provides an interface for template validation services.
/// </summary>
public interface ITemplateValidationService
{
    /// <summary>
    /// Validates the specified content.
    /// </summary>
    /// <param name="content">The content to validate.</param>
    /// <returns>A ValueTask containing the validation result.</returns>
    ValueTask<ValidationResult> Validate(Content content);
}
