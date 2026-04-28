namespace Services.Interfaces;

public interface IMessageProducer
{
    void PublishMessage<T>(T message, string queue);
}
