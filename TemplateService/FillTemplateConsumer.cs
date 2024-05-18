using MassTransit;
using Trumpee.MassTransit;
using Trumpee.MassTransit.Messages.Notifications;

namespace TemplateService;

public class FillTemplateConsumer : IConsumer<Notification>
{
    public async Task Consume(ConsumeContext<Notification> context)
    {
        var message = context.Message;
        
        var vars = message.Content!.Variables!
            .ToDictionary(x => x.Key, x => x.Value.Value.ToString());

        var notification = message with
        {
            Content = new Content
            {
                Variables = message.Content.Variables,
                Body = TemplateFiller.FillTemplate(message.Content.Body, vars!),
                Subject = TemplateFiller.FillTemplate(message.Content.Subject, vars!)
            }
        };

        await context.Send(new Uri(QueueNames.ValidationQueueName), notification);
    }
}
