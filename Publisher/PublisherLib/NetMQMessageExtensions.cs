using NetMQ;

namespace PublisherLib
{
    internal static class NetMQMessageExtensions
    {
        public static NetMQMessage CreateMessage(string topic, string data)
        {
            var msg = new NetMQMessage();
            msg.Append(topic);
            msg.Append(data);
            return msg;
        }
    }
}
