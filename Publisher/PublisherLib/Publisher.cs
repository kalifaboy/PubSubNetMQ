using NetMQ;
using System;

namespace PublisherLib
{
    public class Publisher : IDisposable
    {
        private readonly ReliableServer _server;
        public Publisher(string baseAddress, short basePort)
        {
            _server = new ReliableServer($"{baseAddress}:{basePort}");
        }

        public void Publish(string topic, string data)
        {
            NetMQMessage message = NetMQMessageExtensions.CreateMessage(topic, data);
            _server.Publish(message);
        }

        public void Dispose()
        {
            _server?.Dispose();
        }
    }
}
