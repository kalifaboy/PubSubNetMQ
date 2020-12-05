using NetMQ;
using NetMQ.Sockets;
using System;

namespace SubscriberExample
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0) throw new Exception("Please specify topic subject in arguments.");

            string topicName = args[0];
            using var subscriberSocket = new SubscriberSocket(">tcp://localhost:10000");
            using var poller = new NetMQPoller { subscriberSocket };
            subscriberSocket.Options.ReceiveHighWatermark = 1000;
            subscriberSocket.Subscribe(topicName);
            Console.WriteLine($"Launching subscriber on topic {topicName}");
            subscriberSocket.ReceiveReady += SubscriberSocket_ReceiveReady;
            
            poller.Run();
        }

        private static void SubscriberSocket_ReceiveReady(object sender, NetMQSocketEventArgs e)
        {
            var msg = e.Socket.ReceiveMultipartMessage();
            Console.WriteLine(msg.Last.ConvertToString());
        }
    }
}
