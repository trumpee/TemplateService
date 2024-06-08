using Trumpee.MassTransit.Messages.Notifications;

namespace TemplateService.Services;

/// <summary>
/// Provides an interface for services that fill templates.
/// </summary>
public interface ITemplateFillService
{
    /// <summary>
    /// Fills the template in the specified notification.
    /// </summary>
    /// <param name="notification">The notification containing the template to fill.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task FillTemplate(Notification notification);
}
