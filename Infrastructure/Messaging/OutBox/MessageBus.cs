using MassTransit;

namespace Infrastructure.Messaging.OutBox
{
    public class MessageBus : IMessageBus
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public MessageBus(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public async Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : class
        {
            await _publishEndpoint.Publish(message, cancellationToken);
        }
    }
}