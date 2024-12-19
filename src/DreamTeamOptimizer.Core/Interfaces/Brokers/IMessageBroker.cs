namespace DreamTeamOptimizer.Core.Interfaces.Brokers;

public interface IMessageBroker
{
    Task<bool> Publish(string channel, string message);
    Task<bool> Subscribe(string channel, Action<string, string> handle);
}