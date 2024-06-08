using System.Text;
using MassTransit;
using Microsoft.Extensions.Logging;
using Trumpee.MassTransit;
using Trumpee.MassTransit.Messages.Notifications;
using Trumpee.MassTransit.Messages.Notifications.Template;

namespace TemplateService.Services;

/// <summary>
/// Implementation of the template fill service.
/// </summary>
public class TemplateFillService(
    ISendEndpointProvider sendEndpointProvider,
    ILogger<TemplateFillService> logger) : ITemplateFillService
{
    private readonly ISendEndpointProvider _sendEndpointProvider = sendEndpointProvider;
    private readonly ILogger<TemplateFillService> _logger = logger;

    /// <summary>
    /// Fills the template in the specified notification.
    /// </summary>
    /// <param name="notification">The notification containing the template to fill.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task FillTemplate(Notification notification)
    {
        try
        {
            await FillTemplateInternal(notification);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while filling the template.");
            await SendTemplateNotFilledAnalytics(notification, CollectInnerExceptions(e));
        }
    }

    private async Task FillTemplateInternal(Notification notification)
    {
        var vars = MapVariables(notification);

        var filledNotification = notification with
        {
            Content = new Content
            {
                Variables = notification.Content!.Variables,
                Body = Fill(notification.Content.Body, vars),
                Subject = Fill(notification.Content.Subject, vars)
            }
        };

        await Task.WhenAll(
            SendTemplateFilledAnalytics(notification),
            SendNotificationToValidation(filledNotification)
        );
    }

    /// <summary>
    /// Fills the template with the specified variables.
    /// </summary>
    /// <param name="dest">The template string to be filled.</param>
    /// <param name="variables">The dictionary of variables to replace in the template.</param>
    /// <returns>The filled template string.</returns>
    private static string Fill(string dest, Dictionary<string, string> variables)
    {
        if (string.IsNullOrEmpty(dest) || variables.Count == 0)
        {
            return dest;
        }

        var builder = new StringBuilder(dest);
        foreach (var item in variables)
        {
            var replace = "${{" + item.Key + "}}";
            builder.Replace(replace, item.Value);
        }

        return builder.ToString();
    }

    private Dictionary<string, string> MapVariables(Notification notification)
    {
        return notification.Content!.Variables!
            .ToDictionary(x => x.Key, x =>
            {
                var variableValue = GetValueAsString(x.Value);
                return string.IsNullOrEmpty(variableValue) ? string.Empty : variableValue;
            });
    }

    private async Task SendNotificationToValidation(Notification filledNotification)
    {
        var validationEndpoint = await _sendEndpointProvider.GetSendEndpoint(
            new Uri(QueueNames.ValidationQueueName));
        await validationEndpoint.Send(filledNotification);
    }

    private async Task SendTemplateFilledAnalytics(Notification notification)
    {
        const string templateId = "content-section";
        var analyticsEvent = Template.Filled(
            nameof(TemplateFillService), notification.NotificationId, templateId);

        await SendAnalytics(analyticsEvent);
    }

    private async Task SendTemplateNotFilledAnalytics(Notification notification, IEnumerable<string> errors)
    {
        const string templateId = "content-section";
        var analyticsEvent = Template.NotFilled(
            nameof(TemplateFillService), notification.NotificationId, templateId, errors);

        await SendAnalytics(analyticsEvent);
    }

    private async Task SendAnalytics<T>(Trumpee.MassTransit.Messages.Event<T> analyticsEvent)
    {
        var analyticsQueue = new Uri("analytics");
        var analyticsEndpoint = await _sendEndpointProvider.GetSendEndpoint(analyticsQueue);
        await analyticsEndpoint.Send(analyticsEvent);
    }

    private static string? GetValueAsString(Variable valueValue)
    {
        return valueValue.ValueType switch
        {
            "string" or "number" or "boolean" => valueValue.Value!.ToString(),
            _ => null
        };
    }

    private static List<string> CollectInnerExceptions(Exception e)
    {
        var innerExceptions = new List<string>();
        while (e.InnerException != null)
        {
            innerExceptions.Add(e.InnerException.Message);
            e = e.InnerException;
        }

        return innerExceptions;
    }
}