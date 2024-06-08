using MassTransit;
using Microsoft.Extensions.Logging;
using TemplateService.Services;
using Trumpee.MassTransit.Messages.Notifications;

namespace TemplateService;

/// <summary>
/// Consumer for handling template fill requests.
/// </summary>
/// <param name="templateFillService">The service used to fill templates.</param>
/// <param name="logger">The logger instance used for logging.</param>
public class FillTemplateConsumer(
    ITemplateFillService templateFillService,
    ILogger<FillTemplateConsumer> logger) : IConsumer<Notification>
{
    private readonly ITemplateFillService _templateFillService = templateFillService;
    private readonly ILogger<FillTemplateConsumer> _logger = logger;

    /// <summary>
    /// Consumes the specified notification message and fills the template.
    /// </summary>
    /// <param name="context">The context containing the notification message.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task Consume(ConsumeContext<Notification> context)
    {
        try
        {
            await _templateFillService.FillTemplate(context.Message);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while filling the template");
        }
    }
}
